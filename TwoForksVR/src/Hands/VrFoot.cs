using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Hands
{
    public class VrFoot : MonoBehaviour
    {
        private string handName;
        private bool isLeft;
        
        public static VrFoot Create(bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var instance = new GameObject($"Vr{handName}Foot").AddComponent<VrFoot>();
            instance.isLeft = isLeft;
            instance.handName = handName;
            return instance;
        }

        public void SetUp(Transform playerRootBone)
        {
            if (!playerRootBone) return;

            var shoeBone = playerRootBone.Find(
                                    $"henryPelvis/henryHips/henryLeg{handName}1/henryLeg{handName}2/henryLeg{handName}Foot");
            if (!shoeBone)
            {
                Logs.LogError("### could not find shoe bone");
                return;
            }
            var handLid = Instantiate(VRAssetLoader.ShoeLid).transform;
            LayerHelper.SetLayer(handLid.Find("ShoeLidModel"), GameLayer.PlayerBody);
            handLid.SetParent(shoeBone, false);
            if (isLeft) handLid.localScale = new Vector3(1, 1, -1);
        }
    }
}