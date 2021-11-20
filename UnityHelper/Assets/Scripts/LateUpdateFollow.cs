using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raicuparta.TwoForksVr.UnityHelper {
    public class LateUpdateFollow : MonoBehaviour {
        public Transform Target;
        
        void LateUpdate () {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
            transform.localScale = Target.localScale;
        }
    }
}
