using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.StrokeEffect_Samples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public List<Drawables.IDrawable> drawList = new List<Drawables.IDrawable>();

        SpriteFont
            arialSpriteFont,
            cooperBlackSpriteFont;

        Drawables.Image
            imgObjCamera,
            imgObjGlobe,
            imgObjSurge,
            imgObjMultipleText,
            imgObjHeart,
            imgObjImagePlus,
            imgObjTextType,
            imgObjTextOutlineWithTexture,
            imgObjTextOutlineWithTextureCheckbox,
            imgObjTextOutlineWithoutTexture,
            imgObjTextOutlineWithoutTextureCheckbox,
            imgObjTextSize,
            imgObjTextSizeNumber,
            imgObjTextColor,
            imgObjTextSprintFontStroke,
            imgObjTextSmall,
            imgObjTextBigText,
            imgObjColorSelected,
            imgObjFpsText;

       Texture2D
            imgPixel,
            imgCheckBox,
            imgCheckBoxChecked,
            imgCameraOriginal,
            imgGlobeOriginal,
            imgSurgeOriginal,
            imgHeartOriginal,
            imgImagePlusOriginal;

        Color strokeColor = Color.White;

        StrokeType strokeType = StrokeType.OutlineAndTexture;

        int strokeSize = 3;

        int lastFpsCount = 0;
        int fpsCount = 0;
        TimeSpan fpsTimeSpan = new TimeSpan();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);


            imgPixel = new Texture2D(GraphicsDevice, 1, 1);
            Color[] _Color = new Color[1];
            _Color[0] = Color.White;
            imgPixel.SetData(_Color);

            arialSpriteFont = Content.Load<SpriteFont>("Arial");
            cooperBlackSpriteFont = Content.Load<SpriteFont>("CooperBlack");

            imgCheckBox = Content.Load<Texture2D>("CheckBox");
            imgCheckBoxChecked = Content.Load<Texture2D>("CheckBox_Checked");

            imgCameraOriginal = Content.Load<Texture2D>("camera");
            imgGlobeOriginal = Content.Load<Texture2D>("globe");
            imgHeartOriginal = Content.Load<Texture2D>("heart");
            imgImagePlusOriginal = Content.Load<Texture2D>("picture-plus");
            imgSurgeOriginal = Content.Load<Texture2D>("surge");

            var pos = new Vector2(50, 50);

            imgObjTextType = CreateAndAddImageObject(new Drawables.Image(imgPixel, pos));
            pos.X += 100;
            imgObjTextOutlineWithTextureCheckbox = CreateAndAddImageObject(new Drawables.Image(imgPixel, pos));
            imgObjTextOutlineWithTextureCheckbox.OnClick = () =>
            {
                strokeType = StrokeType.OutlineAndTexture;
                UpdateTextureOutlines();
            };
            imgObjTextOutlineWithTexture = CreateAndAddImageObject(new Drawables.Image(imgPixel, pos + new Vector2(38, 0)));

            pos.Y += 40;
            imgObjTextOutlineWithoutTextureCheckbox = CreateAndAddImageObject(new Drawables.Image(imgPixel, pos));
            imgObjTextOutlineWithoutTextureCheckbox.OnClick = () =>
            {
                strokeType = StrokeType.OutlineWithoutTexture;
                UpdateTextureOutlines();
            };
            imgObjTextOutlineWithoutTexture = CreateAndAddImageObject(new Drawables.Image(imgPixel, pos + new Vector2(38, 0)));

            pos = new Vector2(600, 15);

            CreateAndAddImageObject(new Drawables.Image(imgPixel, new Rectangle((int)pos.X - 30, (int)pos.Y, 2, 180)))
                .color = Color.Yellow;

            imgObjTextSize = new Drawables.Image(imgPixel, pos);
            drawList.Add(imgObjTextSize);
            pos.X += 80;
            var imgBtnMinus = Content.Load<Texture2D>("btnMinus");
            CreateAndAddImageObject(new Drawables.Image(imgBtnMinus, pos))
                .OnClick = () =>
                {
                    strokeSize = Math.Max(strokeSize - 1, 0);
                    UpdateTextureOutlines();
                };
            pos.X += 45;
            imgObjTextSizeNumber = new Drawables.Image(imgPixel, pos);
            drawList.Add(imgObjTextSizeNumber);
            pos.X += 40;
            var imgBtnPlus = Content.Load<Texture2D>("btnPlus");
            CreateAndAddImageObject(new Drawables.Image(imgBtnPlus, pos))
                .OnClick = () =>
                {
                    strokeSize++;
                    UpdateTextureOutlines();
                };

            pos = new Vector2(600, 80);
            imgObjTextColor = new Drawables.Image(imgPixel, pos);
            drawList.Add(imgObjTextColor);

            var posLeftColor = pos.X + 100;
            pos.X = posLeftColor;
            CreateSquareColor(pos, Color.White, true);
            pos.X += 45;
            CreateSquareColor(pos, Color.Yellow);
            pos.X += 45;
            CreateSquareColor(pos, Color.Black);
            pos.X += 45;
            CreateSquareColor(pos, Color.Red);
            pos.X += 45;
            CreateSquareColor(pos, Color.Blue);
            pos.X += 45;
            CreateSquareColor(pos, Color.Green);
            pos.X += 45;
            CreateSquareColor(pos, Color.Pink);
            pos.X = posLeftColor;
            pos.Y += 45;
            CreateSquareColor(pos, Color.LightGray);
            pos.X += 45;
            CreateSquareColor(pos, Color.Gray);
            pos.X += 45;
            CreateSquareColor(pos, Color.DarkGray);
            pos.X += 45;
            CreateSquareColor(pos, Color.Olive);
            pos.X += 45;
            CreateSquareColor(pos, Color.Cyan);
            pos.X += 45;
            CreateSquareColor(pos, Color.Orange);
            pos.X += 45;
            CreateSquareColor(pos, Color.Purple);

            CreateAndAddImageObject(new Drawables.Image(imgPixel, new Rectangle(15, 195, 1000, 2)))
                .color = Color.Yellow;

            imgObjTextSprintFontStroke = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(25, 210)));
            imgObjTextSmall = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(25, 260)));
            imgObjTextBigText = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(25, 280)));

            CreateAndAddImageObject(new Drawables.Image(imgPixel, new Rectangle(15, 195, 1000, 2)))
                .color = Color.Yellow;

            CreateAndAddImageObject(new Drawables.Image(imgSurgeOriginal, new Vector2(850, 250)));
            imgObjSurge = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(900, 250)));

            var posY = 370;

            CreateAndAddImageObject(new Drawables.Image(imgHeartOriginal, new Vector2(50, posY)));
            imgObjHeart = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(100, posY)));

            CreateAndAddImageObject(new Drawables.Image(imgCameraOriginal, new Vector2(200, posY)));
            imgObjCamera = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(275, posY)));

            CreateAndAddImageObject(new Drawables.Image(imgImagePlusOriginal, new Vector2(50, posY + 75)));
            imgObjImagePlus = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(200, posY + 75)));

            CreateAndAddImageObject(new Drawables.Image(imgGlobeOriginal, new Vector2(440, posY)));
            imgObjGlobe = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(700, posY)));

            CreateAndAddImageObject(new Drawables.Image(imgPixel, new Rectangle(15, 630, 1000, 2)))
                .color = Color.Yellow;

            imgObjMultipleText = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(25, 640)));

            imgObjFpsText = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Vector2(600, 660)));

            UpdateTextureOutlines();
        }

        private void UpdateTextureOutlines()
        {
            imgObjTextOutlineWithTextureCheckbox.texture = strokeType == StrokeType.OutlineAndTexture ? imgCheckBoxChecked : imgCheckBox;
            imgObjTextOutlineWithoutTextureCheckbox.texture = strokeType == StrokeType.OutlineWithoutTexture ? imgCheckBoxChecked : imgCheckBox;

            CreateStrokeSpriteFont(imgObjTextType, arialSpriteFont, "Type:");
            CreateStrokeSpriteFont(imgObjTextOutlineWithTexture, arialSpriteFont, "Outline");
            CreateStrokeSpriteFont(imgObjTextOutlineWithoutTexture, arialSpriteFont, "Outline no texture");

            CreateStrokeSpriteFont(imgObjTextSize, arialSpriteFont, "Size:");
            CreateStrokeSpriteFont(imgObjTextSizeNumber, arialSpriteFont, strokeSize.ToString());

            CreateStrokeSpriteFont(imgObjTextColor, arialSpriteFont, "Color:");


            CreateStrokeSpriteFont(imgObjTextSprintFontStroke, arialSpriteFont, "SPRITE FONT STROKE", new Vector2(1.5f));
            CreateStrokeSpriteFont(imgObjTextSmall, arialSpriteFont, "SPRITE FONT STROKE - SMALL TEXT", new Vector2(0.8f));
            CreateStrokeSpriteFont(imgObjTextBigText, cooperBlackSpriteFont, "Sprite font stroke - BIG TEXT", new Vector2(1.3f));

            CreateStroke(imgObjCamera, imgCameraOriginal);
            CreateStroke(imgObjGlobe, imgGlobeOriginal);
            CreateStroke(imgObjSurge, imgSurgeOriginal);
            CreateStroke(imgObjHeart, imgHeartOriginal);
            CreateStroke(imgObjImagePlus, imgImagePlusOriginal);

            CreateStrokeSpriteFont(imgObjMultipleText, cooperBlackSpriteFont, "Multiple Borders", new Vector2(1.5f), Color.White);
            CreateStroke(imgObjMultipleText, imgObjMultipleText.texture, Color.Orange);
            CreateStroke(imgObjMultipleText, imgObjMultipleText.texture, Color.Red);
            CreateStroke(imgObjMultipleText, imgObjMultipleText.texture, Color.Yellow);
            CreateStroke(imgObjMultipleText, imgObjMultipleText.texture, Color.Black);

            UpdateFpsTexture();
        }

        private void CreateStroke(Drawables.Image image, Texture2D texture, Color? imageStrokeColor = null)
        {
            var imageStroke = StrokeEffect.CreateStroke(texture, strokeSize, imageStrokeColor == null ? strokeColor : imageStrokeColor.Value, GraphicsDevice, Content, strokeType);

            // Dispose if the previous texture is not pixel
            if (image.texture != imgPixel)
                image.texture.Dispose();

            image.texture = imageStroke;
        }

        private void CreateStrokeSpriteFont(Drawables.Image image, SpriteFont spriteFont, string text, Vector2? scale = null, Color? textStrokeColor = null, Color? textColor = null)
        {
            var textStroke = StrokeEffect.CreateStrokeSpriteFont(spriteFont, text, textColor == null ? Color.Black : textColor.Value, scale ?? Vector2.One, strokeSize, textStrokeColor == null ? strokeColor : textStrokeColor.Value, GraphicsDevice, Content, strokeType);

            // Dispose if the previous texture is not pixel
            if (image.texture != imgPixel)
                image.texture.Dispose();

            image.texture = textStroke;
        }

        private void UpdateFpsTexture()
        {
            CreateStrokeSpriteFont(imgObjFpsText, cooperBlackSpriteFont, $"FPS: {lastFpsCount}", new Vector2(0.85f), null, Color.Blue);
        }

        private bool clicked = false;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            fpsCount++;
            fpsTimeSpan = fpsTimeSpan.Add(gameTime.ElapsedGameTime);
            if (fpsTimeSpan >= new TimeSpan(0, 0, 1))
            {
                lastFpsCount = fpsCount;
                fpsTimeSpan = new TimeSpan(0, 0, 0, 0, fpsTimeSpan.Milliseconds);
                fpsCount = 0;
                UpdateFpsTexture();
            }

            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (!clicked)
                {
                    clicked = true;
                    var pos = new Vector2(mouse.X, mouse.Y);

                    var imgObj = drawList.FirstOrDefault(item => item.Collision(pos));
                    imgObj?.OnClick?.Invoke();
                }
            }
            else
                clicked = false;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            drawList.ForEach(drawItem => drawItem.Draw(spriteBatch));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CreateSquareColor(Vector2 pos, Color color, bool isSelected = false)
        {
            var squareSize = 32;

            var rec = new Rectangle(pos.ToPoint(), new Point(squareSize));

            if (isSelected)
                SelectedColor(rec);

            var imgObj = CreateAndAddImageObject(new Drawables.Image(imgPixel, rec));
            imgObj.color = color;
            imgObj.OnClick = () =>
            {
                strokeColor = imgObj.color;
                SelectedColor(rec);
                UpdateTextureOutlines();
            };
        }

        private void SelectedColor(Rectangle rec)
        {
            var squareBorder = 4;

            if (imgObjColorSelected == null)
            {
                imgObjColorSelected = CreateAndAddImageObject(new Drawables.Image(imgPixel, new Rectangle(Point.Zero, new Point(rec.Width + (squareBorder * 2)))));
                imgObjColorSelected.color = Color.GreenYellow;
            }

            imgObjColorSelected.destinationRectangle = new Rectangle(rec.Location - new Point(squareBorder), imgObjColorSelected.destinationRectangle.Size);
        }

        private Drawables.Image CreateAndAddImageObject(Drawables.Image imgObj)
        {
            drawList.Add(imgObj);
            return imgObj;
        }

        private Texture2D GetCheckBox(bool isChecked)
        {
            return isChecked ? imgCheckBoxChecked : imgCheckBox;
        }
    }
}
