using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoForksVr.LaserPointer
{
    public class LaserInputModule : PointerInputModule
    {
        private const float rayMaxDistance = 30f;
        public Camera EventCamera;
        private Laser laser;

        public static LaserInputModule Create(Laser laser)
        {
            var instance = laser.gameObject.AddComponent<LaserInputModule>();
            instance.laser = laser;
            Input.simulateMouseWithTouches = true;
            return instance;
        }

        public override void Process()
        {
            if (!EventCamera) return;

            var isHit = Physics.Raycast(
                transform.position,
                transform.forward,
                out var hit,
                rayMaxDistance,
                LayerHelper.GetMask(GameLayer.UI));

            if (!isHit)
            {
                laser.SetTarget(null);
                return;
            }

            laser.SetTarget(hit.point);

            var pointerData = GetTouchPointerEventData(new Touch
            {
                position = EventCamera.WorldToScreenPoint(hit.point)
            }, out _, out _);

            var stateDown = laser.ClickDown();
            var stateUp = laser.ClickUp();

            ProcessMove(pointerData);
            // ProcessTouchPress(pointerData, stateDown, stateUp);


            if (pointerData.pointerEnter != null)
            {
                var handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerData.pointerEnter);
                if (handler == null) return;

                if (stateDown)
                    ExecuteEvents.ExecuteHierarchy(handler, pointerData, ExecuteEvents.pointerDownHandler);
                // ExecuteEvents.ExecuteHierarchy(handler, pointerData, ExecuteEvents.pointerClickHandler);

                if (stateUp)
                    ExecuteEvents.ExecuteHierarchy(handler, pointerData, ExecuteEvents.pointerUpHandler);
            }

            // if (stateUp)
            //     RemovePointerData(pointerData);
            // else
            //     ProcessMove(pointerData);
        }
    }
}