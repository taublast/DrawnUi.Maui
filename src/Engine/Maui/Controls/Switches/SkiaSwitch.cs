namespace DrawnUi.Maui.Draw;

/// <summary>
/// Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public class SkiaSwitch : SkiaToggle
{
    #region DEFAULT CONTENT

    protected override void CreateDefaultContent()
    {
        if (this.Views.Count == 0)
        {
            switch (ControlStyle)
            {
                case PrebuiltControlStyle.Cupertino:
                    CreateCupertinoStyleContent();
                    break;
                case PrebuiltControlStyle.Material:
                    CreateMaterialStyleContent();
                    break;
                case PrebuiltControlStyle.Windows:
                    CreateWindowsStyleContent();
                    break;
                case PrebuiltControlStyle.Platform:
                    #if IOS || MACCATALYST
                    CreateCupertinoStyleContent();
                    #elif ANDROID
                    CreateMaterialStyleContent();
                    #elif WINDOWS
                    CreateWindowsStyleContent();
                    #else
                    CreateDefaultStyleContent();
                    #endif
                    break;
                case PrebuiltControlStyle.Unset:
                default:
                    CreateDefaultStyleContent();
                    break;
            }
            
            ApplyProperties();
        }
    }
    
    protected virtual void CreateDefaultStyleContent()
    {
        SetDefaultContentSize(50, 32);

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

        var hotspot = new SkiaHotspot() { TransformView = this.Thumb, };
        hotspot.Tapped += (s, e) => { IsToggled = !IsToggled; };
        this.AddSubView(hotspot);
    }
    
    protected virtual void CreateCupertinoStyleContent()
    {
        SetDefaultContentSize(51, 31);
        
        // Cupertino style has more rounded corners and a thinner shape
        var shape = new SkiaShape
        {
            Tag = "Frame",
            Type = ShapeType.Rectangle,
            CornerRadius = 15.5, // More rounded corners for iOS style
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

        var hotspot = new SkiaHotspot() { TransformView = this.Thumb, };
        hotspot.Tapped += (s, e) => { IsToggled = !IsToggled; };
        this.AddSubView(hotspot);
        
        // iOS default colors
        ColorFrameOff = Color.FromRgba(229, 229, 229, 255);
        ColorFrameOn = Color.FromRgba(48, 209, 88, 255);
        ColorThumbOff = Colors.White;
        ColorThumbOn = Colors.White;
    }
    
    protected virtual void CreateMaterialStyleContent()
    {
        SetDefaultContentSize(46, 28);
        
        // Material style has a more flat track
        var shape = new SkiaShape
        {
            Tag = "Frame",
            Type = ShapeType.Rectangle,
            CornerRadius = 14,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };
        this.AddSubView(shape);

        this.AddSubView(new SkiaShape()
        {
            UseCache = SkiaCacheType.Operations,
            Type = ShapeType.Circle,
            Margin = 1,
            LockRatio = -1,
            Tag = "Thumb",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        });

        var hotspot = new SkiaHotspot() { TransformView = this.Thumb, };
        hotspot.Tapped += (s, e) => { IsToggled = !IsToggled; };
        this.AddSubView(hotspot);
        
        // Material design default colors
        ColorFrameOff = Color.FromRgba(158, 158, 158, 255);
        ColorFrameOn = Color.FromRgba(33, 150, 243, 255);
        ColorThumbOff = Colors.White;
        ColorThumbOn = Colors.White;
    }
    
    protected virtual void CreateWindowsStyleContent()
    {
        SetDefaultContentSize(44, 20);
        
        // Windows style is more squared
        var shape = new SkiaShape
        {
            Tag = "Frame",
            Type = ShapeType.Rectangle,
            CornerRadius = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };
        this.AddSubView(shape);

        this.AddSubView(new SkiaShape()
        {
            UseCache = SkiaCacheType.Operations,
            Type = ShapeType.Rectangle,
            CornerRadius = 8,
            Margin = 2,
            LockRatio = -1,
            Tag = "Thumb",
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
        });

        var hotspot = new SkiaHotspot() { TransformView = this.Thumb, };
        hotspot.Tapped += (s, e) => { IsToggled = !IsToggled; };
        this.AddSubView(hotspot);
        
        // Windows default colors
        ColorFrameOff = Color.FromRgba(153, 153, 153, 255);
        ColorFrameOn = Color.FromRgba(0, 120, 215, 255);
        ColorThumbOff = Colors.White;
        ColorThumbOn = Colors.White;
    }

    #endregion

    public virtual void ApplyOn()
    {
        if (Thumb != null)
        {
            Thumb.TranslationX = GetThumbPosForOn();
            Thumb.BackgroundColor = this.ColorThumbOn;
            Track.BackgroundColor = this.ColorFrameOn;
        }
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

    public SkiaShape Track;
    public SkiaShape Thumb;

    protected virtual void FindViews()
    {
        Track = FindView<SkiaShape>("Frame");
        Thumb = FindView<SkiaShape>("Thumb");
    }

    public override void OnChildrenChanged()
    {
        base.OnChildrenChanged();

        FindViews();
    }

    public override void ApplyProperties()
    {
        if (IsToggled)
        {
            ApplyOn();
        }
        else
        {
            ApplyOff();
        }
    }

    public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        if (args.Type == TouchActionResult.Tapped)
        {
            IsToggled = !IsToggled;
            return this;
        }

        return base.ProcessGestures(args, apply);
    }

    #region ANIMATE

    protected virtual double GetThumbPosForOn()
    {
        if (Track == null || Thumb == null)
            return 0;
            
        // Calculate space for thumb to travel within the track
        var x = Track.Width + Track.Margins.Right + Track.Margins.Left
                - Thumb.Width - Thumb.Margins.Right - Thumb.Margins.Left;
                
        // Handle edge cases for different styles
        switch (ControlStyle)
        {
            case PrebuiltControlStyle.Cupertino:
                // iOS style has precise positioning
                return Math.Max(0, x);
                
            case PrebuiltControlStyle.Material:
                // Material style may need slight adjustment for visual balance
                return Math.Max(0, x);
                
            case PrebuiltControlStyle.Windows:
                // Windows style needs a bit more precise control
                return Math.Max(0, x);
                
            default:
                return Math.Max(0, x);
        }
    }

    protected virtual double GetThumbPosForOff()
    {
        // Default to 0 position for all styles
        return 0;
    }

    public static uint AnimationSpeed = 200;

    protected override void OnToggledChanged()
    {
        if (LayoutReady && IsAnimated)
        {
            // Ensure we have the required elements
            if (Thumb == null || Track == null)
            {
                FindViews();
                if (Thumb == null || Track == null)
                {
                    ApplyProperties();
                    return;
                }
            }
            
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

    #endregion
}
