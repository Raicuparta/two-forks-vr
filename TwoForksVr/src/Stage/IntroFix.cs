using UnityEngine;

namespace TwoForksVr.Stage
{
    internal class IntroFix : MonoBehaviour
    {
        private GameObject introManager;

        public static IntroFix Create()
        {
            return new GameObject("VrIntroFix").AddComponent<IntroFix>();
        }

        private void Awake()
        {
            introManager = GameObject.Find("IntroManager");
            if (!introManager) return;
            VrStage.FallbackCamera.tag = "MainCamera";
            introManager.SetActive(false);
            GameObject.Find("IntroTextAndBackground").SetActive(false);
        }

        private void Start()
        {
            if (introManager) introManager.SetActive(true);
        }
    }
}