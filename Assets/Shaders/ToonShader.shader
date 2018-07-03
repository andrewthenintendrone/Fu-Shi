Shader "Unlit/ToonShader"
{
	// unity properties
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {} // Diffuse texture
		_Color("Color", Color) = (1, 1, 1, 1) // tint color
		_ColorMask("ColorMask", 2D) = "black" {} // mask for tinting color
		_Shadow("Shadow", Range(0, 1)) = 0.4 // shadow instensity
		_outline_width("outline_width", Float) = 0.2 // outline width
		_outline_color("outline_color", Color) = (0.5,0.5,0.5,1) // outline color
		_BumpMap("BumpMap", 2D) = "bump" {} // bump map
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5 // alpha cutoff

												  // Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _OutlineMode("__outline_mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}


	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			// use forward lighting
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			// write to z buffer
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM

			// include base toon lighting
			#include "ToonCore.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
