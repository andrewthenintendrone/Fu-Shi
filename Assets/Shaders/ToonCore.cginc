#ifndef TOON_CORE

// include all the nice unity lighting stuff
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"

// texture uniforms
uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
uniform sampler2D _ColorMask; uniform float4 _ColorMask_ST;
uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;

// other uniforms
uniform float4 _Color;
uniform float _Shadow;
uniform float _Cutoff;
uniform float _outline_width;
uniform float4 _outline_color;

// multiply a color by this vector to make it grayscale
static const float3 grayscale_vector = float3(0, 0.3823529, 0.01845836);

// vertex shader to geometry shader
struct v2g
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float4 posWorld : TEXCOORD2;
	float3 normalDir : TEXCOORD3;
	float3 tangentDir : TEXCOORD4;
	float3 bitangentDir : TEXCOORD5;
	float4 pos : CLIP_POS;
	SHADOW_COORDS(6)
	UNITY_FOG_COORDS(7)
};

// vertex shader
v2g vert(appdata_full v)
{
	// create output v2g
	v2g o;

	// copy uvs
	o.uv0 = v.texcoord;
	o.uv1 = v.texcoord1;

	// object space normal and tangent
	o.normal = v.normal;
	o.tangent = v.tangent;

	// TBN matrix
	o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
	o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
	o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);

	// world position
	float4 objPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
	o.posWorld = mul(unity_ObjectToWorld, v.vertex);

	// light color
	float3 lightColor = _LightColor0.rgb;

	// model position
	o.vertex = v.vertex;

	// clip position
	o.pos = UnityObjectToClipPos(v.vertex);

	// unity macros for shadows and fog
	TRANSFER_SHADOW(o);
	UNITY_TRANSFER_FOG(o, o.pos);

	// move on to the geometry shader
	return o;
}

// geometry shader to fragment shader
struct VertexOutput
{
	float4 pos : SV_POSITION;
	float2 uv0 : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float4 posWorld : TEXCOORD2;
	float3 normalDir : TEXCOORD3;
	float3 tangentDir : TEXCOORD4;
	float3 bitangentDir : TEXCOORD5;
	float4 col : COLOR;
	bool is_outline : IS_OUTLINE;
	SHADOW_COORDS(6)
	UNITY_FOG_COORDS(7)
};

// geometry shader
[maxvertexcount(6)]
void geom(triangle v2g IN[3], inout TriangleStream<VertexOutput> tristream)
{
	VertexOutput o;

	// outline
	#if !NO_OUTLINE

	// create outline geometry
	for (int i = 2; i >= 0; i--)
	{
		// offset the vertex position along its normal multiplied by outline width
		o.pos = UnityObjectToClipPos(IN[i].vertex + normalize(IN[i].normal) * (_outline_width * .01));

		// uvs stay the same
		o.uv0 = IN[i].uv0;
		o.uv1 = IN[i].uv1;

		// set outline color
		o.col = fixed4( _outline_color.r, _outline_color.g, _outline_color.b, 1);

		// recalculate world position
		o.posWorld = mul(unity_ObjectToWorld, IN[i].vertex);

		// recalculate TBN matrix
		o.normalDir = UnityObjectToWorldNormal(IN[i].normal);
		o.tangentDir = IN[i].tangentDir;
		o.bitangentDir = IN[i].bitangentDir;

		// recalculate world position and mark vertex as an outline
		o.posWorld = mul(unity_ObjectToWorld, IN[i].vertex);
		o.is_outline = true;

		// Pass-through the shadow coordinates if this pass has shadows.
		#if defined (SHADOWS_SCREEN) || ( defined (SHADOWS_DEPTH) && defined (SPOT) ) || defined (SHADOWS_CUBE)
		o._ShadowCoord = IN[i]._ShadowCoord;
		#endif

		// Pass-through the fog coordinates if this pass has shadows.
		#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		o.fogCoord = IN[i].fogCoord;
		#endif

		tristream.Append(o);
	}

	// return to the start of the triangles
	tristream.RestartStrip();
	#endif

	// append existing geometry
	for (int ii = 0; ii < 3; ii++)
	{
		// copy vertex data and mark as non outline
		o.pos = UnityObjectToClipPos(IN[ii].vertex);
		o.uv0 = IN[ii].uv0;
		o.uv1 = IN[ii].uv1;
		o.col = fixed4(1., 1., 1., 0.);
		o.posWorld = mul(unity_ObjectToWorld, IN[ii].vertex);
		o.normalDir = UnityObjectToWorldNormal(IN[ii].normal);
		o.tangentDir = IN[ii].tangentDir;
		o.bitangentDir = IN[ii].bitangentDir;
		o.posWorld = mul(unity_ObjectToWorld, IN[ii].vertex);
		o.is_outline = false;

		// Pass-through the shadow coordinates if this pass has shadows.
		#if defined (SHADOWS_SCREEN) || ( defined (SHADOWS_DEPTH) && defined (SPOT) ) || defined (SHADOWS_CUBE)
		o._ShadowCoord = IN[ii]._ShadowCoord;
		#endif

		// Pass-through the fog coordinates if this pass has shadows.
		#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		o.fogCoord = IN[ii].fogCoord;
		#endif

		tristream.Append(o);
	}

	// return to the start of the triangles
	tristream.RestartStrip();
}

// convert a normal direction to a grayscale color
float grayscaleSH9(float3 normalDirection)
{
	return dot(ShadeSH9(half4(normalDirection, 1.0)), grayscale_vector);
}

#endif