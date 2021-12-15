using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrLaserMouse : StandaloneInputModule
    {
        private VrHandLaser handLaser;
        
        public static void Create(VrHandLaser handLaser)
        {
            var instance = handLaser.gameObject.AddComponent<VrLaserMouse>();
            instance.handLaser = handLaser;
            Input.simulateMouseWithTouches = true;
        }

        private void Update()
        {
            var isHit = Physics.Raycast(transform.position, transform.forward, out var hit, 30, LayerHelper.GetMask(GameLayer.UI));
            if (!isHit)
            {
                handLaser.SetTarget(null);
                return;
            }

            handLaser.SetTarget(hit.point);
            
            var pointerData = GetTouchPointerEventData(new Touch()
            {
                position =  Camera.main.WorldToScreenPoint(hit.point),
            }, out _, out _);
            
            var pressed = false;
            var released = false;
            if (SteamVR_Actions.default_Grip.stateDown)
            {
                pressed = true;
            }
            else if (SteamVR_Actions.default_Grip.stateUp)
            {
                released = true;
            }

            ProcessTouchPress(pointerData, pressed, released);

            if (released)
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