using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrFoot : MonoBehaviour
    {
        private string footName;
        private bool isLeft;

        public static VrFoot Create(bool isLeft = false)
        {
            var footName = isLeft ? "Left" : "Right";
            var instance = new GameObject($"Vr{footName}Foot").AddComponent<VrFoot>();
            instance.isLeft = isLeft;
            instance.footName = footName;
            return instance;
        }

        public void SetUp(Transform playerRootBone)
        {
            if (!playerRootBone) return;

            var shoeBone = playerRootBone.Find(
                $"henryPelvis/henryHips/henryLeg{footName}1/henryLeg{footName}2/henryLeg{footName}Foot");
            if (!shoeBone)
            {
                Logs.LogError("### could not find shoe bone");
                return;
            }

            var shoeLid = Instantiate(VrAssetLoader.ShoeLid).transform;
            LayerHelper.SetLayer(shoeLid.Find("ShoeLidModel"), GameLayer.PlayerBody);
            shoeLid.SetParent(shoeBone, false);
            if (isLeft) shoeLid.localScale = new Vector3(1, 1, -1);
        }
    }
}