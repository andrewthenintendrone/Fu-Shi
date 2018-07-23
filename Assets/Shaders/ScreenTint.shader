Shader "ScreenTint"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintTex("Tint Texture", 2D) = "white" {}
		_Power("Power", Range(0, 1)) = 0.5
		_InnerColor("Inner Color", Color) = (1, 1, 1, 1)
		_OuterColor("Outer Color", Color) = (1, 1, 1, 1)
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
			sampler2D _TintTex;
			float4 _TintTex_ST;
			float _Power;
			float4 _InnerColor;
			float4 _OuterColor;
			float _camX;
			float _camY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _TintTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 offset = float2(_camX, _camY) * 0.01f;
				fixed4 render = tex2D(_MainTex, i.uv);
				fixed4 canvas = tex2D(_TintTex, i.uv2 + offset);
				float4 canvasBlend = lerp(render, canvas, _Power);

				float2 center = float2(0.5f, 0.5f);
				float distanceFromCenter = length(i.uv - center);

				float4 color = lerp(_InnerColor, _OuterColor, distanceFromCenter);
				return canvasBlend * color;
			}
			ENDCG
		}
	}
}
