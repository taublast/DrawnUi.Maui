namespace DrawnUi.Maui.Draw;

public class SkiaEffect : BindableObject, IDisposable, ICanBeUpdated
{
    protected SkiaControl Parent { get; set; }

    public virtual void Attach(SkiaControl parent)
    {
        Parent = parent;
    }

    protected virtual void OnDisposing()
    {

    }

    public void Dispose()
    {
        OnDisposing();
        Parent = null;
    }

    public void Update()
    {
        Parent?.Update();
    }

    protected static void NeedUpdate(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaEffect effect)
        {
            effect.Update();
        }
    }

    public virtual bool NeedApply
    {
        get
        {
            return true;
        }
    }

}