#define D2D_INPUT_COUNT 4
#define D2D_INPUT0_SIMPLE // Normal (RGB) and height (A) map
#define D2D_INPUT1_SIMPLE // Grass texture
#define D2D_INPUT2_SIMPLE // Snow texture
#define D2D_INPUT3_SIMPLE // Rock texture
#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"

// DPI
float dpi;

// Amount of ambient light
float ambientLight = .1;

// The real-world height in meters of a black pixel on the heightmap
float baseHeight = 719;

// The real-world height difference in meters between
// a black pixel and a white pixel on the height map.
float height = 1609;

// The sun light direction. "Shadows" are rendered according to this value.
float3 lightDirection = { 1, -2, 1 };

// The minimum steepness (between 0 and 1) required for rock to be rendered.
float rockMinSteepness = .45;

// The minimum height (in meters) for snow to be rendered.
float snowMinHeight = 1500;

// From the snow start height, this is the height difference
// in meters that is needed until we have 100% of snow 
static const float distanceFromSnowStartToFullSnow = 100;

static const float rockMultiplier = 30;


D2D_PS_ENTRY(main)
{
	float2 uv = D2DGetScenePosition().xy * 96 / dpi;
	
	float4 normalMap = D2DGetInput(0); // RGB = normal, A = normalized height
	float4 grassColor = D2DGetInput(1);
	float4 snowColor = D2DGetInput(2);
	float4 rockColor = D2DGetInput(3);
	
	// Determine amount of light by computing the angle
	// between the sunlight direction and the surface normal
	float3 normal = normalMap.rgb * 2 - 1;
	float lightToSurfaceAngle = dot(normalize(lightDirection), normal);
	float light = 1 - (lightToSurfaceAngle + 1) / 2;

	// Blend together grass and snow textures
	float actualHeight = baseHeight + normalMap.a * height; // Real-world height in meters

	float steepness = 1 - dot(normalMap.rgb, float3(0, 1, 0)); // 0 = flat, 1 = 90°
	float rockAmount = min(1, rockMultiplier * max(0, steepness - .45));
	float snowAmount = (1 - rockAmount) * clamp(actualHeight - snowMinHeight, 0, distanceFromSnowStartToFullSnow) / distanceFromSnowStartToFullSnow;
	float grassAmount = 1 - rockAmount - snowAmount;

	float4 color =
		rockAmount * rockColor +
		snowAmount * snowColor +
		grassAmount * grassColor;

	return lerp(ambientLight, 1, light) * color;
}

