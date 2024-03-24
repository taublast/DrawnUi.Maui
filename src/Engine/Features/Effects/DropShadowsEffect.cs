using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DrawnUi.Maui.Draw;

[ContentProperty("Shadows")]
public class DropShadowsEffect : BaseRenderEffect
{
    #region PROPERTIES

    public static readonly BindableProperty ShadowsProperty = BindableProperty.Create(
        nameof(Shadows),
        typeof(IList<SkiaShadow>),
        typeof(DropShadowsEffect),
        defaultValueCreator: (instance) =>
        {
            var created = new ObservableCollection<SkiaShadow>();
            ShadowsPropertyChanged(instance, null, created);
            return created;
        },
        validateValue: (bo, v) => v is IList<SkiaShadow>,
        propertyChanged: ShadowsPropertyChanged,
        coerceValue: CoerceShadows);

    public IList<SkiaShadow> Shadows
    {
        get => (IList<SkiaShadow>)GetValue(ShadowsProperty);
        set => SetValue(ShadowsProperty, value);
    }

    private static object CoerceShadows(BindableObject bindable, object value)
    {
        if (!(value is ReadOnlyCollection<SkiaShadow> readonlyCollection))
        {
            return value;
        }

        return new ReadOnlyCollection<SkiaShadow>(
            readonlyCollection.ToList());
    }

    private static void ShadowsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var effect = (DropShadowsEffect)bindable;

        var enumerableShadows = (IEnumerable<SkiaShadow>)newvalue;

        if (oldvalue != null)
        {
            if (oldvalue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= effect.OnShadowCollectionChanged;
            }

            if (oldvalue is IEnumerable<SkiaShadow> oldList)
            {
                foreach (var shade in oldList)
                {
                    shade.Parent = null;
                    shade.BindingContext = null;
                }
            }
        }

        foreach (var shade in enumerableShadows)
        {
            shade.Parent = effect;

            shade.BindingContext = effect.BindingContext;
        }

        if (newvalue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged -= effect.OnShadowCollectionChanged;
            newCollection.CollectionChanged += effect.OnShadowCollectionChanged;
        }

        effect.Update();
    }

    private void OnShadowCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
        case NotifyCollectionChangedAction.Add:
        foreach (SkiaShadow newSkiaPropertyShadow in e.NewItems)
        {
            newSkiaPropertyShadow.Parent = this;
            newSkiaPropertyShadow.BindingContext = BindingContext;
        }

        break;

        case NotifyCollectionChangedAction.Reset:
        case NotifyCollectionChangedAction.Remove:
        foreach (SkiaShadow oldSkiaPropertyShadow in e.OldItems ?? new SkiaShadow[0])
        {
            oldSkiaPropertyShadow.Parent = null;
            oldSkiaPropertyShadow.BindingContext = null;
        }

        break;
        }
    }

    #endregion

    public override void Draw(SkiaControl parent, SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {
        if (NeedApply)
        {
            using var paint = new SKPaint();

            //draw every shadow without the controls itsselfs
            foreach (var shadow in Shadows)
            {
                paint.ImageFilter = SKImageFilter.CreateDropShadowOnly(
                    (float)Math.Round(shadow.X * Parent.RenderingScale),
                    (float)Math.Round(shadow.Y * Parent.RenderingScale),
                    (float)shadow.Blur, (float)shadow.Blur,
                    shadow.Color.ToSKColor());

                var restore = ctx.Canvas.SaveLayer(paint);

                drawControl(ctx);

                if (restore != 0)
                    ctx.Canvas.RestoreToCount(restore);
            }
        }

        //draw as usual
        base.Draw(parent, destination, ctx, drawControl);
    }

    public override bool NeedApply
    {
        get
        {
            return base.NeedApply && Shadows.Count > 0;
        }
    }
}