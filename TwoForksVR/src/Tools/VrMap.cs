using UnityEngine;

namespace TwoForksVr.Tools
{
    public class VrMap : MonoBehaviour
    {
        private const float mapScale = 2f;

        private void Start()
        {
            AdjustPosition();
            ResetCloth();
        }

        private void LateUpdate()
        {
            ForceHighResolutionMap();
        }

        public static VrMap Create(Transform mapInHand, string handName)
        {
            if (!mapInHand || mapInHand.GetComponent<VrMap>()) return null;

            // Very hard to read the map in VR since we can't zoom in.
            // So making it bigger to make it easier, especially for lower resolution headsets.
            mapInHand.parent.parent.localScale = Vector3.one * mapScale;

            return mapInHand.gameObject.AddComponent<VrMap>();
        }

        // Map doesn't quite fit the hand after scaling, need to move it a bit.
        private void AdjustPosition()
        {
            transform.localPosition = new Vector3(0.01f, 0f, 0.02f);
            transform.localEulerAngles = new Vector3(10f, 0f, 0f);
        }

        // Need to disable and re-enable the cloth component to reset it,
        // otherwise the physics would be all fucky after resizing.
        private void ResetCloth()
        {
            var cloth = gameObject.GetComponent<Cloth>();
            cloth.enabled = false;
            cloth.enabled = true;
        }

        // In base game, map resolution changes depending on zoom. In VR there's no zoom,
        // so we need to always have high resolution. For whatever reason I don't seem to be
        // able to patch the IsZoomingMap method, so I'm forcing it on LateUpdate instead.
        private static void ForceHighResolutionMap()
        {
            if (!vgMapManager.Instance) return;
            var mapController = vgMapManager.Instance.mapController;
            if (!mapController) return;
            mapController.isZoomingMap = true;
        }
    }
}
