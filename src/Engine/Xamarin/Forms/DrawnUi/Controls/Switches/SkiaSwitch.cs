namespace DrawnUi.Draw;

/// <summary>
/// Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public class SkiaSwitch : SkiaToggle
{
    #region DEFAULT CONTENT

    protected override void CreateDefaultContent()
    {
        //todo can make different upon platform!
        if (!DefaultChildrenCreated && this.Views.Count == 0)
        {
            if (CreateChildren == null)
            {
                DefaultChildrenCreated = true;

                if (this.WidthRequest < 0)
                    this.WidthRequest = 50;
                if (this.HeightRequest < 0)
                    this.HeightRequest = 32;

                var shape = new SkiaShape
                {
                    Tag = "Frame",
                    Type = ShapeType.Rectangle,
                    CornerRadius = 20,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
                this.AddSubView(shape);

                this.AddSubView(new SkiaShape()
                {
                    UseCache = SkiaCacheType.Operations,
                    Type = ShapeType.Circle,
                    Margin = 2,
                    LockRatio = -1,
                    Tag = "Thumb",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Fill,
                });

                var hotspot = new SkiaHotspot()
                {
                    TransformView = this.Thumb,
                };
                hotspot.Tapped += (s, e) =>
                {
                    IsToggled = !IsToggled;
                };
                this.AddSubView(hotspot);

                ApplyProperties();
            }

        }

    }

    #endregion

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        ApplyProperties();
    }

    public virtual void ApplyOff()
    {
        if (Thumb != null)
        {
            Thumb.TranslationX = GetThumbPosForOff();
            Thumb.BackgroundColor = this.ColorThumbOff;
            Track.BackgroundColor = this.ColorFrameOff;
        }
    }

    public virtual void ApplyOn()
    {
        if (Thumb != null)
        {
            Thumb.TranslationX = GetThumbPosForOn();
            Thumb.BackgroundColor = this.ColorThumbOn;
            Track.BackgroundColor = this.ColorFrameOn;
        }
    }

    public SkiaShape Track;
    public SkiaShape Thumb;

    protected virtual double GetThumbPosForOn()
    {
        var x = Track.Width + Track.Margins.Right + Track.Margins.Left
                - Thumb.Width - Thumb.Margins.Right - Thumb.Margins.Left;
        return x;
    }

    protected virtual double GetThumbPosForOff()
    {
        return 0;
    }

    protected virtual void FindViews()
    {
        Track = FindView<SkiaShape>("Frame");
        Thumb = FindView<SkiaShape>("Thumb");
    }

    public override void ApplyProperties()
    {
        if (Track == null)
        {
            FindViews();
        }

        if (IsToggled)
        {
            ApplyOn();
        }
        else
        {
            ApplyOff();
        }
    }

    public static uint AnimationSpeed = 200;

    protected override void OnToggledChanged()
    {
        if (LayoutReady && IsAnimated)
        {
            var easing = Easing.CubicOut;
            var msSpeed = AnimationSpeed;
            var pos = 0.0;
            if (!IsToggled)
            {
                Task.Run(async () =>
                {
                    await Thumb.TranslateToAsync(pos, 0, msSpeed, easing);
                    ApplyOff();
                });
            }
            else
            {
                pos = GetThumbPosForOn();
                Task.Run(async () =>
                {
                    await Thumb.TranslateToAsync(pos, 0, msSpeed, easing);
                    ApplyOn();
                });
            }
        }
        else
        {
            ApplyProperties();
        }
    }

}