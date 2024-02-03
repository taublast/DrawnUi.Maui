using DrawnUi.Maui.Draw;
using FontWeight = DrawnUi.Maui.Draw.FontWeight;


namespace DrawnUi.Maui.Infrastructure.Extensions;

public static class DrawnExtensions
{
    public static IFontCollection AddFont(
        this IFontCollection fontCollection,
        string filename,
        string alias, int weight)
    {
        var weightEnum = SkiaFontManager.GetWeightEnum(weight);
        return fontCollection.AddFont(filename, alias, weightEnum);
    }


    public static IFontCollection AddFont(
        this IFontCollection fontCollection,
        string filename,
        string alias, FontWeight weight)
    {
        //FontText alias for weight Regular will be registered as FontTextRegular
        //After that when we look for FontText with weight Regular we will ask FontManager for FontTextRegular
        var newAlias = SkiaFontManager.GetAlias(alias, weight);
        SkiaFontManager.RegisterWeight(alias, weight);
        fontCollection.AddFont(filename, newAlias);

        return fontCollection;
    }

    public static Task AnimateRangeAsync(this SkiaControl owner, Action<double> callback, double start, double end, uint length = 250, Easing easing = null, CancellationTokenSource _cancelTranslate = default)
    {
        RangeAnimator animator = null;
        var tcs = new TaskCompletionSource<bool>(_cancelTranslate.Token);
        tcs.Task.ContinueWith(task =>
        {
            animator?.Dispose();
        });

        animator = new RangeAnimator(owner)
        {
            OnStop = () =>
            {
                if (animator.WasStarted && !_cancelTranslate.IsCancellationRequested)
                {
                    tcs.SetResult(true);
                }
            }
        };
        animator.Start(
            (value) =>
            {
                if (!_cancelTranslate.IsCancellationRequested)
                {
                    callback?.Invoke(value);
                }
                else
                {
                    animator.Stop();
                }
            },
            start, end, length, easing);

        return tcs.Task;
    }


    public static (float RatioX, float RatioY) GetVelocityRatioForChild(this IDrawnBase container,
        ISkiaControl control)
    {
        //return (1, 1);

        var containerWidth = container.Width;
        if (containerWidth <= 0)
            containerWidth = container.MeasuredSize.Units.Width;

        var containerHeight = container.Height;
        if (containerHeight <= 0)
            containerHeight = container.MeasuredSize.Units.Height;

        var velocityRatoX = (float)(containerWidth / control.MeasuredSize.Units.Width);
        var velocityRatoY = (float)(containerHeight / control.MeasuredSize.Units.Height);

        if (float.IsNaN(velocityRatoY) || velocityRatoY == 0)
        {
            velocityRatoY = 1;
        }
        if (float.IsNaN(velocityRatoX) || velocityRatoX == 0)
        {
            velocityRatoX = 1;
        }

        return (velocityRatoX, velocityRatoY);
    }
}
