using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Draw;

public class CachedShader : IDisposable
{

    public SKRect Destination { get; set; }
    public SKShader Shader { get; set; }

    public void Dispose()
    {
        Shader?.Dispose();
        Shader = null;
        OnDisposing();
    }

    protected virtual void OnDisposing()
    {

    }
}

public class CachedGradient : CachedShader
{
    public SkiaGradient Gradient { get; set; }

    protected override void OnDisposing()
    {
        Gradient = null;
    }
}

public class CachedShadow : IDisposable
{
    public SkiaShadow Shadow { get; set; }
    public SKImageFilter Filter { get; set; }
    public float Scale { get; set; }

    public void Dispose()
    {
        Filter?.Dispose();
    }
}

public class DescendingZIndexGestureListenerComparer : IComparer<ISkiaGestureListener>
{
    public int Compare(ISkiaGestureListener x, ISkiaGestureListener y)
    {
        // Compare y to x instead of x to y to sort in descending order
        int result = y.ZIndex.CompareTo(x.ZIndex);

        // If ZIndex are equal, compare RegistrationTime in descending order
        if (result == 0)
        {
            result = x.GestureListenerRegistrationTime.CompareTo(y.GestureListenerRegistrationTime);
        }

        // If RegistrationTime is equal, compare Uid to ensure uniqueness
        if (result == 0)
        {
            result = y.Uid.CompareTo(x.Uid);
        }

        return result;
    }
}