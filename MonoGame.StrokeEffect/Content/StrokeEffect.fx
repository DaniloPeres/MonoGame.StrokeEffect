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

struct PixelCheckerInfo
{
	float2 Position;
	float4 Color;
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

PixelCheckerInfo getHighestAlphaPixel(PixelCheckerInfo highestPixelInfoAlpha, float2 pos, float2 originalPos)
{
	float4 color = tex2D(InputSampler, pos);

	if (highestPixelInfoAlpha.Color.a > color.a)
		return highestPixelInfoAlpha;
	
	// in case the same alpha, get the closest pixel
	if (highestPixelInfoAlpha.Color.a == color.a && abs(GetDistanceBetween2Points(highestPixelInfoAlpha.Position, originalPos)) <= abs(GetDistanceBetween2Points(pos, originalPos)))
		return highestPixelInfoAlpha;
	
	PixelCheckerInfo newPixelInfo;
	newPixelInfo.Color = color;
	newPixelInfo.Position = pos;
	return newPixelInfo;
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

	float radius = outlineWidth + alphaCenterPoint;
	float pointsCenter = CalculateDistancePixelFromCenter(positionOrigin, positionDestination, radius);
	float alpha = pointsCenter / maxPointsCenter;

	if (alpha <= 1)
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

	PixelCheckerInfo highestPixelInfoAlpha;
	highestPixelInfoAlpha.Color = float4(0, 0, 0, 0);
	highestPixelInfoAlpha.Position = float2(0, 0);

	int MAX_OUTLINE_WIDTH = 7;

    // Check first horizontal and vertical pixels
    [unroll(MAX_OUTLINE_WIDTH)]
	for (int i = 1; i <= outlineWidth; i++)
	{
		highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(i, 0) * uvPix, pos);
		highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(-i, 0) * uvPix, pos);
		highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(0, i) * uvPix, pos);
		highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(0, -i) * uvPix, pos);
	}

	[unroll(MAX_OUTLINE_WIDTH)]
	for (int x = 1; x <= outlineWidth; x++)
	{
		[unroll(MAX_OUTLINE_WIDTH)]
		for (int y = 1; y <= outlineWidth; y++)
		{
			highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(x, y) * uvPix, pos);
			highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(-x, y) * uvPix, pos);
			highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(x, -y) * uvPix, pos);
			highestPixelInfoAlpha = getHighestAlphaPixel(highestPixelInfoAlpha, pos + float2(-x, -y) * uvPix, pos);
		}
	}
  
	if (highestPixelInfoAlpha.Color.a == 0)
		return col;

    // Calculate the Alpha by the distance between the stroke pixel to the highestAlphaPixel
    // float maxPointsCenter = 0.001f; //CalculateDistancePixelFromCenter(pos, pos, (outlineWidth + 1) * uvPix.x);
	float alpha = CalculatePixelAlpha(pos / uvPix, highestPixelInfoAlpha.Position / uvPix, highestPixelInfoAlpha.Color.a);

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