using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace TwoForksVR
{
    class IntroFix: MonoBehaviour
    {
        GameObject introManager;

        private void Start()
        {
            VRSettings.enabled = false;
            Invoke(nameof(Init), 1);
            Invoke(nameof(After), 2);
        }

        private void Init()
        {
            introManager = GameObject.Find("IntroManager");
            introManager.SetActive(false);
            GameObject.Find("IntroTextAndBackground").SetActive(false);
            VRSettings.enabled = true;
        }

        private void After()
        {
            introManager.SetActive(true);
        }
    }
}
