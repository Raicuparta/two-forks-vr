Shader "Marmoset/Bumped Specular IBL" {
	Properties {
		_Color ("Diffuse Color", Vector) = (1,1,1,1)
		_SpecColor ("Specular Color", Vector) = (1,1,1,1)
		_SpecInt ("Specular Intensity", Float) = 1
		_Shininess ("Specular Sharpness", Range(2, 8)) = 4
		_Fresnel ("Fresnel Strength", Range(0, 1)) = 0
		_MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
		_SpecTex ("Specular(RGB) Gloss(A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard alpha
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Marmoset/Specular IBL"
}