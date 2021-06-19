using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace Raicuparta.TwoForksVR
{
    public class VRHand: MonoBehaviour
    {
        public bool IsLeft = false;

        private VRNode vrNode;

        private void Start()
        {
            name = $"{(IsLeft ? "Left" : "Right")} Hand";
            transform.SetParent(Camera.main.transform.parent, false); // TODO make sure camera is initialized?
            vrNode = IsLeft ? VRNode.LeftHand : VRNode.RightHand;

            if (IsLeft)
            {
                var handModel = transform.Find("handModel");
                handModel.localScale = new Vector3(-handModel.localScale.x, handModel.localScale.y, handModel.localScale.z);
                SetUpWeddingRing();
            }
        }

        private void SetUpWeddingRing()
        {
            var weddingRing = GameObject.Find("HenryWeddingRing 1").transform;
            var socket = transform.Find("handModel/weddingRingSocket");
            weddingRing.SetParent(socket);
            weddingRing.localPosition = Vector3.zero;
            weddingRing.localRotation = Quaternion.identity;
        }

        public void SetMaterial(Material material)
        {
            transform.Find("handModel/hand").GetComponent<MeshRenderer>().material = material;
        }

        private void LateUpdate()
        {
            transform.localPosition = InputTracking.GetLocalPosition(vrNode);
            transform.localRotation = InputTracking.GetLocalRotation(vrNode);
        }
    }
}
