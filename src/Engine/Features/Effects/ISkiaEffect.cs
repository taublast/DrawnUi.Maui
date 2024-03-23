namespace DrawnUi.Maui.Draw;

public interface ISkiaEffect : ICanBeUpdated
{
    public void SetParent(SkiaControl parent);

    public bool NeedApply { get; }

    public object BindingContext { get; set; }
}