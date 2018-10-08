Shader "Unlit/TriplanarShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 1
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
				float4 worldVertex : TEXCOORD1;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldVertex : TEXCOORD1;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.normal = mul(unity_ObjectToWorld, v.normal).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// determine blend
				float3 blend = pow(i.normal, 2);
				blend /= (blend.x + blend.y + blend.z);

				// sample axis
				fixed4 xDiff = tex2D(_MainTex, i.worldVertex.zy / _Scale);
				fixed4 yDiff = tex2D(_MainTex, i.worldVertex.xz / _Scale);
				fixed4 zDiff = tex2D(_MainTex, i.worldVertex.xy / _Scale);

				fixed4 diff = xDiff * blend.x + yDiff * blend.y + zDiff * blend.z;

				return diff;
			}
			ENDCG
		}
	}
}
