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
        }
        
        public void ClickAt(Vector3 worldPoint, bool pressed, bool released)
        {
            ClickAt((Vector2) Camera.main.WorldToScreenPoint(worldPoint), pressed, released);
        }
        
        public void ClickAt(Vector2 screenPoint, bool pressed, bool released)
        {
            Input.simulateMouseWithTouches = true;
            var pointerData = GetTouchPointerEventData(new Touch()
            {
                position = new Vector2(screenPoint.x, screenPoint.y),
            }, out _, out _);

            ProcessTouchPress(pointerData, pressed, released);
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

            var screenPoint = (Vector2) Camera.main.WorldToScreenPoint(hit.point);
            if (SteamVR_Actions.default_Grip.state)
            {
                ClickAt(screenPoint, true, false);
            }
            else if (SteamVR_Actions.default_Grip.stateUp)
            {
                ClickAt(screenPoint, false, true);
            }
            else
            {
                ClickAt(screenPoint, false, false);
            }
            

            var pointerData = GetTouchPointerEventData(new Touch()
            {
                position = new Vector2(screenPoint.x, screenPoint.y),
            }, out _, out _);
            
            ProcessMove(pointerData);
        }
    }
}