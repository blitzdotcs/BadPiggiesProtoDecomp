Shader "e2d/Curve" {
Properties {
 _ControlSize ("Control Size", Float) = 1
 _Control ("Control (RGBA)", 2D) = "red" {}
 _Splat0 ("Layer 0 (R)", 2D) = "white" {}
 _SplatParams0 ("Splat Params 0", Vector) = (1,1,0,0)
 _Splat1 ("Layer 1 (G)", 2D) = "white" {}
 _SplatParams1 ("Splat Params 1", Vector) = (1,1,0,0)
 _MainTex ("BaseMap (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}