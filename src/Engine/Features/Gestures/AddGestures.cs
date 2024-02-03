using DrawnUi.Maui.Draw;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// For fast and lazy gestures handling to attach to dran controls inside the canvas only
/// </summary>
public static class AddGestures
{
    public class GestureListener : SkiaControl, ISkiaGestureListener
    {
        public new string Tag
        {
            get
            {
                return $"AddGestures:{_parent.Tag}";
            }
            set
            {

            }
        }

        public override bool CanDraw
        {
            get
            {
                return _parent.CanDraw;
            }
        }

        private readonly SkiaControl _parent;

        public GestureListener(SkiaControl control)
        {
            _parent = control;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool HitIsInside(float x, float y)
        {
            return _parent.HitIsInside(x, y);
        }

        public override SKRect HitBoxAuto
        {
            get
            {
                return _parent.HitBoxAuto;
            }
        }

        public override SKPoint TranslateInputCoords(SKPoint childOffset, bool accountForCache = true)
        {
            return _parent.TranslateInputCoords(childOffset, accountForCache);
        }

        public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
            SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
        {
            if (!_parent.CanDraw)
                return null;

            if (touchAction == TouchActionResult.Tapped)
            {
                var anim = GetAnimationTapped(_parent);
                if (anim != SkiaTouchAnimation.None)
                {
                    var view = GetTransformView(_parent);
                    if (view == null)
                        view = _parent;

                    if (view is IHasAfterEffects hasEffects)
                    {
                        var thisOffset = TranslateInputCoords(childOffset, false);
                        var pixX = args.Location.X + thisOffset.X;
                        var pixY = args.Location.Y + thisOffset.Y;
                        var x = pixX / RenderingScale;
                        var y = pixY / RenderingScale;

                        var color = GetTouchEffectColor(_parent);
                        if (anim == SkiaTouchAnimation.Ripple)
                        {
                            var ptsInsideControl = hasEffects.GetOffsetInsideControlInPoints(args.Location, childOffset);
                            hasEffects.PlayRippleAnimation(color, ptsInsideControl.X, ptsInsideControl.Y);
                        }
                        else
                        if (anim == SkiaTouchAnimation.Shimmer)
                        {
                            hasEffects.PlayShimmerAnimation(color, 150, 33, 500);
                        }
                    }
                }

                var command = GetCommandTapped(_parent);
                if (command != null)
                {
                    var parameter = GetCommandTappedParameter(_parent);
                    //Trace.WriteLine($"[CommandTapped] ctx - {_parent.BindingContext} - {parameter}");
                    if (parameter == null)
                        parameter = _parent.BindingContext;
                    command?.Execute(parameter);
                    return this;
                }
            }
            else
            if (touchAction == TouchActionResult.LongPressing)
            {
                var command = GetCommandLongPressing(_parent);
                if (command != null)
                {
                    var parameter = GetCommandLongPressingParameter(_parent);
                    if (parameter == null)
                        parameter = _parent.BindingContext;
                    command?.Execute(parameter);
                    return this;
                }
            }

            if (_parent is ISkiaGestureListener listener)
            {
                return listener.OnSkiaGestureEvent(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
            }

            return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
        }

        public void OnFocusChanged(bool focus)
        {

        }
    }

    public static Dictionary<SkiaControl, GestureListener> AttachedListeners = new();

    private static void OnAttachableChanged(BindableObject view, object oldValue, object newValue)
    {
        if (view is SkiaControl control)
        {
            ManageRegistration(control);
        }
    }

    static void ManageListener(SkiaControl control, bool attach)
    {
        if (attach)
        {
            if (!AttachedListeners.ContainsKey(control))
            {
                GestureListener gestureListener = new GestureListener(control);
                AttachedListeners.TryAdd(control, gestureListener);
                //will throw if parent null
                control.GesturesEffect = gestureListener;
                control.Parent.RegisterGestureListener(gestureListener);
            }
        }
        else
        {
            if (AttachedListeners.ContainsKey(control))
            {
                AttachedListeners.Remove(control, out var gestureListener);
                control.GesturesEffect = null;
                if (control.Parent != null)
                {
                    control.Parent.UnregisterGestureListener(gestureListener);
                }
            }
        }
    }

    public static void ManageRegistration(SkiaControl control)
    {
        if (control.Parent != null)
        {
            ManageListener(control, IsActive(control));
        }

        control.ParentChanged -= OnParentChanged;
        control.ParentChanged += OnParentChanged;
    }

    private static void OnParentChanged(object sender, IDrawnBase parent)
    {
        if (sender is SkiaControl control)
        {
            control.ParentChanged -= OnParentChanged;
            if (parent == null)
            {
                ManageListener(control, false);
            }
            else
            {
                ManageListener(control, IsActive(control));
            }
        }
    }
    public static bool IsActive(SkiaControl listener)
    {
        if (listener is SkiaControl control)
        {
            ICommand command = GetCommandTapped(control);
            var effect = GetAnimationTapped(control);
            var needAttach = command != null || effect != SkiaTouchAnimation.None || GetCommandLongPressing(control) != null;
            return needAttach;
        }
        return false;
    }

    public static readonly BindableProperty CommandTappedProperty =
        BindableProperty.CreateAttached(
            "CommandTapped",
            typeof(ICommand),
            typeof(AddGestures),
            null,
            propertyChanged: OnAttachableChanged);

    public static ICommand GetCommandTapped(BindableObject view)
    {
        return (ICommand)view.GetValue(CommandTappedProperty);
    }

    public static void SetCommandTapped(BindableObject view, ICommand value)
    {
        //Trace.WriteLine($"[SetCommandTapped] to {value}");

        view.SetValue(CommandTappedProperty, value);
    }

    public static readonly BindableProperty CommandTappedParameterProperty =
        BindableProperty.CreateAttached(
            "CommandTappedParameter",
            typeof(object),
            typeof(AddGestures),
            null);

    public static object GetCommandTappedParameter(BindableObject view)
    {
        return view.GetValue(CommandTappedParameterProperty);
    }

    public static void SetCommandTappedParameter(BindableObject view, object value)
    {
        //Trace.WriteLine($"[SetCommandTappedParameter] to {value}");

        view.SetValue(CommandTappedParameterProperty, value);
    }

    public static readonly BindableProperty CommandLongPressingProperty =
        BindableProperty.CreateAttached(
            "CommandLongPressing",
            typeof(ICommand),
            typeof(AddGestures),
            null,
            propertyChanged: OnAttachableChanged);

    public static ICommand GetCommandLongPressing(BindableObject view)
    {
        return (ICommand)view.GetValue(CommandLongPressingProperty);
    }

    public static void SetCommandLongPressing(BindableObject view, ICommand value)
    {
        view.SetValue(CommandLongPressingProperty, value);
    }



    public static readonly BindableProperty CommandLongPressingParameterProperty =
        BindableProperty.CreateAttached(
            "CommandLongPressingParameter",
            typeof(object),
            typeof(AddGestures),
            null);

    public static object GetCommandLongPressingParameter(BindableObject view)
    {
        return view.GetValue(CommandLongPressingParameterProperty);
    }

    public static void SetCommandLongPressingParameter(BindableObject view, object value)
    {
        view.SetValue(CommandLongPressingParameterProperty, value);
    }

    public static readonly BindableProperty TransformViewProperty =
        BindableProperty.CreateAttached(
            "TransformView",
            typeof(object),
            typeof(AddGestures),
            null);

    public static object GetTransformView(BindableObject view)
    {
        return view.GetValue(TransformViewProperty);
    }

    public static void SetTransformView(BindableObject view, object value)
    {
        view.SetValue(TransformViewProperty, value);
    }

    public static readonly BindableProperty AnimationTappedProperty =
        BindableProperty.CreateAttached(
            "AnimationTapped",
            typeof(SkiaTouchAnimation),
            typeof(AddGestures),
            null,
            propertyChanged: OnAttachableChanged);

    public static SkiaTouchAnimation GetAnimationTapped(BindableObject view)
    {
        return (SkiaTouchAnimation)view.GetValue(AnimationTappedProperty);
    }

    public static void SetAnimationTapped(BindableObject view, SkiaTouchAnimation value)
    {
        view.SetValue(AnimationTappedProperty, value);
    }

    public static readonly BindableProperty TouchEffectColorProperty =
        BindableProperty.CreateAttached(
            "TouchEffectColor",
            typeof(Color),
            typeof(AddGestures),
            Colors.White);

    public static Color GetTouchEffectColor(BindableObject view)
    {
        return (Color)view.GetValue(TouchEffectColorProperty);
    }

    public static void SetTouchEffectColor(BindableObject view, Color value)
    {
        view.SetValue(TouchEffectColorProperty, value);
    }




}