Shader "Unlit/inkShader"
{
	Properties
	{
		_MainTex("Ink Texture", 2D) = "white" {}
		_PerlinTex("Perlin Noise Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

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
			sampler2D _PerlinTex;
			float4 _PerlinTex_ST;
			float _Cutoff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _PerlinTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample ink texture
				fixed4 ink = tex2D(_MainTex, i.uv);
				fixed4 perlin = tex2D(_PerlinTex, i.uv2);

				fixed4 finalColor = ink;

				// calculate new alpha using cutoff
				if (ink.a * perlin.r * (1 - i.uv.x) < pow(_Cutoff, 2.0))
				{
					finalColor.a = 0;
				}

				return finalColor;
			}
			ENDCG
		}
	}
}
