Shader "Custom/GradientShader" {
	Properties {
        _ColorTop ("Top Color", Color) = (0.5,0.5,0.5,1)
        _ColorBottom ("Bottom Color", Color) = (1,1,1,1)
		_GradientOffset ("Gradient Offset", Range(-1,1)) = 0.0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma vertex vert
		#pragma target 3.0

		// Surface shader input
		struct Input 
		{
			float2 uv_MainTex;
			float3 gradientColor;
		};

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		half _GradientOffset;
		fixed4 _ColorTop;
		fixed4 _ColorBottom;

		void vert (inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// Normalised height value where 0 is the bottom of the object and 1 is the top
			half height = saturate((v.vertex.y) + 0.5 + _GradientOffset);

			// Use height to lerp between bottom and top color
			o.gradientColor = lerp(_ColorBottom, _ColorTop, height);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			o.Albedo = c.rgb * IN.gradientColor.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}