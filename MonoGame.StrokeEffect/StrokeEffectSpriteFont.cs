using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame;

public static partial class StrokeEffect
{
    private static Texture2D transparentPixel;

    public static Texture2D CreateStrokeSpriteFont(SpriteFont spriteFont, string text, Color textColor, Vector2 scale, int strokeSize, Color strokeColor, GraphicsDevice graphics, StrokeType strokeType = StrokeType.OutlineAndTexture)
    {
        // Step 1: Render unscaled text to texture
        using var unscaledTextTexture = DrawSpriteFontToTexture2D(spriteFont, text, textColor, graphics);

        // Step 2: Create stroke on unscaled texture
        using var strokedTexture = CreateStroke(unscaledTextTexture, strokeSize, strokeColor, graphics, strokeType);

        // Step 3: Scale stroked texture to final size
        if (scale != Vector2.One)
        {
            int scaledWidth = (int)(strokedTexture.Width * scale.X);
            int scaledHeight = (int)(strokedTexture.Height * scale.Y);
            return ScaleTexture(strokedTexture, scaledWidth, scaledHeight, graphics);
        }

        return strokedTexture;
    }

    private static Texture2D DrawSpriteFontToTexture2D(SpriteFont spriteFont, string text, Color textColor, GraphicsDevice graphics)
    {
        var textSize = spriteFont.MeasureString(text);
        if (textSize.X == 0 || textSize.Y == 0)
            return GetTransparentPixel(graphics);

        lock (graphics)
        {
            var target = new RenderTarget2D(graphics, (int)textSize.X, (int)textSize.Y);
            graphics.SetRenderTarget(target);
            graphics.Clear(Color.Transparent);

            var spriteBatch = GetSpriteBatch(graphics);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            spriteBatch.DrawString(spriteFont, text, Vector2.Zero, textColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
            spriteBatch.End();

            graphics.SetRenderTarget(null);
            return target;
        }
    }

    private static Texture2D ScaleTexture(Texture2D original, int width, int height, GraphicsDevice graphics)
    {
        var scaledTarget = new RenderTarget2D(graphics, width, height);
        graphics.SetRenderTarget(scaledTarget);
        graphics.Clear(Color.Transparent);

        var spriteBatch = GetSpriteBatch(graphics);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        spriteBatch.Draw(original, new Rectangle(0, 0, width, height), Color.White);
        spriteBatch.End();

        graphics.SetRenderTarget(null);
        return scaledTarget;
    }

    private static Texture2D GetTransparentPixel(GraphicsDevice graphics)
    {
        if (transparentPixel == null)
        {
            transparentPixel = new Texture2D(graphics, 1, 1);
            transparentPixel.SetData(new[] { Color.Transparent });
        }

        return transparentPixel;
    }
}
