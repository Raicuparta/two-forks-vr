using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace TwoForksVr.VrLaser
{
    public class VrLaserInputModule : StandaloneInputModule
    {
        private const float rayMaxDistance = 30f;
        private VrLaser laser;
        private readonly SteamVR_Action_Boolean clickAction = SteamVR_Actions.default_Interact;

        public static void Create(VrLaser laser)
        {
            var instance = laser.gameObject.AddComponent<VrLaserInputModule>();
            instance.laser = laser;
            Input.simulateMouseWithTouches = true;
        }

        public override void Process()
        {
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
            
            var pointerData = GetTouchPointerEventData(new Touch()
            {
                position =  Camera.main.WorldToScreenPoint(hit.point), // TODO dont use camera.main
            }, out _, out _);
            
            ProcessTouchPress(pointerData, clickAction.stateDown, clickAction.stateUp);

            if (clickAction.stateUp)
            {
                RemovePointerData(pointerData);
            }
            else
            {
                ProcessMove(pointerData);
            }
        }
    }
}