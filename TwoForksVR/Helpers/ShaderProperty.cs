using UnityEngine;

namespace TwoForksVR.Helpers
{
    public static class ShaderProperty
    {
        public static readonly int MainTexture = Shader.PropertyToID("_MainTex");
        public static readonly int Color = Shader.PropertyToID("_Color");
    }
}