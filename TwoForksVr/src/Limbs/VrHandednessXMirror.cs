using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrHandednessXMirror : MonoBehaviour
    {
        private Vector3 defaultScale;
        private Vector3 mirroredScale;

        private void Awake()
        {
            defaultScale = transform.localScale;
            mirroredScale = new Vector3(-defaultScale.x, defaultScale.y, defaultScale.z);
        }

        private void Update()
        {
            transform.localScale = VrSettings.LeftHandedMode.Value ? mirroredScale : defaultScale;
        }
    }
}