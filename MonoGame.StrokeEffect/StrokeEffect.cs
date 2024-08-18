using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame;

public static partial class StrokeEffect
{
    private static Effect strokeEffectCache;
    private static SpriteBatch spriteBatchMemory;

    public static Texture2D CreateStroke(Texture2D src, int size, Color color, GraphicsDevice graphics, ContentManager contentManager, StrokeType strokeType = StrokeType.OutlineAndTexture)
    {

        lock (graphics)
        {
            var effect = GetEffectStroke(contentManager);

            // create a render target with margins
            using var renderTargetResize = new RenderTarget2D(graphics, src.Width + size * 2, src.Height + size * 2);
            graphics.SetRenderTarget(renderTargetResize);
            graphics.Clear(Color.Transparent);

            var spriteBatch = GetSpriteBatch(graphics);
            // Create a new texture with the new size
            spriteBatch.Begin();

            // draw the texture with the margin
            spriteBatch.Draw(src, new Vector2(size), Color.White);

            spriteBatch.End();

            var renderTarget = new RenderTarget2D(graphics, renderTargetResize.Width, renderTargetResize.Height);

            // Apply my effect
            effect.Parameters["textureSize"].SetValue(new Vector2(renderTargetResize.Width, renderTargetResize.Height));
            effect.Parameters["outlineWidth"].SetValue(size);
            effect.Parameters["outlineColor"].SetValue(color.ToVector4());
            effect.Parameters["typeId"].SetValue((int)strokeType);

            // Draw the img with the effect
            graphics.SetRenderTarget(renderTarget);
            graphics.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, effect);
            spriteBatch.Draw(renderTargetResize, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);

            return renderTarget;
        }
    }

    private static SpriteBatch GetSpriteBatch(GraphicsDevice graphics)
    {
        if (spriteBatchMemory == null)
            spriteBatchMemory = new SpriteBatch(graphics);

        return spriteBatchMemory;
    }

    private static Effect GetEffectStroke(ContentManager contentManager)
    {
        strokeEffectCache ??= contentManager.Load<Effect>("StrokeEffect");
        return strokeEffectCache;
    }
}
