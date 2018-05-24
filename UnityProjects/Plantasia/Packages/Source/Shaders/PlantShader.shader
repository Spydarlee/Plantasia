// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/PlantShader" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_WiltedColor ("Wilted Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Wilted ("Wilted", Range(0,1)) = 1.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Wilted;
		half _Glossiness;
		half _Metallic;
		fixed4 _TintColor;
		fixed4 _WiltedColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Lerp between texture colour and grayscale depending on if we're wilting or not!
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _TintColor;
			c = lerp(c, dot(_WiltedColor.rgb, c), _Wilted);
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
