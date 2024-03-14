using System.Windows.Input;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Button-like control, can include any content inside. It's either you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag `MainLabel`, SkiaShape with Tag `MainFrame`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public class SkiaButton : SkiaLayout, ISkiaGestureListener
{
    public SkiaButton()
    {
    }

    public override void InvalidateMeasureInternal()
    {
        ApplyProperties();

        base.InvalidateMeasureInternal();
    }

    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        var measured = base.Measure(widthConstraint, heightConstraint, scale);
        var test = this.WidthRequest;
        return measured;
    }

    #region DEFAULT CONTENT

    protected override void CreateDefaultContent()
    {
        if (!DefaultChildrenCreated && this.Views.Count == 0)
        {
            if (CreateChildren == null)
            {
                DefaultChildrenCreated = true;

                if (this.WidthRequest < 0)
                    this.WidthRequest = 100;
                if (this.HeightRequest < 0)
                    this.HeightRequest = 32;

                var shape = new SkiaShape
                {
                    Tag = "BtnShape",
                    BackgroundColor = Super.ColorAccent,
                    CornerRadius = 8,
                    HorizontalOptions = LayoutOptions.Fill,
                    IsClippedToBounds = true,
                    VerticalOptions = LayoutOptions.Fill,
                };
                shape.SetBinding(SkiaShape.CornerRadiusProperty, new Binding(nameof(CornerRadius), source: this));
                this.AddSubView(shape);

                this.AddSubView(new SkiaLabel()
                {
                    UseCache = SkiaCacheType.Operations,
                    Tag = "BtnText",
                    Text = "Test",
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                });

                ApplyProperties();
            }

        }

    }

    #endregion

    protected SkiaLabel MainLabel;

    protected SkiaShape MainFrame;

    public virtual void FindViews()
    {
        if (MainLabel == null)
        {
            MainLabel = FindView<SkiaLabel>("BtnText");
        }
        if (MainFrame == null)
        {
            MainFrame = FindView<SkiaShape>("BtnShape");
        }
    }

    public virtual void ApplyProperties()
    {
        FindViews();

        if (MainLabel != null)
        {
            MainLabel.Text = this.Text;
            MainLabel.TextColor = this.TextColor;
        }

        if (MainFrame != null)
        {
            MainFrame.BackgroundColor = this.TintColor;
            MainFrame.CornerRadius = this.CornerRadius;
        }

    }

    public virtual bool OnDown(TouchActionEventArgs args, SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener wasConsumed)
    {
        if (this.ApplyEffect != SkiaTouchAnimation.None)
        {
            var control = this as SkiaControl;
            if (this.TransformView is SkiaControl other)
            {
                control = other;
            }

            if (ApplyEffect == SkiaTouchAnimation.Ripple)
            {
                var ptsInsideControl = GetOffsetInsideControlInPoints(args.Location, childOffset);
                control.PlayRippleAnimation(TouchEffectColor, ptsInsideControl.X, ptsInsideControl.Y);
            }
            else
            if (ApplyEffect == SkiaTouchAnimation.Shimmer)
            {
                var color = ShimmerEffectColor;
                control.PlayShimmerAnimation(color, ShimmerEffectWidth, ShimmerEffectAngle, ShimmerEffectSpeed);
            }
        }

        return false;
    }

    public virtual void OnUp()
    {

    }


    public virtual bool OnTapped(TouchActionEventArgs args, SKPoint childOffset)
    {
        var ret = false;

        if (!IsDisabled)
        {
            if (Tapped != null)
            {
                ret = true;
                Tapped?.Invoke(this, args);
            }
            if (CommandTapped != null)
            {
                ret = true;
                Tasks.StartDelayedAsync(TimeSpan.FromMilliseconds(DelayCallbackMs), async () =>
                {
                    await Task.Run(() => { CommandTapped?.Execute(CommandTappedParameter); }).ConfigureAwait(false);
                });
            }
            else
            {
                Debug.WriteLine("[SkiaButton] Command NULL");
            }

        }

        return ret;
    }

    bool hadDown;

    public static float PanThreshold = 5;

    public override ISkiaGestureListener ProcessGestures(
        TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
    SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {
        Trace.WriteLine($"SkiaButton. {type} {touchAction} {args.Location.X} {args.Location.Y}");

        var point = TranslateInputOffsetToPixels(args.Location, childOffset);

        var ret = false;

        void SetUp()
        {
            hadDown = false; //todo track multifingers
            Up?.Invoke(this, args);
            OnUp();
        }

        if (touchAction == TouchActionResult.Down)
        {
            _lastDownPts = point;
            hadDown = true;
            TotalDown++;
            Down?.Invoke(this, args);
            return OnDown(args, childOffset, childOffsetDirect, alreadyConsumed) ? this : null;
        }

        if (touchAction == TouchActionResult.Panning)
        {
            var current = point;
            if (Math.Abs(current.X - _lastDownPts.X) > PanThreshold
                || Math.Abs(current.Y - _lastDownPts.Y) > PanThreshold)
            {
                SetUp();
                return null;
            }
        }
        else
        if (touchAction == TouchActionResult.Up)
        {
            hadDown = false; //todo track multifingers
            Up?.Invoke(this, args);
            OnUp();
        }
        else
        if (touchAction == TouchActionResult.Tapped)
        {
            TotalTapped++;
            return OnTapped(args, childOffset) ? this : null;
        }

        return hadDown ? this : null;
    }

    /// <summary>
    /// You might want to pause to show effect before executing command. Default is 0.
    /// </summary>
    public static int DelayCallbackMs = 0;

    public event EventHandler<TouchActionEventArgs> Up;

    public event EventHandler<TouchActionEventArgs> Down;

    public event EventHandler<TouchActionEventArgs> Tapped;



    private long _TotalTapped;
    public long TotalTapped
    {
        get
        {
            return _TotalTapped;
        }
        set
        {
            if (_TotalTapped != value)
            {
                _TotalTapped = value;
                OnPropertyChanged();
            }
        }
    }

    private long _TotalDown;
    public long TotalDown
    {
        get
        {
            return _TotalDown;
        }
        set
        {
            if (_TotalDown != value)
            {
                _TotalDown = value;
                OnPropertyChanged();
            }
        }
    }


    protected SKPoint _lastDownPts;


    #region STATIC PROPERTIES

    public static readonly BindableProperty IsDisabledProperty = BindableProperty.Create(
        nameof(IsDisabled),
        typeof(bool),
        typeof(SkiaButton),
        false, propertyChanged: NeedApplyProperties);

    private static void NeedApplyProperties(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaButton control)
        {
            control.ApplyProperties();
        }
    }

    public bool IsDisabled
    {
        get { return (bool)GetValue(IsDisabledProperty); }
        set { SetValue(IsDisabledProperty, value); }
    }


    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(SkiaButton),
        string.Empty, propertyChanged: NeedApplyProperties);

    /// <summary>
    /// Bind to your own content!
    /// </summary>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }


    public static readonly BindableProperty ShimmerEffectColorProperty = BindableProperty.Create(nameof(ShimmerEffectColor),
        typeof(Color),
        typeof(SkiaButton),
        Colors.White.WithAlpha(0.33f));
    public Color ShimmerEffectColor
    {
        get { return (Color)GetValue(ShimmerEffectColorProperty); }
        set { SetValue(ShimmerEffectColorProperty, value); }
    }

    public static readonly BindableProperty ShimmerEffectAngleProperty = BindableProperty.Create(nameof(ShimmerEffectAngle),
        typeof(float),
        typeof(SkiaButton),
        33.0f);
    public float ShimmerEffectAngle
    {
        get { return (float)GetValue(ShimmerEffectAngleProperty); }
        set { SetValue(ShimmerEffectAngleProperty, value); }
    }

    public static readonly BindableProperty ShimmerEffectWidthProperty = BindableProperty.Create(nameof(ShimmerEffectWidth),
        typeof(float),
        typeof(SkiaButton),
        150.0f);
    public float ShimmerEffectWidth
    {
        get { return (float)GetValue(ShimmerEffectWidthProperty); }
        set { SetValue(ShimmerEffectWidthProperty, value); }
    }

    public static readonly BindableProperty ShimmerEffectSpeedProperty = BindableProperty.Create(nameof(ShimmerEffectSpeed),
        typeof(int),
        typeof(SkiaButton),
        500);
    public int ShimmerEffectSpeed
    {
        get { return (int)GetValue(ShimmerEffectSpeedProperty); }
        set { SetValue(ShimmerEffectSpeedProperty, value); }
    }


    public static readonly BindableProperty TouchEffectColorProperty = BindableProperty.Create(nameof(TouchEffectColor), typeof(Color),
         typeof(SkiaButton),
        Colors.White);
    public Color TouchEffectColor
    {
        get { return (Color)GetValue(TouchEffectColorProperty); }
        set { SetValue(TouchEffectColorProperty, value); }
    }

    public static readonly BindableProperty ApplyEffectProperty = BindableProperty.Create(nameof(ApplyEffect),
        typeof(SkiaTouchAnimation),
        typeof(SkiaButton), SkiaTouchAnimation.Ripple);
    public SkiaTouchAnimation ApplyEffect
    {
        get { return (SkiaTouchAnimation)GetValue(ApplyEffectProperty); }
        set { SetValue(ApplyEffectProperty, value); }
    }

    public static readonly BindableProperty TransformViewProperty = BindableProperty.Create(nameof(TransformView), typeof(object),
        typeof(SkiaButton), null);
    public object TransformView
    {
        get { return (object)GetValue(TransformViewProperty); }
        set { SetValue(TransformViewProperty, value); }
    }

    public static readonly BindableProperty CommandTappedProperty = BindableProperty.Create(nameof(CommandTapped), typeof(ICommand),
        typeof(SkiaButton),
        null);
    public ICommand CommandTapped
    {
        get { return (ICommand)GetValue(CommandTappedProperty); }
        set { SetValue(CommandTappedProperty, value); }
    }

    public static readonly BindableProperty CommandTappedParameterProperty = BindableProperty.Create(nameof(CommandTappedParameter), typeof(object),
        typeof(SkiaButton),
        null);
    public object CommandTappedParameter
    {
        get { return GetValue(CommandTappedParameterProperty); }
        set { SetValue(CommandTappedParameterProperty, value); }
    }

    public static readonly BindableProperty CommandLongPressingProperty = BindableProperty.Create(nameof(CommandLongPressing), typeof(ICommand),
        typeof(SkiaButton),
        null);
    public ICommand CommandLongPressing
    {
        get { return (ICommand)GetValue(CommandLongPressingProperty); }
        set { SetValue(CommandLongPressingProperty, value); }
    }

    public static readonly BindableProperty CommandLongPressingParameterProperty = BindableProperty.Create(nameof(CommandLongPressingParameter), typeof(object),
        typeof(SkiaButton),
        null);
    public object CommandLongPressingParameter
    {
        get { return GetValue(CommandLongPressingParameterProperty); }
        set { SetValue(CommandLongPressingParameterProperty, value); }
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(Thickness),
        typeof(SkiaButton),
        new Thickness(8),
        propertyChanged: NeedInvalidateMeasure);

    public Thickness CornerRadius
    {
        get { return (Thickness)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor),
        typeof(Color),
        typeof(SkiaButton),
        Colors.Red, propertyChanged: NeedApplyProperties);

    public Color TintColor
    {
        get { return (Color)GetValue(TintColorProperty); }
        set { SetValue(TintColorProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SkiaButton),
        Colors.White, propertyChanged: NeedApplyProperties);

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    #endregion


}