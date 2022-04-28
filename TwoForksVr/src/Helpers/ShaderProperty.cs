using UnityEngine;

namespace TwoForksVr.Helpers;

public static class ShaderProperty
{
    public static readonly int MainTexture = Shader.PropertyToID("_MainTex");
    public static readonly int Color = Shader.PropertyToID("_Color");
    public static readonly int UnityGuizTestMode = Shader.PropertyToID("unity_GUIZTestMode");
}