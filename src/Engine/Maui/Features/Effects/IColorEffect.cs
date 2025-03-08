namespace DrawnUi.Maui.Draw;

public interface IColorEffect : ISkiaEffect
{
    SKColorFilter Filter { get; }

    SKColorFilter CreateFilter(SKRect destination);
}