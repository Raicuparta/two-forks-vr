// Based on https://github.com/googlearchive/tango-examples-unity/blob/master/TangoWithCardboardExperiments/Assets/Cardboard/Scripts/GazeInputModule.cs

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
    [AddComponentMenu("Cardboard/GazeInputModule")]
    public class LaserInputModuleNew : BaseInputModule
    {
        public Camera EventCamera;

        private Laser laser;
        private Vector3 lastHeadPose;
        private PointerEventData pointerData;

        public static LaserInputModuleNew Create(Laser laser)
        {
            var instance = laser.gameObject.AddComponent<LaserInputModuleNew>();
            instance.laser = laser;
            return instance;
        }

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
            CastRayFromGaze();
            UpdateCurrentObject();

            // Handle input
            if (!SteamVR_Actions._default.Interact.stateDown && SteamVR_Actions._default.Interact.state)
                // Drag is only supported if TapIsTrigger is false.
                HandleDrag();
            else if (!pointerData.eligibleForClick && SteamVR_Actions._default.Interact.stateDown)
                // New trigger action.
                HandleTrigger();
            else if (!SteamVR_Actions._default.Interact.state)
                // Check if there is a pending click to handle.
                HandlePendingClick();
        }

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
            if (!pointerData.dragging || !moving || pointerData.pointerDrag == null) return;
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
            if (pointerData.pointerDrag != null)
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
    }
}