Shader "Unlit/TimeReverse"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EffectDistance("Effect Distance", Range(0.0, 1.0)) = 0.0
		_EffectScale("Effect Scale", Float) = 0.0
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float2 _PlayerPos;
			float _EffectDistance;
			float _EffectScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 aspectRatio = float2(1, _MainTex_TexelSize.x / _MainTex_TexelSize.y);
				float distanceToPlayer = distance(i.uv * aspectRatio, _PlayerPos * aspectRatio);

				float4 finalColor = tex2D(_MainTex, i.uv);

				if (distanceToPlayer < _EffectDistance)
				{

				}

				return finalColor;
			}
			ENDCG
		}
	}
}
