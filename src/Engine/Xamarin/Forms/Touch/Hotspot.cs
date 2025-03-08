using AppoMobi.Maui.Gestures;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace AppoMobi.Framework.Forms.UI.Touch;

public class Hotspot : ContentView, ITouchView, IGestureListener
{


    public double RenderingScale
    {
        get
        {
            return Screen.DisplayInfo.Density;
        }
    }

    #region GESTURES

    public static Color DebugGesturesColor { get; set; } = Color.Transparent;// Color.Parse("#ff0000");
    /// <summary>
    /// To filter micro-gestures on super sensitive screens, start passing panning only when threshold is once overpassed
    /// </summary>
    public static float FirstPanThreshold = 5;

    bool _isPanning;


    private SKPoint _panningOffset;


    /// <summary>
    /// IGestureListener implementation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args1"></param>
    /// <param name="args1"></param>
    /// <param name=""></param>
    public void OnGestureEvent(TouchActionType type, TouchActionEventArgs args1, TouchActionResult touchAction)
    {
        var args = SkiaGesturesParameters.Create(touchAction, args1);

        if (args.Type == TouchActionResult.Panning)
        {
            //filter micro-gestures
            if ((Math.Abs(args.Event.Distance.Delta.X) < 1 && Math.Abs(args.Event.Distance.Delta.Y) < 1)
                || (Math.Abs(args.Event.Distance.Velocity.X / RenderingScale) < 1 && Math.Abs(args.Event.Distance.Velocity.Y / RenderingScale) < 1))
            {
                return;
            }

            var threshold = FirstPanThreshold * RenderingScale;

            if (!_isPanning)
            {
                //filter first panning movement on super sensitive screens
                if (Math.Abs(args.Event.Distance.Total.X) < threshold && Math.Abs(args.Event.Distance.Total.Y) < threshold)
                {
                    _panningOffset = SKPoint.Empty;
                    return;
                }

                if (_panningOffset == SKPoint.Empty)
                {
                    _panningOffset = args.Event.Distance.Total.ToSKPoint();
                }

                //args.PanningOffset = _panningOffset;

                _isPanning = true;
            }
        }

        if (args.Type == TouchActionResult.Down)
        {
            _isPanning = false;
        }

        ProcessGestures(args);
    }

    protected VelocityAccumulator VelocityAccumulator { get; } = new();

    protected virtual void ResetPan()
    {
        WasSwiping = false;
        IsUserFocused = true;
        IsUserPanning = false;

        VelocityAccumulator.Clear();
    }

    protected bool WasSwiping { get; set; }

    protected bool IsUserFocused { get; set; }

    protected bool IsUserPanning { get; set; }

    public float VelocityY
    {
        get
        {
            return _velocityY;
        }

        protected set
        {
            if (Math.Abs(value) > MaxVelocity)
            {
                value = MaxVelocity * Math.Sign(value);
            }
            if (_velocityY != value)
            {
                _velocityY = value;
                OnPropertyChanged();
            }
        }
    }
    float _velocityY;

    float _velocityX;

    public float VelocityX
    {
        get
        {
            return _velocityX;
        }

        protected set
        {
            if (Math.Abs(value) > MaxVelocity)
            {
                value = MaxVelocity * Math.Sign(value);
            }
            if (_velocityX != value)
            {
                _velocityX = value;
                OnPropertyChanged();
            }
        }
    }

    public static readonly BindableProperty MaxVelocityProperty = BindableProperty.Create(
        nameof(MaxVelocity),
        typeof(float),
        typeof(Hotspot),
        5000f);

    /// <summary>
    /// Limit user input velocity
    /// </summary>
    public float MaxVelocity
    {
        get { return (float)GetValue(MaxVelocityProperty); }
        set { SetValue(MaxVelocityProperty, value); }
    }

    /// <summary>
    /// Min velocity in points/sec to flee/swipe when finger is up
    /// </summary>
    public static float ThesholdSwipeOnUp = 40f;

