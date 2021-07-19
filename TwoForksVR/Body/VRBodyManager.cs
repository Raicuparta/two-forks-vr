using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using UnityEngine;

namespace TwoForksVR
{
    public class VRBodyManager: MonoBehaviour
    {
        private void Start()
        {
            var playerBodyTransform = GetPlayerBodyTransform();
            //HideBody(playerBodyTransform);

            var handsManager = new GameObject("VRHandsManager").AddComponent<VRHandsManager>();
            handsManager.PlayerBody = playerBodyTransform;
        }

        private void HideBody(Transform bodyTransform)
        {
            bodyTransform.gameObject.SetActive(false);
        }

        private Transform GetPlayerBodyTransform()
        {
            return GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
        }
    }
}
