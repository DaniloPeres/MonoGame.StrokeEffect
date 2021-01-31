using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.StrokeEffect_Samples.Drawables
{
    public class Image : IDrawable
    {
        public Texture2D texture, texture_MouseMove, texture_Initial;
        public Rectangle destinationRectangle = Rectangle.Empty;
        public Rectangle? sourceRectangle = null;
        public Color color = Color.White;
        public float rotation;
        public Vector2 origin;
        public SpriteEffects effects = SpriteEffects.None;
        public float layerDepth;
        public Vector2 position;
        public Vector2 scale = new Vector2(1);
        public Action OnClick { get; set; }

        public Image(Texture2D texture, Rectangle destinationRectangle, float layerDepth = 1)
        {
            this.texture = texture;
            this.texture_Initial = texture;
            this.destinationRectangle = destinationRectangle;
            this.layerDepth = layerDepth;
        }

        public Image(Texture2D texture, Vector2 position, float layerDepth = 1)
        {
            this.texture = texture;
            this.texture_Initial = texture;
            this.position = position;
            this.layerDepth = layerDepth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!destinationRectangle.IsEmpty)
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
            else
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public Boolean Collision(Vector2 pos)
        {
            if (texture == null) return false;

            var R1 = getImageRectangle();
            Rectangle R2 = new Rectangle(pos.ToPoint(), new Point(1));

            return R1.Intersects(R2);
        }


        private Rectangle getImageRectangle()
        {
            Rectangle R1;

            if (!destinationRectangle.IsEmpty)
            {
                R1 = destinationRectangle;

                //Verificar Origin e arrumar a posicao do objeto
                R1.X -= (int)(origin.X * (R1.Width / (float)texture.Width));
                R1.Y -= (int)(origin.Y * (R1.Height / (float)texture.Height));
            }
            else
            {
                Point size = new Point(texture.Width, texture.Height);

                if (sourceRectangle.HasValue)
                    size = sourceRectangle.Value.Size;

                R1 = new Rectangle(
                    position.ToPoint() - (origin * scale).ToPoint(),
                    (size.ToVector2() * scale).ToPoint()
                    );
            }

            return R1;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
