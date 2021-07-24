using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace TwoForksVR.Stage
{
    class IntroFix: MonoBehaviour
    {
        GameObject introManager;

        public static IntroFix Create()
        {
            return new GameObject("VRIntroFix").AddComponent<IntroFix>();
        }

        private void Awake()
        {
            introManager = GameObject.Find("IntroManager");
            if (!introManager)
            {
                return;
            }
            introManager.SetActive(false);
            GameObject.Find("IntroTextAndBackground").SetActive(false);
        }

        private void Start()
        {
            if (introManager)
            {
                introManager.SetActive(true);
            }
        }
    }
}