    protected virtual void ProcessGestures(SkiaGesturesParameters args)
    {
        VelocityX = (float)(args.Event.Distance.Velocity.X / RenderingScale);
        VelocityY = (float)(args.Event.Distance.Velocity.Y / RenderingScale);

        switch (args.Type)
        {
            case TouchActionResult.Up:
            OnUp1(this, args.Event);

            var swipeThreshold = ThesholdSwipeOnUp * RenderingScale;
            if (Math.Abs(VelocityX) > swipeThreshold || Math.Abs(VelocityY) > swipeThreshold)
            {
                if (Math.Abs(args.Event.Distance.Delta.X) > Math.Abs(args.Event.Distance.Delta.Y))
                {
                    if (args.Event.Distance.Total.X > 0)
                        SwipedRight?.Invoke(this, args.Event);
                    else
                        SwipedLeft?.Invoke(this, args.Event);
                }
                else
                {
                    if (args.Event.Distance.Total.Y > 0)
                        SwipedDown?.Invoke(this, args.Event);
                    else
                        SwipedUp?.Invoke(this, args.Event);
                }
            }

            break;
            case TouchActionResult.Down:
            OnDown1(this, args.Event);
            break;
            case TouchActionResult.LongPressing:
            OnLongPressing(this, args.Event);
            break;
            case TouchActionResult.Tapped:
            OnTapped(this, args.Event);
            break;
            case TouchActionResult.Panning:

            _panDirection = DirectionType.Vertical;
            if (Math.Abs(args.Event.Distance.Delta.X) > Math.Abs(args.Event.Distance.Delta.Y))
            {
                _panDirection = DirectionType.Horizontal;
            }

            OnPanning(this, args.Event);
            break;
        }
    }

    #endregion



    public event EventHandler<TouchActionEventArgs> Tapped;
    public event EventHandler<TouchActionEventArgs> Up;
    public event EventHandler<TouchActionEventArgs> Down;
    public event EventHandler<TouchActionEventArgs> Panning;
    public event EventHandler<TouchActionEventArgs> SwipedLeft;
    public event EventHandler<TouchActionEventArgs> SwipedRight;
    public event EventHandler<TouchActionEventArgs> SwipedUp;
    public event EventHandler<TouchActionEventArgs> SwipedDown;

    public virtual void OnUp(TouchActionEventArgs args)
    {
        Up?.Invoke(this, args);
        CommandUp?.Execute(args);
    }

    public virtual void OnDown(TouchActionEventArgs args)
    {
        Down?.Invoke(this, args);
        CommandDown?.Execute(args);
    }

    protected void SyncTouchMode(TouchHandlingStyle mode)
    {
        TouchEffect.SetShareTouch(this, mode);
    }



    //-------------------------------------------------------------
    // TouchMode
    //-------------------------------------------------------------
    private const string nameTouchMode = "TouchMode";

    public static readonly BindableProperty TouchModeProperty = BindableProperty.Create(nameTouchMode,
        typeof(TouchHandlingStyle), typeof(Hotspot),
        TouchHandlingStyle.Default);

