#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_COMPLEX
#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"

// The height difference (in meters) between a black
// and a white height map pixel
float height = 1609;

// DPI
float dpi = 96;

D2D_PS_ENTRY(main)
{

	float2 uv = D2DGetScenePosition().xy * 96 / dpi;
	
	float heightCenter = D2DGetInput(0).r;
	float heightN = height * D2DSampleInputAtOffset(0, float2(0, -1)).r;
	float heightS = height * D2DSampleInputAtOffset(0, float2(0, +1)).r;
	float heightW = height * D2DSampleInputAtOffset(0, float2(-1, 0)).r;
	float heightE = height * D2DSampleInputAtOffset(0, float2(+1, 0)).r;

	// Sample neighbor pixels
	float3 n = float3(uv.x, heightN, uv.y - 1);
	float3 s = float3(uv.x, heightS, uv.y + 1);
	float3 w = float3(uv.x - 1, heightW, uv.y);
	float3 e = float3(uv.x + 1, heightE, uv.y);

	// Calculate normal vector (x, y, z in [-1..1])
	float3 normal = normalize((cross(n, e) + cross(e, s) + cross(s, w) + cross(w, n)) / -4);

	// Return normal vector with x, y, z in range [0..1]
	return float4(normal * .5 + .5, heightCenter);
}

