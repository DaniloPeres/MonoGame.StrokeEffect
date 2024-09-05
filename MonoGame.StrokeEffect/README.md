# MonoGame.StrokeEffect
MonoGame.StrokeEffect is a library to generate strokes for Texture2D in MonoGame. We also support Sprite Font.
We use a shader effect to generate the Stroke effect.

# How to use

## Stroke Types:

- 'OutlineAndTexture' - Create a new Texture with the original Texture and an outline stroke
- 'OutlineWithoutTexture' - Create a new Texture with only the outline stroke

## Create a new Texture with stroke

```csharp
int strokeSize = 3;
Color strokeColor = Color.Black;
StrokeType strokeType = StrokeType.OutlineAndTexture;
var textureWithStroke = StrokeEffect.CreateStroke(myTexture, strokeSize, strokeColor, GraphicsDevice, strokeType);
```

## 

```csharp
Color textColor = Color.White;
Vector2 scale = Vector2.One;
int strokeSize = 3;
Color strokeColor = Color.Black;
StrokeType strokeType = StrokeType.OutlineAndTexture;
var textStroke = StrokeEffect.CreateStrokeSpriteFont(arialSpriteFont, "My Text", textColor, scale, strokeSize, strokeColor, GraphicsDevice, strokeType);
```

## License

MIT