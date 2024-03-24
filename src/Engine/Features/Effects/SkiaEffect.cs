namespace DrawnUi.Maui.Draw;

public class SkiaEffect : BindableObject, IDisposable, ICanBeUpdatedWithContext
{
    /// <summary>
    /// For public set use Attach/Detach
    /// </summary>
    public SkiaControl Parent { get; protected set; }

    protected virtual void OnDisposing()
    {

    }

    public void Attach(SkiaControl parent)
    {
        this.Parent = parent;
        this.BindingContext = parent.BindingContext;
    }

    public void Dettach()
    {
        this.BindingContext = null;
        this.Parent = null;
    }

    public void Dispose()
    {
        OnDisposing();
        Parent = null;
    }

    public virtual void Update()
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
            return Parent != null;
        }
    }

}