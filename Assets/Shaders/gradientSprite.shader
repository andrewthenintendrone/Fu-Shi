Shader "Sprites/gradientSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color1 ("Color 1", Color) = (0, 0, 0, 1)
		_Color2("Color 2", Color) = (1, 1, 1, 1)
		_Center("Center", Range(0, 1)) = 0.5
		[KeywordEnum(Vertical, Horizontal, Radial)] _GradientType("GradientType", Float) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		LOD 100

		Cull Off
		Lighting Off
		ZWrite Off
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			// Properties
			float4 _Color1;
			float4 _Color2;
			float _Center;
			float _GradientType;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float4 colorBlend = float4(1, 0, 1, 1);

				// vertical gradient
				if (_GradientType == 0)
				{
					colorBlend = lerp(_Color2, _Color1, clamp(i.uv.y - _Center * 2 + 1, 0, 1));
				}
				// horizontal gradient
				else if (_GradientType == 1)
				{
					colorBlend = lerp(_Color2, _Color1, clamp(i.uv.x - _Center * 2 + 1, 0, 1));
				}
				// radial gradient
				else if (_GradientType == 2)
				{
					float distanceFromCenter = distance(i.uv, float2(0.5, 0.5));
					colorBlend = lerp(_Color2, _Color1, clamp(distanceFromCenter - _Center * 2 + 1, 0, 1));
				}

				return col * colorBlend;
			}
			ENDCG
		}
	}
}
