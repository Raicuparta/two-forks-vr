using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrFoot : MonoBehaviour
    {
        public static void Create(Transform playerRootBone, bool isLeft = false)
        {
            var footName = isLeft ? "Left" : "Right";

            if (!playerRootBone) return;

            var shoeBone = playerRootBone.Find(
                $"henryPelvis/henryHips/henryLeg{footName}1/henryLeg{footName}2/henryLeg{footName}Foot");
            if (!shoeBone)
            {
                Logs.LogError("### could not find shoe bone");
                return;
            }

            var shoeLid = Instantiate(VrAssetLoader.ShoeLid).transform;
            shoeLid.gameObject.AddComponent<VrFoot>();
            LayerHelper.SetLayer(shoeLid.Find("ShoeLidModel"), GameLayer.PlayerBody);
            shoeLid.SetParent(shoeBone, false);
            if (isLeft) shoeLid.localScale = new Vector3(1, 1, -1);
        }
        
        private void Awake()
        {
            VrSettings.ShowFeet.SettingChanged += HandleShowFeetChanged;
        }

        private void Start()
        {
            SetUpVisibility();
        }

        private void OnDestroy()
        {
            VrSettings.ShowFeet.SettingChanged -= HandleShowFeetChanged;
        }

        private void HandleShowFeetChanged(object sender, EventArgs e)
        {
            SetUpVisibility();
        }

        private void SetUpVisibility()
        {
            gameObject.SetActive(VrSettings.ShowFeet.Value);
        }
    }
}