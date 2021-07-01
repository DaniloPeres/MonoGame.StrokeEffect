<p align="center">
  <img src="http://daniloperes.com/MonoGame.StrokeEffect_Logo_256.png" alt="MonoGame.StrokeEffect" width="120" height="120">
</p>

# MonoGame.StrokeEffect
<b>MonoGame.StrokeEffect</b> is a library to generate strokes for Texture2D in MonoGame. We also support Sprite Font.
We use a shader effect to generate the Stroke effect.

## Nuget package
There is a nuget package avaliable here https://www.nuget.org/packages/MonoGame.StrokeEffect/.

# Examples

<img src="http://daniloperes.com/MonoGame.StrokeEffect.Sample.gif?1" alt="MonoGame.StrokeEffect" width="600" height="469">

https://github.com/DaniloPeres/MonoGame.StrokeEffect/tree/main/MonoGame.StrokeEffect-Samples

## Anti-Aliasing

<img src="http://daniloperes.com/MonoGame.StrokeEffect.Anti-Aliasing.png?1" alt="MonoGame.StrokeEffect" width="450" height="170">

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
