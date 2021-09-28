using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;

namespace MonoGame
{
    public enum GraphicsApi
    {
        OPEN_GL,
        DIRECT_X
    }

    public static partial class StrokeEffect
    {
        private static Effect strokeEffectCache;
        private static string CurrentShaderExtension => GetShaderExtension() == GraphicsApi.OPEN_GL ? "OpenGL" : "DirectX";
        private static SpriteBatch spriteBatchMemory;

        public static Texture2D CreateStroke(Texture2D src, int size, Color color, GraphicsDevice graphics, StrokeType strokeType = StrokeType.OutlineAndTexture)
        {
#if ANDROID
            if (!MainThread.IsMainThread)
                throw new Exception("To create a stroke effect, it must be running in the main thread");
#endif

            lock (graphics)
            {
                var effect = GetEffectStroke(graphics);

                // create a render target with margins
                using (var renderTargetResize = new RenderTarget2D(graphics, src.Width + size * 2, src.Height + size * 2))
                {
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
        }

        private static SpriteBatch GetSpriteBatch(GraphicsDevice graphics)
        {
            if (spriteBatchMemory == null)
                spriteBatchMemory = new SpriteBatch(graphics);

            return spriteBatchMemory;
        }

        private static Effect GetEffectStroke(GraphicsDevice graphics)
        {
            if (strokeEffectCache == null)
            {

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"MonoGame.StrokeEffect.Content.{CurrentShaderExtension}.StrokeEffect.xnb";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        var content = new XnbContentManager(ms, graphics);
                        strokeEffectCache = content.Load<Effect>("MyMemoryStreamAsset");
                    }
                }
            }

            return strokeEffectCache;
        }

        private static GraphicsApi GetShaderExtension()
        {
            // Use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1).
            // May need to be changed / fixed for future shader profiles.
            const string SHADER_TYPE = "Microsoft.Xna.Framework.Graphics.Shader";
            const string PROFILE = "Profile";

            var assembly = typeof(Game).GetTypeInfo().Assembly;
            if (assembly == null)
                throw new Exception(
                    "Error determining shader profile. Couldn't find assembly. Falling back to OpenGL.");

            var shaderType = assembly.GetType(SHADER_TYPE);
            if (shaderType == null)
                throw new Exception(
                    $"Error determining shader profile. Couldn't find shader type of '{SHADER_TYPE}'. Falling back to OpenGL.");

            var shaderTypeInfo = shaderType.GetTypeInfo();
            if (shaderTypeInfo == null)
                throw new Exception(
                    "Error determining shader profile. Couldn't get TypeInfo of shadertype. Falling back to OpenGL.");

            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/Shader/Shader.cs#L47
            var profileProperty = shaderTypeInfo.GetDeclaredProperty(PROFILE);
            var value = (int)profileProperty.GetValue(null);

            switch (value)
            {
                case 0:
                    return GraphicsApi.OPEN_GL;
                case 1:
                    return GraphicsApi.DIRECT_X;
                default:
                    throw new Exception("Unknown shader profile.");
            }
        }
    }
}
