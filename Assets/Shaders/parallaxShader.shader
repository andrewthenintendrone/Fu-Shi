Shader "Unlit/parallaxShader"
{
	Properties
	{
		_Texture1 ("Texture 1", 2D) = "white" {}
		_Texture2 ("Texture 2", 2D) = "white" {}
		_Texture3 ("Texture 3", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
			};

			struct v2f
			{
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Texture1;
			float4 _Texture1_ST;
			sampler2D _Texture2;
			float4 _Texture2_ST;
			sampler2D _Texture3;
			float4 _Texture3_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _Texture1);
				o.uv2 = TRANSFORM_TEX(v.uv2, _Texture2);
				o.uv3 = TRANSFORM_TEX(v.uv3, _Texture3);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 finalColor = tex2D(_Texture1, i.uv1);
				
				finalColor = lerp(finalColor, tex2D(_Texture2, i.uv2), tex2D(_Texture2, i.uv2).a);
				finalColor = lerp(finalColor, tex2D(_Texture3, i.uv3), tex2D(_Texture3, i.uv3).a);

				return finalColor;
			}
			ENDCG
		}
	}
}
