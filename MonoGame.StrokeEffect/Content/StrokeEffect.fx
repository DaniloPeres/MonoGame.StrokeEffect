#if SM4
	#define PS_PROFILE ps_4_0
	#define VS_PROFILE vs_4_0
#else
	#define PS_PROFILE ps_3_0
	#define VS_PROFILE vs_3_0
#endif

int outlineWidth = 2;
float4 outlineColor = float4(1.0f, 0, 0, 1.0f);
float2 textureSize : VPOS;
// TypeID:
//		0 -> Outline
//		1 -> Outline no texture
int typeId = 0;

Texture2D SpriteTexture;
sampler2D InputSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float OverlapColorParameter(float baseColor, float addColor, float addAlpha)
{
	float baseAlphaPercetage = 1 - addAlpha;
	float mixedColor = (baseColor * baseAlphaPercetage) + (addColor * addAlpha);
	return min(mixedColor, 1);
}

float4 OverlapColor(float4 baseColor, float4 addColor)
{
    // Add color has no alpha, nothing to mix
	if (addColor.a == 1)
		return addColor;

    // In case the original image is fully transparent, use the add color
	if (addColor.a == 0)
		return baseColor;

	float red = OverlapColorParameter(baseColor.r, addColor.r, addColor.a);
	float green = OverlapColorParameter(baseColor.g, addColor.g, addColor.a);
	float blue = OverlapColorParameter(baseColor.b, addColor.b, addColor.a);

    // if the base color has alpha (not a transparent pixel), so the final alpha must be totaly colored;
	float alpha = baseColor.a != 0 ? 1 : addColor.a;

	return float4(red, green, blue, alpha);
}

float GetDistanceBetween2Points(float2 pos1, float2 pos2)
{
    // get dist square to circle
	float distX = pos1.x - pos2.x;
	float distY = pos1.y - pos2.y;
	return pow(distX, 2) + pow(distY, 2);
}

bool isPos1Closer(float2 pos1, float2 pos2, float2 posCompare)
{
	float distPos1 = abs(posCompare.x - pos1.x) + abs(posCompare.y - pos1.y);
	float distPos2 = abs(posCompare.x - pos2.x) + abs(posCompare.y - pos2.y);
	return distPos1 <= distPos2;
}

float getHighestAlphaPixel(float highestPixelAlpha, float2 pos, float2 originalPos)
{
	float4 color = tex2D(InputSampler, pos);

	return highestPixelAlpha > color.a
		? highestPixelAlpha
		: color.a;
}

float2 getClosestPixelWithColor(float2 closestPixel, float2 pos, float2 originalPos)
{
	float4 color = tex2D(InputSampler, pos);
	if (color.a == 0)
		return closestPixel;
	if (closestPixel.x == -1)
		return pos;
	
	float2 uvPix = float2(1, 1) / textureSize;

	return isPos1Closer(closestPixel / uvPix, pos / uvPix, originalPos / uvPix) ? closestPixel : pos;
}

float CalculateDistancePixelFromCenter(float2 positionOrigin, float2 positionDestination, float radius)
{
    // Use a circule reference to know how for is it
	float dist = GetDistanceBetween2Points(positionOrigin, positionDestination);
	return dist - pow(radius, 2);
}

float CalculatePixelAlpha(float2 positionOrigin, float2 positionDestination, float alphaCenterPoint)
{
    // Get the fartest pixel possible to be the max points to calculate the other points in circular
	float maxPointsCenter = CalculateDistancePixelFromCenter(positionOrigin, positionOrigin + float2(outlineWidth, 0), outlineWidth + 1);

	float radius = outlineWidth + 1;
	float pointsCenter = CalculateDistancePixelFromCenter(positionOrigin, positionDestination, radius);
	float alpha = pointsCenter / maxPointsCenter;

	if (alpha <= 1.0001)
	 	alpha *= alphaCenterPoint;

	return max(min(1, alpha), 0);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 pos = float2(input.TextureCoordinates.x, input.TextureCoordinates.y);
	float2 uvPix = float2(1, 1) / textureSize;
	float4 col = tex2D(InputSampler, pos);

	if (col.a == 1) {
		// typeId 1 - Only outline without texture
		if (typeId == 1)
			return float4(0,0,0,0);

		return col;
	}

	float highestPixelAlpha = 0;
	float2 closestPixel = float2(-1, -1);

    // Check first horizontal and vertical pixels
#if OPENGL
	const static int MAX_LOOP = 7;
#else
	int MAX_LOOP = outlineWidth;
#endif
    [loop]
	for (int i = 1; i <= MAX_LOOP; i++)
	{
		if (i <= outlineWidth) {
			highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(i, 0) * uvPix, pos);
			highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(-i, 0) * uvPix, pos);
			highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(0, i) * uvPix, pos);
			highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(0, -i) * uvPix, pos);
			closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(i, 0) * uvPix, pos);
			closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(-i, 0) * uvPix, pos);
			closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(0, i) * uvPix, pos);
			closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(0, -i) * uvPix, pos);
		}
	}

    [loop]
	for (int x = 1; x <= MAX_LOOP; x++)
	{ 
		if (x <= outlineWidth) {
			[loop]
			for (int y = 1; y <= MAX_LOOP; y++)
			{
				if (y <= outlineWidth) {
					highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(x, y) * uvPix, pos);
					highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(-x, y) * uvPix, pos);
					highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(x, -y) * uvPix, pos);
					highestPixelAlpha = getHighestAlphaPixel(highestPixelAlpha, pos + float2(-x, -y) * uvPix, pos);
					closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(x, y) * uvPix, pos);
					closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(-x, y) * uvPix, pos);
					closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(x, -y) * uvPix, pos);
					closestPixel = getClosestPixelWithColor(closestPixel, pos + float2(-x, -y) * uvPix, pos);
				}
			}
		}
	}
  
    if (highestPixelAlpha == 0 || closestPixel.x == -1)
		return col;

    // Calculate the Alpha by the distance between the stroke pixel to the closest pixel (no transparency) using the highest transparency found
	float alpha = CalculatePixelAlpha(pos / uvPix, closestPixel / uvPix, highestPixelAlpha);

	float4 outlineCalc = outlineColor * alpha;

	if (col.a == 0 || typeId == 1)
		return outlineCalc;
    
	// The pixel is not transparent, merge the pixel with stroke color
	return OverlapColor(outlineCalc, col);
}


technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_PROFILE MainPS();
	}
};