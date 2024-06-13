namespace DrawnUi.Maui.Draw;

public interface IShaderEffect : ISkiaEffect
{
    SKShader Shader { get; }

    /// <summary>
    /// Pass input texture as `source`, if it is null then will create automatically from snapshot in case AutoCreateInputTexture property is set to True. 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    SKShader CreateShader(SkiaDrawingContext ctx, SKRect destination, SKImage source);
}