    public TouchHandlingStyle TouchMode
    {
        get { return (TouchHandlingStyle)GetValue(TouchModeProperty); }
        set { SetValue(TouchModeProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandLongPressingParameter
    //-------------------------------------------------------------
    private const string nameCommandLongPressingParameter = "CommandLongPressingParameter";

    public static readonly BindableProperty CommandLongPressingParameterProperty = BindableProperty.Create(
        nameCommandLongPressingParameter, typeof(object), typeof(ITouchView),
        null);
    public object CommandLongPressingParameter
    {
        get { return GetValue(CommandLongPressingParameterProperty); }
        set { SetValue(CommandLongPressingParameterProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandTapped
    //-------------------------------------------------------------
    private const string nameCommandTapped = "CommandTapped";
    public static readonly BindableProperty CommandTappedProperty = BindableProperty.Create(nameCommandTapped, typeof(ICommand), typeof(ITouchView),
        null);
    public ICommand CommandTapped
    {
        get { return (ICommand)GetValue(CommandTappedProperty); }
        set { SetValue(CommandTappedProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandSwiped
    //-------------------------------------------------------------
    private const string nameCommandSwiped = "CommandSwiped";
    public static readonly BindableProperty CommandSwipedProperty = BindableProperty.Create(nameCommandSwiped, typeof(ICommand), typeof(ITouchView),
        null);
    public ICommand CommandSwiped
    {
        get { return (ICommand)GetValue(CommandSwipedProperty); }
        set { SetValue(CommandSwipedProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandUp
    //-------------------------------------------------------------
    private const string nameCommandUp = "CommandUp";
    public static readonly BindableProperty CommandUpProperty = BindableProperty.Create(nameCommandUp, typeof(ICommand), typeof(ITouchView),
        null);
    public ICommand CommandUp
    {
        get { return (ICommand)GetValue(CommandUpProperty); }
        set { SetValue(CommandUpProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandDown
    //-------------------------------------------------------------
    private const string nameCommandDown = "CommandDown";
    public static readonly BindableProperty CommandDownProperty = BindableProperty.Create(nameCommandDown, typeof(ICommand), typeof(ITouchView),
        null);
    public ICommand CommandDown
    {
        get { return (ICommand)GetValue(CommandDownProperty); }
        set { SetValue(CommandDownProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandTappedParameter
    //-------------------------------------------------------------
    private const string nameCommandTappedParameter = "CommandTappedParameter";
    public static readonly BindableProperty CommandTappedParameterProperty = BindableProperty.Create(nameCommandTappedParameter, typeof(object), typeof(ITouchView),
        null);
    public object CommandTappedParameter
    {
        get { return GetValue(CommandTappedParameterProperty); }
        set { SetValue(CommandTappedParameterProperty, value); }
    }

    //-------------------------------------------------------------
    // CommandLongPressing
    //-------------------------------------------------------------
    private const string nameCommandLongPressing = "CommandLongPressing";
    public static readonly BindableProperty CommandLongPressingProperty = BindableProperty.Create(nameCommandLongPressing, typeof(ICommand), typeof(ITouchView),
        null);
    public ICommand CommandLongPressing
    {
        get { return (ICommand)GetValue(CommandLongPressingProperty); }
        set { SetValue(CommandLongPressingProperty, value); }
    }

    public Hotspot()
    {

    }

    private bool lockTap;

    private void OnTapped(object sender, TouchActionEventArgs args)
    {
        if (InputTransparent)
            return;

        if (lockTap && TouchEffect.LockTimeTimeMsDefault > 0)
            return;

        lockTap = true;

        //invoke action
        Tapped?.Invoke(this, args);
        if (CommandTapped != null)
        {
            Debug.WriteLine($"[HOTSPOT] Executing tap command..");
            CommandTapped.Execute(CommandTappedParameter);
        }

        if (TouchEffect.LockTimeTimeMsDefault > 0)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(TouchEffect.LockTimeTimeMsDefault), () =>
            {
                lockTap = false;
                return false;
            });
        }
        else
        {
            lockTap = false;
        }
    }

    //private void OnUp(object sender, TouchActionEventArgs args)
    //{
    //    //Debug.WriteLine($"[TOUCH] UP");

    //    TouchDown = false;
    //}

    //private void OnDown(object sender, TouchActionEventArgs args)
    //{
    //    //Debug.WriteLine($"[TOUCH] DOWN");
    //    TouchDown = true;
    //}

    private void OnTouch(object sender, TouchActionEventArgs args)
    {
        Debug.WriteLine($"[TOUCH] {args.Type} {JsonConvert.SerializeObject(args)}");
    }

    void AttachGestures()
    {
        TouchEffect.SetForceAttach(this, true);
    }

    public virtual void OnPanning(object sender, TouchActionEventArgs args)
    {
        Panning?.Invoke(this, args);
    }

    protected void OnUp1(object sender, TouchActionEventArgs args)
    {
        if (TransformView != null)
        {
            if (animating)
            {
                if (Reaction == HotspotReaction.Minify)
                {
                    TransformView.Scale = _savedScale;
                }
                else
                if (Reaction == HotspotReaction.Zoom)
                {
                    TransformView.Scale = _savedScale;
                }
                animating = false;
            }

            TransformView.Opacity = _savedOpacity;
            TransformView.BackgroundColor = _savedColor;
        }

        TouchDown = false;

        if (!wentOut && !_panning)
        {
            //TappedSmartCommand?.Execute(CommandTappedParameter);
        }

        if (!_panning)
            wentOut = false;

        OnUp(args);
    }

    protected bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        DetachGestures();
    }

    protected void DetachGestures()
    {
        TouchEffect.SetForceAttach(this, false);
    }

    private bool _panning;
    private bool wentOut;

    //private void OnPanned(object sender, PanEventArgs e)
    //{
    //    //        Debug.WriteLine("[TOUCH] OnPanned");


    //    if (e.Center.X < 0 || e.Center.X > Width || e.Center.Y < 0 || e.Center.Y > Height)
    //        wentOut = true;

    //    if (_panning)
    //    {
    //        if (!wentOut)
    //        {
    //            //TappedSmartCommand?.Execute(CommandTappedParameter);
    //        }

    //        _panning = false;
    //        wentOut = false;
    //    }

    //}

    //private void OnPanning(object sender, PanEventArgs e)
    //{
    //    //   Debug.WriteLine("[TOUCH] OnPanning");

    //    _panning = true;
    //}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
    }

    private bool _isRendererSet;
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        //Debug.WriteLine($"[1] {propertyName}");

        base.OnPropertyChanged(propertyName);

        if (propertyName == "Renderer") //TRICK NOT WORKING INSIDE SHELL !!!!
        {
            _isRendererSet = !_isRendererSet;
            if (!_isRendererSet)
            {
                Dispose();
            }
            else
            {
                AttachGestures();
            }
        }
        else
        if (propertyName == nameof(TouchMode))
        {
            SyncTouchMode(this.TouchMode);
        }
    }



    private void OnLongPressing(object sender, TouchActionEventArgs args)
    {

        //Debug.WriteLine($"[TOUCH] LongPressing!");

        CommandLongPressing?.Execute(CommandLongPressingParameter);

    }


    //-------------------------------------------------------------
    // Reaction
    //-------------------------------------------------------------
    private const string nameReaction = "Reaction";
    public static readonly BindableProperty ReactionProperty = BindableProperty.Create(nameReaction,
        typeof(HotspotReaction), typeof(Hotspot), HotspotReaction.None);
    public HotspotReaction Reaction
    {
        get { return (HotspotReaction)GetValue(ReactionProperty); }
        set { SetValue(ReactionProperty, value); }
    }


    //-------------------------------------------------------------
    // TintColor
    //-------------------------------------------------------------
    private const string nameTintColor = "TintColor";
    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameTintColor, typeof(Color), typeof(Hotspot), Color.Transparent); //, BindingMode.TwoWay
    public Color TintColor
    {
        get { return (Color)GetValue(TintColorProperty); }
        set { SetValue(TintColorProperty, value); }
    }

    //-------------------------------------------------------------
    // TransformView
    //-------------------------------------------------------------
    private const string nameTransformView = "TransformView";
    public static readonly BindableProperty TransformViewProperty = BindableProperty.Create(nameTransformView, typeof(View), typeof(Hotspot), null); //, BindingMode.TwoWay
    public View TransformView
    {
        get { return (View)GetValue(TransformViewProperty); }
        set { SetValue(TransformViewProperty, value); }
    }


    //-------------------------------------------------------------
    // DownOpacity
    //-------------------------------------------------------------
    private const string nameDownOpacity = "DownOpacity";
    public static readonly BindableProperty DownOpacityProperty = BindableProperty.Create(nameDownOpacity, typeof(double), typeof(Hotspot), 0.75); //, BindingMode.TwoWay

    private double _savedOpacity;
    private double _savedScale;

    public double DownOpacity
    {
        get { return (double)GetValue(DownOpacityProperty); }
        set { SetValue(DownOpacityProperty, value); }
    }

    protected Color _savedColor;

    private bool _TouchDown;
    public bool TouchDown
    {
        get { return _TouchDown; }
        set
        {
            if (_TouchDown != value)
            {
                _TouchDown = value;
                OnPropertyChanged();
            }
        }
    }

    private bool animating;
    private DirectionType _panDirection;

    private void OnDown1(object sender, TouchActionEventArgs args)
    {
        ResetPan();

        if (TransformView != null)
        {
            if (!animating)
            {
                animating = true;

                if (_savedOpacity != DownOpacity)
                    _savedOpacity = TransformView.Opacity;

                _savedScale = TransformView.Scale;

                if (_savedColor != TintColor)
                    _savedColor = TransformView.BackgroundColor;

                if (Reaction == HotspotReaction.Tint)
                {
                    if (TintColor != Color.Transparent)
                    {
                        TransformView.BackgroundColor = TintColor;
                    }
                }
                else
                if (Reaction == HotspotReaction.Minify)
                {
                    TransformView.Opacity = DownOpacity;
                    TransformView.Scale = 0.985;
                    if (TintColor != Color.Transparent)
                    {
                        TransformView.BackgroundColor = TintColor;
                    }
                }
                else
                if (Reaction == HotspotReaction.Zoom)
                {
                    TransformView.Scale = 1.1;
                    if (TintColor != Color.Transparent)
                    {
                        TransformView.BackgroundColor = TintColor;
                    }
                }
            }
        }

        TouchDown = true;

        OnDown(args);
    }
}