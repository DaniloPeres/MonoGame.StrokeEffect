<p align="center">
  <img src="http://daniloperes.com/MonoGame.StrokeEffect_Logo_256.png" alt="MonoGame.StrokeEffect" width="120" height="120">
</p>

# MonoGame.StrokeEffect
<b>MonoGame.StrokeEffect</b> is a library to generate strokes for Texture2D in MonoGame. We also support Sprite Font.

## Nuget package
There is a nuget package avaliable here https://www.nuget.org/packages/MonoGame.StrokeEffect/.

# Examples

<p align="center">
  <img src="http://daniloperes.com/MonoGame.StrokeEffect.Sample.gif" alt="MonoGame.StrokeEffect" width="600" height="493">
</p>

## Stroke Types:

- 'OutlineAndTexture' - Create a new Texture with the original Texture and an outline stroke
- 'OutlineWithoutTexture' - Create a new Texture with only the outline stroke
- 'InlineWithoutTexture' - Create a new Texture with only the inline stroke
- 'OutlineAndInlineWithoutTexture' - Create a new Texture with outline and inline strokes (Without original texture)

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
