using UnityEngine;

namespace TwoForksVr.Stage
{
    internal class IntroFix : MonoBehaviour
    {
        private GameObject introManager;

        private void Awake()
        {
            introManager = GameObject.Find("IntroManager");
            if (!introManager) return;
            VRStage.FallbackCamera.tag = "MainCamera";
            introManager.SetActive(false);
            GameObject.Find("IntroTextAndBackground").SetActive(false);
        }

        private void Start()
        {
            if (introManager) introManager.SetActive(true);
        }

        public static IntroFix Create()
        {
            return new GameObject("VRIntroFix").AddComponent<IntroFix>();
        }
    }
}
