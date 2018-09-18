Shader "ScreenTint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		// canvas effect
		_CanvasTex("Canvas Texture", 2D) = "white" {}
		_CanvasPower("Canvas Power", Range(0, 1)) = 0.5

		// color effect
		_InnerColor("Inner Color", Color) = (1, 1, 1, 1)
		_OuterColor("Outer Color", Color) = (1, 1, 1, 1)
		_ColorPower("Color Power", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			sampler2D _CanvasTex;
			float4 _CanvasTex_ST;
			float4 _CanvasTex_TexelSize;
			float _CanvasPower;

			float4 _InnerColor;
			float4 _OuterColor;
			float _ColorPower;

			float3 _CameraPosition;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _CanvasTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample render texture
				fixed4 render = tex2D(_MainTex, i.uv);

				// sample canvas texture
				fixed4 canvas = tex2D(_CanvasTex, i.uv2);

				// blend render and canvas textures
				float4 canvasBlend = lerp(render, canvas, _CanvasPower);

				// create color gradient based on distance from center
				float2 center = float2(0.5f, 0.5f);
				float distanceFromCenter = length(i.uv - center);
				float4 color = lerp(_InnerColor, _OuterColor, distanceFromCenter);

				return lerp(canvasBlend, color, _ColorPower);
			}

			ENDCG
		}
	}
}
