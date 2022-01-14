Shader "Marmoset/Transparent/Cutout/Bumped Diffuse IBL Double Faces" {
	Properties {
		_Color ("Diffuse Color", Vector) = (1,1,1,1)
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
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
	Fallback "Marmoset/Transparent/Cutout/Diffuse IBL"
}