using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Infrastructure.Extensions;
using System.Windows.Input;


namespace DrawnUi.Maui.Draw;

/// <summary>
/// Button-like control, can include any content inside. It's either you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag `MainLabel`, SkiaShape with Tag `MainFrame`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public partial class SkiaButton : SkiaLayout, ISkiaGestureListener
{
    public SkiaButton()
    {
    }

    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        if (IsDisposed || IsDisposing)
            return ScaledSize.Default;

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

                if (this.WidthRequest < 0 && HorizontalOptions.Alignment != LayoutAlignment.Fill)
                    this.WidthRequest = 100;

                if (this.HeightRequest < 0 && VerticalOptions.Alignment != LayoutAlignment.Fill)
                    this.HeightRequest = 40;

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
                    TextColor = BlackColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                });

                ApplyProperties();
            }

        }
        else
        {
            ApplyProperties();
        }
    }

    #endregion

    /// <summary>
    /// Clip effects with rounded rect of the frame inside
    /// </summary>
    /// <returns></returns>
    public override SKPath CreateClip(object arguments, bool usePosition, SKPath path = null)
    {
        if (MainFrame != null)
        {
            return MainFrame.CreateClip(arguments, false);
            //var offsetFrame = new SKPoint(MainFrame.DrawingRect.Left - DrawingRect.Left, MainFrame.DrawingRect.Top - DrawingRect.Top);
            //var clip = MainFrame.CreateClip(arguments, usePosition); ;
            //clip.Offset(offsetFrame);
            //return clip;
        }

        return base.CreateClip(arguments, usePosition);
    }

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
            MainLabel.StrokeColor = TextStrokeColor;
            MainLabel.FontFamily = this.FontFamily;
            MainLabel.FontSize = this.FontSize;
        }

        if (MainFrame != null)
        {
            MainFrame.BackgroundColor = this.BackgroundColor;
            MainFrame.Background = this.Background;
            MainFrame.CornerRadius = this.CornerRadius;
        }
    }

    public virtual bool OnDown(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        Pressed?.Invoke(this, args);

        if (this.ApplyEffect != SkiaTouchAnimation.None)
        {
            var control = this as SkiaControl;
            if (this.TransformView is SkiaControl other)
            {
                control = other;
            }

            if (ApplyEffect == SkiaTouchAnimation.Ripple)
            {
                var ptsInsideControl = GetOffsetInsideControlInPoints(args.Event.Location, apply.childOffset);
                control.PlayRippleAnimation(TouchEffectColor, ptsInsideControl.X, ptsInsideControl.Y);
            }
            else
            if (ApplyEffect == SkiaTouchAnimation.Shimmer)
            {
                var color = ShimmerEffectColor;
                control.PlayShimmerAnimation(color, ShimmerEffectWidth, ShimmerEffectAngle, ShimmerEffectSpeed);
            }
        }

        return true;
    }

    public virtual void OnUp(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        Released?.Invoke(this, args);
    }

    public virtual bool OnTapped(SkiaGesturesParameters args, SKPoint childOffset)
    {
        var ret = false;

        if (!IsDisabled)
        {
            if (Tapped != null)
            {
                ret = true;
                Tapped?.Invoke(this, args);
            }
            if (Clicked != null)
            {
                ret = true;
                Clicked(this, args);
            }
            if (CommandTapped != null)
            {
                ret = true;
                Tasks.StartDelayedAsync(TimeSpan.FromMilliseconds(DelayCallbackMs), async () =>
                {
                    await Task.Run(() => { CommandTapped?.Execute(CommandTappedParameter); }).ConfigureAwait(false);
                });
            }

        }

        return ret;
    }

    bool hadDown;
    protected SKPoint _lastDownPts;

    public static float PanThreshold = 5;

    public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        //Debug.WriteLine($"SkiaButton {Text}. {args.Type} {args.Event.Distance.Delta}");

        var point = TranslateInputOffsetToPixels(args.Event.Location, apply.childOffset);

        var ret = false;

        void SetUp()
        {
            IsPressed = false;
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    IsPressed = false;
            //});
            hadDown = false; //todo track multifingers
            Up?.Invoke(this, args);
            OnUp(args, apply);
        }

        if (args.Type == TouchActionResult.Down)
        {
            IsPressed = true;
            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    IsPressed = true;
            //});
            _lastDownPts = point;
            hadDown = true;
            TotalDown++;
            Down?.Invoke(this, args);
            return OnDown(args, apply) ? this : null;
        }

        if (args.Type == TouchActionResult.Panning)
        {
            if (LockPanning)
            {
                return this; //no panning for you my friend 
            }

            var current = point;
            var panthreshold = PanThreshold * RenderingScale;

            if (Math.Abs(current.X - _lastDownPts.X) > panthreshold
                || Math.Abs(current.Y - _lastDownPts.Y) > panthreshold)
            {
                if (hadDown)
                    SetUp();
                hadDown = false;

                return null;
            }
        }
        else
        if (args.Type == TouchActionResult.Up)
        {
            //todo track multifingers?
            SetUp();
            //hadDown = false; 
            //Up?.Invoke(this, args);
            //OnUp();
        }
        else
        if (args.Type == TouchActionResult.Tapped)
        {
            TotalTapped++;
            return OnTapped(args, apply.childOffset) ? this : null;
        }

        return hadDown ? this : null;
    }

    /// <summary>
    /// You might want to pause to show effect before executing command. Default is 0.
    /// </summary>
    public static int DelayCallbackMs = 0;

    public event EventHandler<SkiaGesturesParameters> Up;

    public event EventHandler<SkiaGesturesParameters> Down;

    public event EventHandler<SkiaGesturesParameters> Tapped;

    /// <summary>
    /// Occurs when the button is clicked/tapped (Tapped event).
    /// </summary>
    public Action<SkiaButton, SkiaGesturesParameters> Clicked;

    /// <summary>
    /// Occurs when the button is released (Up event).
    /// </summary>
    public Action<SkiaButton, SkiaGesturesParameters> Released;

    /// <summary>
    /// Occurs when the button is pressed (Down event).
    /// </summary>
    public Action<SkiaButton, SkiaGesturesParameters> Pressed;


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




    #region PROPERTIES

    private static void OnLookChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaButton control)
        {
            control.ApplyProperties();
        }
    }

    public static readonly BindableProperty LockPanningProperty = BindableProperty.Create(nameof(LockPanning),
        typeof(bool),
        typeof(SkiaButton),
        false);
    public bool LockPanning
    {
        get { return (bool)GetValue(LockPanningProperty); }
        set { SetValue(LockPanningProperty, value); }
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(SkiaButton),
        12.0,
        propertyChanged: OnLookChanged);

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(SkiaButton),
        defaultValue: string.Empty,
        propertyChanged: OnLookChanged);

    public string FontFamily
    {
        get { return (string)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public static readonly BindableProperty IsDisabledProperty = BindableProperty.Create(
        nameof(IsDisabled),
        typeof(bool),
        typeof(SkiaButton),
        false, propertyChanged: OnLookChanged);

    public bool IsDisabled
    {
        get { return (bool)GetValue(IsDisabledProperty); }
        set { SetValue(IsDisabledProperty, value); }
    }

    public static readonly BindableProperty IsPressedProperty = BindableProperty.Create(
        nameof(IsPressed),
        typeof(bool),
        typeof(SkiaButton),
        false,
        BindingMode.OneWayToSource);

    public bool IsPressed
    {
        get { return (bool)GetValue(IsPressedProperty); }
        set { SetValue(IsPressedProperty, value); }
    }


    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(SkiaButton),
        string.Empty, propertyChanged: OnLookChanged);

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
        WhiteColor.WithAlpha(0.33f));
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
        WhiteColor);
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


    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SkiaButton),
        WhiteColor,
        propertyChanged: NeedApplyProperties);

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly BindableProperty TextStrokeColorProperty = BindableProperty.Create(
        nameof(TextStrokeColor),
        typeof(Color),
        typeof(SkiaButton),
        TransparentColor,
        propertyChanged: NeedApplyProperties);

    public Color TextStrokeColor
    {
        get { return (Color)GetValue(TextStrokeColorProperty); }
        set { SetValue(TextStrokeColorProperty, value); }
    }

    private static void NeedApplyProperties(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaButton control)
        {
            control.ApplyProperties();
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.IsEither(nameof(Background), nameof(BackgroundColor)))
        {
            ApplyProperties();
        }
    }

    #endregion

    protected override bool SetupBackgroundPaint(SKPaint paint, SKRect destination)
    {
        if (MainFrame != null)
        {
            //will paint its background instead
            return false;
        }

        return base.SetupBackgroundPaint(paint, destination);
    }
}