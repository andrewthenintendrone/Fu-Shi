Shader "Sprites/FadeShader"
{
	Properties
	{
		_MainTex ("Texture 1", 2D) = "white" {}
		_MainTex2 ("Texture 2", 2D) = "white" {}
		_Fade("Fade", Range(0, 1)) = 0.0
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
			sampler2D _MainTex2;
			float4 _MainTex2_ST;

			// Properties
			float _Fade;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the textures
				fixed4 tex1 = tex2D(_MainTex, i.uv);
				fixed4 tex2 = tex2D(_MainTex2, i.uv);

				fixed4 color = lerp(tex1, tex2, _Fade);

				return color;
			}
			ENDCG
		}
	}
}
