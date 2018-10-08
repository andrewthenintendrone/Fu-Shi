Shader "Custom/toonShader"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Amount ("Amount", Range(0, 1)) = 0
	}
	SubShader
		{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float _Amount;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldNormal;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			fixed3 c = IN.worldNormal;
			c.z += 0.5;
			o.Albedo = c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
