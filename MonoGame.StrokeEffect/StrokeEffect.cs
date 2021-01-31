using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame
{
    public static partial class StrokeEffect
    {
        private static readonly List<Point> pixels4Neighbors = new[]
            {
                new Point(-1, 0), // Left
                new Point(0, -1), // Top
                new Point(0, 1), // Bottom
                new Point(1, 0), // Right
            }.ToList();

        public static Texture2D CreateStroke(Texture2D src, int size, Color color, GraphicsDevice graphics, StrokeType strokeType = StrokeType.OutlineAndTexture)
        {
            int count = src.Width * src.Height;
            Color[] dataSrc = new Color[count];
            src.GetData(dataSrc);

            var dataBorder = GetBorderData(dataSrc, src.Width, src.Height, size, color, strokeType);

            // Draw the source texture if is the case
            if (strokeType == StrokeType.OutlineAndTexture)
            {
                var marginBorder = new Point(size);
                for (var i = 0; i < dataSrc.Length; i++)
                {
                    var posSrc = ConvertPositionIndexToPositionPoint(src.Width, i);
                    var posWithBorder = posSrc + marginBorder;
                    var indexWithBorder = ConvertPositionPointToPositionIndex(src.Width + (size * 2), posWithBorder);
                    dataBorder[indexWithBorder] = OverlapColor(dataBorder[indexWithBorder], dataSrc[i]);
                }
            }

            // Convert data colors to texture2d
            Texture2D textureWithBorder = new Texture2D(graphics, src.Width + (size * 2), src.Height + (size * 2));
            textureWithBorder.SetData(dataBorder);

            return textureWithBorder;
        }

        private static Color OverlapColor(Color baseColor, Color addColor)
        {
            // Add color has no alpha, nothing to mix
            if (addColor.A == 255)
                return addColor;

            // In case the original image is fully transparent, use the add color
            if (addColor.A == 0)
                return baseColor;

            var alphaPercetage = addColor.A / 255f;
            var red = OverlapColor(baseColor.R, addColor.R, alphaPercetage);
            var green = OverlapColor(baseColor.G, addColor.G, alphaPercetage);
            var blue = OverlapColor(baseColor.B, addColor.B, alphaPercetage);

            // if the base color has alpha (not a transparent pixel), so the final alpha must be totaly colored;
            var alpha = baseColor.A != 0 ? 255f : addColor.A;

            return new Color(new Color(red, green, blue), alpha / 255f);
        }

        private static byte OverlapColor(byte baseColor, byte addColor, float addAlphaPercetage)
        {
            var baseAlphaPercetage = 1 - addAlphaPercetage;
            var mixedColor = (baseColor * baseAlphaPercetage) + (addColor * addAlphaPercetage);
            return (byte)Math.Min(mixedColor, 255);
        }

        private static Color[] GetBorderData(Color[] data, int width, int height, int size, Color color, StrokeType strokeType)
        {
            var textureBorderPixels = GetBorderPixels(data, width, height);
            var pixelsBorderWithSize = GetPixelsBorderWithSize(data, textureBorderPixels, width, size);

            // Filter if we should print outline or inline
            var pixelsPrint = pixelsBorderWithSize.Where(pixelPosition =>
            {
                switch (strokeType)
                {
                    case StrokeType.OutlineWithoutTexture:
                        // Draw the pixel only if the pixel does not intersect the texture
                        return PixelIntersectsInImage(data, width, height, pixelPosition.position);
                    case StrokeType.InlineWithoutTexture:
                        // Draw the pixel only if the pixel intersects the texture
                        return !PixelIntersectsInImage(data, width, height, pixelPosition.position);
                    default:
                        return true;
                }
            }).ToList();

            var borderWidth = width + (size * 2);
            var borderHeight = height + (size * 2);
            var count = borderWidth * borderHeight;
            var dataBorder = new Color[count];

            List<(int positionIndex, float alphaPercetage)> pixelsPrintWithSizePosition = pixelsPrint.Select(pixelPosition =>
            {
                var positionPixelBorder = pixelPosition.position + new Point(size);
                return (ConvertPositionPointToPositionIndex(borderWidth, positionPixelBorder), pixelPosition.alphaPercetage);
            }).ToList();

            pixelsPrintWithSizePosition.ForEach(indexWithAlpha =>
            {
                dataBorder[indexWithAlpha.positionIndex] = color * indexWithAlpha.alphaPercetage;
            });

            return dataBorder;
        }

        /// <summary>
        /// Check if the given pixel is located in not completely transparent pixel
        /// When the pixel is out of the texture, it considers that is does not intersect
        /// </summary>
        private static bool PixelIntersectsInImage(Color[] data, int width, int height, Point position)
        {
            if (!IsPositionInsideImage(width, height, position))
                return true;

            var index = ConvertPositionPointToPositionIndex(width, position);

            // Is completely transparent?
            return data[index].A == 0;
        }

        private static List<(Point position, float alphaPercetage)> GetPixelsBorderWithSize(Color[] dataSrc, List<int> textureBorderPixels, int width, int size)
        {
            // Points is used to know the alpha of the pixel, less points, less alpha
            var pixelsToPrint = new Dictionary<Point, float>();


            var pixelsTest = textureBorderPixels.Select(x => ConvertPositionIndexToPositionPoint(width, x)).ToList();

            textureBorderPixels.ForEach(borderPixelIndex =>
            {
                var borderPixelPoint = ConvertPositionIndexToPositionPoint(width, borderPixelIndex);

                // Take the first left middle pixel alpha to set as maximum
                // and we are going to compare all pixels, if the pixel is positive, it is out of the circle, if is negative, calculate the alpha dividing by the max points
                var pixelBaseAlpha = borderPixelPoint + new Point(-size, 0);
                var maxPointsCenter = (float)CalculateDistancePixelFromCenter(borderPixelPoint, pixelBaseAlpha, size + 1);
                var alphaCenterPoint = dataSrc[borderPixelIndex].A / 255f;
                for (var x = -size; x <= size; x++)
                {
                    for (var y = -size; y <= size; y++)
                    {
                        var borderWithSizePixel = borderPixelPoint + new Point(x, y);
                        var alpha = CalculatePixelAlpha(borderPixelPoint, borderWithSizePixel, size, maxPointsCenter, alphaCenterPoint);
                        // in case the pixels was already processed, take the strongest alpha
                        if (pixelsToPrint.ContainsKey(borderWithSizePixel))
                            pixelsToPrint[borderWithSizePixel] = Math.Max(alpha, pixelsToPrint[borderWithSizePixel]);
                        else
                            pixelsToPrint.Add(borderWithSizePixel, alpha);
                    }
                }
            });

            return pixelsToPrint
                .Select(x => (x.Key, Math.Max(0, Math.Min(1, x.Value))))
                .ToList();
        }

        private static float CalculatePixelAlpha(Point positionOrigin, Point positionDestination, int size, float maxPointsCenter, float alphaCenterPoint)
        {
            var radius = size + (1 * alphaCenterPoint);
            var pointsCenter = CalculateDistancePixelFromCenter(positionOrigin, positionDestination, radius);
            var alpha = pointsCenter / maxPointsCenter;

            // if the pixel is too close, stay the 100% of alpha
            if (alpha <= 1)
                alpha *= alphaCenterPoint;

            return alpha;
        }

        private static float CalculateDistancePixelFromCenter(Point positionOrigin, Point positionDestination, float radius)
        {
            // Use a circule reference to know how for is it

            // get dist square to circle
            var dist = Math.Pow(positionDestination.X - positionOrigin.X, 2) + Math.Pow(positionDestination.Y - positionOrigin.Y, 2);

            return (float)(dist - radius * radius);
        }

        private static List<int> GetBorderPixels(Color[] data, int width, int height)
        {
            var bordersPixels = new List<int>();
            for (var i = 0; i < data.Length; i++)
            {
                // If pixel is not completely transparent.
                if (data[i].A != 0 && HasTransparentNeighborPixel(data, width, height, i))
                    bordersPixels.Add(i);
            }

            return bordersPixels;
        }

        private static bool HasTransparentNeighborPixel(Color[] data, int width, int height, int index)
        {
            // check if the pixel has a neighbor transparent (8 pixels, vertical, horizontal and diagonal)
            // also if the pixel is in the corder, we consider it is a border
            var pixelPos = ConvertPositionIndexToPositionPoint(width, index);

            return pixels4Neighbors.Any(neighborPoint =>
            {
                var pixelCheck = pixelPos + neighborPoint;
                if (!IsPositionInsideImage(width, height, pixelCheck))
                    return true;

                var pixelCheckIndex = ConvertPositionPointToPositionIndex(width, pixelCheck);

                // Is completely transparent?
                return data[pixelCheckIndex].A == 0;
            });

        }

        private static bool IsPositionInsideImage(int width, int height, Point position)
        {
            return position.X >= 0
                && position.Y >= 0
                && position.X < width
                && position.Y < height;
        }

        private static Point ConvertPositionIndexToPositionPoint(int width, int index)
        {
            return new Point(index % width, (int)Math.Floor(index / (double)width));
        }

        private static int ConvertPositionPointToPositionIndex(int width, Point position)
        {
            return position.X + (position.Y * width);
        }
    }
}
