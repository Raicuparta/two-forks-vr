// The MIT License (MIT)
//
// Copyright (c) 2015, Unity Technologies & Google, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.

using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace TwoForksVr.LaserPointer
{
    /// @ingroup Scripts
    /// This script provides an implemention of Unity's `BaseInputModule` class, so
    /// that Canvas-based (_uGUI_) UI elements can be selected by looking at them and
    /// pulling the Cardboard trigger or touching the screen.
    /// This uses the player's gaze and the Cardboard trigger as a raycast generator.
    /// 
    /// To use, attach to the scene's **EventSystem** object.  Be sure to move it above the
    /// other modules, such as _TouchInputModule_ and _StandaloneInputModule_, in order
    /// for the user's gaze to take priority in the event system.
    /// 
    /// Next, set the **Canvas** object's _Render Mode_ to **World Space**, and set its _Event Camera_
    /// to a (mono) camera that is controlled by a CardboardHead.  If you'd like gaze to work
    /// with 3D scene objects, add a _PhysicsRaycaster_ to the gazing camera, and add a
    /// component that implements one of the _Event_ interfaces (_EventTrigger_ will work nicely).
    /// The objects must have colliders too.
    /// 
    /// GazeInputModule emits the following events: _Enter_, _Exit_, _Down_, _Up_, _Click_, _Select_,
    /// _Deselect_, and _UpdateSelected_.  Scroll, move, and submit/cancel events are not emitted.
    [AddComponentMenu("Cardboard/GazeInputModule")]
    public class LaserInputModuleNew : BaseInputModule
    {
        /// Determines whether gaze input is active in VR Mode only (`true`), or all of the
        /// time (`false`).  Set to false if you plan to use direct screen taps or other
        /// input when not in VR Mode.
        [Tooltip("Whether gaze input is active in VR Mode only (true), or all the time (false).")]
        public bool vrModeOnly;

        public Camera EventCamera;

        /// Time in seconds between the pointer down and up events sent by a Cardboard trigger.
        /// Allows time for the UI elements to make their state transitions.  If you turn off
        /// _TapIsTrigger_ in Cardboard, then this setting has no effect.
        [HideInInspector] public float clickTime = 0.1f; // Based on default time for a button to animate to Pressed.

        /// The pixel through which to cast rays, in viewport coordinates.  Generally, the center
        /// pixel is best, assuming a monoscopic camera is selected as the `Canvas`' event camera.
        [HideInInspector] public Vector2 hotspot = new Vector2(0.5f, 0.5f);

        // Active state
        private bool isActive = false;
        private Laser laser;
        private Vector3 lastHeadPose;

        private PointerEventData pointerData;

        public static LaserInputModuleNew Create(Laser laser)
        {
            var instance = laser.gameObject.AddComponent<LaserInputModuleNew>();
            instance.laser = laser;
            return instance;
        }

        /// @endcond
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            if (pointerData != null)
            {
                HandlePendingClick();
                HandlePointerExitAndEnter(pointerData, null);
                pointerData = null;
            }

            eventSystem.SetSelectedGameObject(null, GetBaseEventData());
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            return pointerData != null && pointerData.pointerEnter != null;
        }

        public override void Process()
        {
            // Save the previous Game Object
            var gazeObjectPrevious = GetCurrentGameObject();

            CastRayFromGaze();
            UpdateCurrentObject();

            // Get the camera
            var camera = pointerData.enterEventCamera;

            // Handle input
            if (!SteamVR_Actions._default.Interact.stateDown && SteamVR_Actions._default.Interact.state)
            {
                // Drag is only supported if TapIsTrigger is false.
                HandleDrag();
            }
            else if (Time.unscaledTime - pointerData.clickTime < clickTime)
            {
                // Delay new events until clickTime has passed.
            }
            else if (!pointerData.eligibleForClick && SteamVR_Actions._default.Interact.stateDown)
            {
                // New trigger action.
                HandleTrigger();
            }
            else if (!SteamVR_Actions._default.Interact.state)
            {
                // Check if there is a pending click to handle.
                HandlePendingClick();
            }
        }

        /// @endcond
        private void CastRayFromGaze()
        {
            var isHit = Physics.Raycast(
                transform.position,
                transform.forward,
                out var hit,
                30,
                LayerHelper.GetMask(GameLayer.UI));

            if (!isHit)
            {
                laser.SetTarget(null);
                return;
            }

            laser.SetTarget(hit.point);

            var pointerPosition = EventCamera.WorldToScreenPoint(hit.point);

            if (pointerData == null)
            {
                pointerData = new PointerEventData(eventSystem);
                lastHeadPose = pointerPosition;
            }

            // Cast a ray into the scene
            pointerData.Reset();
            pointerData.position = pointerPosition;
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            pointerData.delta = pointerPosition - lastHeadPose;
            lastHeadPose = hit.point;
        }

        private void UpdateCurrentObject()
        {
            // Send enter events and update the highlight.
            var go = pointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerData, go);
            // Update the current selection, or clear if it is no longer the current object.
            var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
            if (selected == eventSystem.currentSelectedGameObject)
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
                    ExecuteEvents.updateSelectedHandler);
            else
                eventSystem.SetSelectedGameObject(null, pointerData);
        }

        private void HandleDrag()
        {
            var moving = pointerData.IsPointerMoving();

            if (moving && pointerData.pointerDrag != null && !pointerData.dragging)
            {
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData,
                    ExecuteEvents.beginDragHandler);
                pointerData.dragging = true;
            }

            // Drag notification
            if (pointerData.dragging && moving && pointerData.pointerDrag != null)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (pointerData.pointerPress != pointerData.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

                    pointerData.eligibleForClick = false;
                    pointerData.pointerPress = null;
                    pointerData.rawPointerPress = null;
                }

                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
            }
        }

        private void HandlePendingClick()
        {
            if (!pointerData.eligibleForClick) return;

            var go = pointerData.pointerCurrentRaycast.gameObject;

            // Send pointer up and click events.
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);

            if (pointerData.pointerDrag != null)
                ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.dropHandler);

            if (pointerData.pointerDrag != null && pointerData.dragging)
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);

            // Clear the click state.
            pointerData.pointerPress = null;
            pointerData.rawPointerPress = null;
            pointerData.eligibleForClick = false;
            pointerData.clickCount = 0;
            pointerData.pointerDrag = null;
            pointerData.dragging = false;
        }

        private void HandleTrigger()
        {
            var go = pointerData.pointerCurrentRaycast.gameObject;

            // Send pointer down event.
            pointerData.pressPosition = pointerData.position;
            pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
            pointerData.pointerPress =
                ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler)
                ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

            // Save the drag handler as well
            pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
            if (pointerData.pointerDrag != null && !false)
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);

            // Save the pending click state.
            pointerData.rawPointerPress = go;
            pointerData.eligibleForClick = true;
            pointerData.delta = Vector2.zero;
            pointerData.dragging = false;
            pointerData.useDragThreshold = true;
            pointerData.clickCount = 1;
            pointerData.clickTime = Time.unscaledTime;
        }

        private Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords)
        {
            cartCoords.Normalize();
            if (cartCoords.x == 0)
                cartCoords.x = Mathf.Epsilon;
            var outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
            if (cartCoords.x < 0)
                outPolar += Mathf.PI;
            var outElevation = Mathf.Asin(cartCoords.y);
            return new Vector2(outPolar, outElevation);
        }

        private GameObject GetCurrentGameObject()
        {
            if (pointerData != null && pointerData.enterEventCamera != null)
                return pointerData.pointerCurrentRaycast.gameObject;

            return null;
        }

        private Vector3 GetIntersectionPosition()
        {
            // Check for camera
            var cam = pointerData.enterEventCamera;
            if (cam == null) return Vector3.zero;

            var intersectionDistance = pointerData.pointerCurrentRaycast.distance + cam.nearClipPlane;
            var intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;

            return intersectionPosition;
        }
    }
}