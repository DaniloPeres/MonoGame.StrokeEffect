using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame
{
    public static partial class StrokeEffect
    {
        public static Texture2D transparentPixel;

        public static Texture2D CreateStrokeSpriteFont(SpriteFont spriteFont, string text, Color textColor, Vector2 scale, int strokeSize, Color strokeColor, GraphicsDevice graphics, StrokeType strokeType = StrokeType.OutlineAndTexture)
        {
            using (var textTexture2D = DrawSpriteFontToTexture2D(spriteFont, text, textColor, scale, graphics))
            {
                return CreateStroke(textTexture2D, strokeSize, strokeColor, graphics, strokeType);
            }
        }

        private static Texture2D DrawSpriteFontToTexture2D(SpriteFont spriteFont, string text, Color textColor, Vector2 scale, GraphicsDevice graphics)
        {
            var textSize = spriteFont.MeasureString(text) * scale;
            if (textSize.X == 0 || textSize.Y == 0)
                return GetTransparentPixel(graphics);

            lock (graphics)
            {
                var target = new RenderTarget2D(graphics, (int)textSize.X, (int)textSize.Y);
                graphics.SetRenderTarget(target);// Now the spriteBatch will render to the RenderTarget2D
                graphics.Clear(Color.Transparent);
                var spriteBatch = GetSpriteBatch(graphics);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                spriteBatch.DrawString(spriteFont, text, Vector2.Zero, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 1);
                spriteBatch.End();
                graphics.SetRenderTarget(null);//This will set the spriteBatch to render to the screen again.
                return target;
            }
        }

        private static Texture2D GetTransparentPixel(GraphicsDevice graphics)
        {
            if (transparentPixel == null)
            {
                transparentPixel = new Texture2D(graphics, 1, 1);
                var color = new Color[1];
                color[0] = Color.Transparent;
                transparentPixel.SetData<Color>(color);
            }

            return transparentPixel;
        }
    }
}
