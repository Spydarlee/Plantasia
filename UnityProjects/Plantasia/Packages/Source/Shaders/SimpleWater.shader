Shader "Unlit/SimpleWater"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, .5) 
		_MainTex ("Main Texture", 2D) = "white" {}
		_NoiseTex("Extra Wave Noise", 2D) = "white" {}
		_Speed("Wave Speed", Range(0,1)) = 0.5
		_Amount("Wave Amount", Range(0,1)) = 0.5
		_Height("Wave Height", Range(0,1)) = 0.5
		_Foam("Foamline Thickness", Range(0,3)) = 0.5		
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD1;
			};

			float4 _Tint;
			uniform sampler2D _CameraDepthTexture; // Main camera depth Texture - global Unity variable
			sampler2D _MainTex, _NoiseTex;
			float4 _MainTex_ST;
			float _Speed, _Amount, _Height, _Foam;
			
			v2f vert (appdata v)
			{
				v2f o;

				// Optionally get additional noise data from a texture
				float4 noiseTex = tex2Dlod(_NoiseTex, float4(v.uv.xy, 0, 0));

				// Adjust vertex height based on sin wave + noise texture
				v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * noiseTex)) * _Height;

				// Standard vertex shader business
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// Get vertex's screen position for use in fragment shader
				o.scrPos = ComputeScreenPos(o.vertex);
				
				// Return the final vertex data struct
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{			
				// Calculate base colour from texture + tint colour
				half4 col = tex2D(_MainTex, i.uv) * _Tint;

				// Get depth value from main camera's depth texture
				half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));

				// Compare depth to screen position to work out if we should draw the foam line/water edge
				half4 foamLine = 1 - saturate(_Foam * (depth - i.scrPos.w));

				// Add (tinted) foam to base colour and return result
				col += foamLine * _Tint;
				return col;
			}
			ENDCG
		}
	}
}