using DrawnUi.Maui.Controls;
using DrawnUi.Maui.Draw;
using HarfBuzzSharp;
using Microsoft.Maui.Devices.Sensors;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DrawnUi.Maui.Draw;

public class SkiaSlider : SkiaLayout
{
    #region GESTURES

    protected bool IsUserPanning { get; set; }

    /// <summary>
    /// enlarge hotspot by pts
    /// </summary>
    public double moreHotspotSize = 10.0;

    protected double lastTouchX;

    /// <summary>
    /// track touched area type
    /// </summary>
    protected RangeZone touchArea;

    protected Vector2 _panningStartOffsetPts;

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        if (Trail == null)
        {
            Trail = FindViewByTag("Trail");
        }
    }

    public SkiaControl Trail { get; set; }

    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
        SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {
        bool passedToChildren = false;

        ISkiaGestureListener PassToChildren()
        {
            passedToChildren = true;

            return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
        }

        ISkiaGestureListener consumed = null;

        //pass Released always to children first
        if (touchAction == TouchActionResult.Up
            || !IsUserPanning || !RespondsToGestures)
        {
            if (args.NumberOfTouches < 2)
                TouchBusy = false;

            consumed = PassToChildren();
            if (consumed != null && touchAction != TouchActionResult.Up)
            {
                return consumed;
            }
        }

        if (!RespondsToGestures)
            return consumed;

        void ResetPan()
        {
            IsUserPanning = false;
            _panningStartOffsetPts = new(args.Location.X, args.Location.Y);
        }

        switch (touchAction)
        {
        case TouchActionResult.Down:

        bool onTrail = true;
        if (Trail != null)
        {
            var point = TranslateInputOffsetToPixels(args.Location, childOffset);
            onTrail = Trail.HitIsInside(point.X, point.Y);
        }

        if (args.NumberOfTouches < 2)
        {
            ResetPan();
            if (onTrail)
                IsPressed = true;
        }

        var thisOffset = TranslateInputCoords(childOffset);
        var x = args.Location.X + thisOffset.X - DrawingRect.Left; //inside this control
        var locationX = x / RenderingScale; //from pix to pts

        var y = args.Location.Y + thisOffset.Y - DrawingRect.Top; //inside this control
        var locationY = y / RenderingScale; //from pix to pts

        if (EnableRange && locationX >= StartThumbX - moreHotspotSize &&
            locationX <= StartThumbX + SliderHeight + moreHotspotSize)
        {
            touchArea = RangeZone.Start;
        }
        else
        if (locationX >= EndThumbX - moreHotspotSize &&
            locationX <= EndThumbX + SliderHeight + moreHotspotSize)
        {
            touchArea = RangeZone.End;
        }
        else
        {
            touchArea = RangeZone.Unknown;
        }

        if (touchArea == RangeZone.Unknown && ClickOnTrailEnabled)
        {
            //clicked on trail maybe


            if (onTrail)
            {
                if (EnableRange)
                {
                    var half = (Width + AvailableWidthAdjustment) / 2.0;

                    if (locationX > half)
                    {
                        MoveEndThumbHere(locationX);
                    }
                    else
                    if (locationX <= half)
                    {
                        MoveStartThumbHere(locationX);
                    }
                }
                else
                {
                    MoveEndThumbHere(locationX);
                }
            }

        }

        if (touchArea == RangeZone.Start)
        {
            consumed = this;
            lastTouchX = StartThumbX;
        }
        else
        if (touchArea == RangeZone.End)
        {
            consumed = this;
            lastTouchX = EndThumbX;
        }

        break;

        case TouchActionResult.Panning when args.NumberOfTouches == 1:

        //filter correct direction so we could scroll below the control in another direction:
        if (!IsUserPanning && IgnoreWrongDirection)
        {
            //first panning gesture..
            var panDirection = GetDirectionType(_panningStartOffsetPts, new Vector2(_panningStartOffsetPts.X + args.Distance.Total.X, _panningStartOffsetPts.Y + args.Distance.Total.Y), 0.8f);

            if (Orientation == OrientationType.Vertical && panDirection != DirectionType.Vertical)
            {
                return null;
            }
            if (Orientation == OrientationType.Horizontal && panDirection != DirectionType.Horizontal)
            {
                return null;
            }
        }


        IsUserPanning = true;


        //synch this 
        if (touchArea == RangeZone.Start)
            lastTouchX = StartThumbX;
        else
        if (touchArea == RangeZone.End)
            lastTouchX = EndThumbX;


        if (touchArea != RangeZone.Unknown)
        {
            TouchBusy = true;

            if (touchArea == RangeZone.Start)
            {
                var maybe = lastTouchX + args.Distance.Delta.X / RenderingScale;//args.TotalDistance.X;
                SetStartOffsetClamped(maybe);
            }
            else
            if (touchArea == RangeZone.End)
            {
                var maybe = lastTouchX + args.Distance.Delta.X / RenderingScale;
                SetEndOffsetClamped(maybe);
            }

            RecalculateValues();
        }

        consumed = this;
        break;

        case TouchActionResult.Up when args.NumberOfTouches < 2:
        IsUserPanning = false;
        IsPressed = false;
        break;

        }

        if (consumed != null || IsUserPanning)// || args.NumberOfTouches > 1)
        {
            return consumed ?? this;
        }

        if (!passedToChildren)
            return PassToChildren();

        return null;
    }


    private bool TouchBusy;

    #endregion

    #region PROPERTIES

    public static readonly BindableProperty ClickOnTrailEnabledProperty = BindableProperty.Create(nameof(ClickOnTrailEnabled), typeof(bool), typeof(SkiaSlider), true);
    public bool ClickOnTrailEnabled
    {
        get { return (bool)GetValue(ClickOnTrailEnabledProperty); }
        set { SetValue(ClickOnTrailEnabledProperty, value); }
    }

    public static readonly BindableProperty EnableRangeProperty = BindableProperty.Create(nameof(EnableRange), typeof(bool), typeof(SkiaSlider), false);
    public bool EnableRange
    {
        get { return (bool)GetValue(EnableRangeProperty); }
        set { SetValue(EnableRangeProperty, value); }
    }

    public static readonly BindableProperty RespondsToGesturesProperty = BindableProperty.Create(
        nameof(RespondsToGestures),
        typeof(bool),
        typeof(SkiaSlider),
        true);

    /// <summary>
    /// Can be open/closed by gestures along with code-behind, default is true
    /// </summary>
    public bool RespondsToGestures
    {
        get { return (bool)GetValue(RespondsToGesturesProperty); }
        set { SetValue(RespondsToGesturesProperty, value); }
    }

    public static readonly BindableProperty SliderHeightProperty = BindableProperty.Create(nameof(SliderHeight), typeof(double), typeof(SkiaSlider), 22.0); //, BindingMode.TwoWay
    public double SliderHeight
    {
        get { return (double)GetValue(SliderHeightProperty); }
        set { SetValue(SliderHeightProperty, value); }
    }

    public static readonly BindableProperty MinProperty = BindableProperty.Create(nameof(Min), typeof(double), typeof(SkiaSlider), 0.0); //, BindingMode.TwoWay
    public double Min
    {
        get { return (double)GetValue(MinProperty); }
        set { SetValue(MinProperty, value); }
    }

    public static readonly BindableProperty MaxProperty = BindableProperty.Create(nameof(Max), typeof(double), typeof(SkiaSlider), 100.0);
    public double Max
    {
        get { return (double)GetValue(MaxProperty); }
        set { SetValue(MaxProperty, value); }
    }

    public static readonly BindableProperty StartProperty =
        BindableProperty.Create(nameof(Start), typeof(double), typeof(SkiaSlider), 0.0, BindingMode.TwoWay,
            coerceValue: (c, o) =>
        {
            if (c is SkiaSlider control)
            {
                double adjustedValue = control.AdjustToStepValue((double)o, control.Min, control.Step);
                return adjustedValue;
            }
            return o;
        });
    public double Start
    {
        get { return (double)GetValue(StartProperty); }
        set { SetValue(StartProperty, value); }
    }

    public static readonly BindableProperty EndProperty = BindableProperty.Create(nameof(End), typeof(double), typeof(SkiaSlider), 100.0,
        BindingMode.TwoWay,
        coerceValue: (c, o) =>
        {
            if (c is SkiaSlider control)
            {
                double adjustedValue = control.AdjustToStepValue((double)o, control.Min, control.Step);
                return adjustedValue;
            }
            return o;
        });

    public double End
    {
        get { return (double)GetValue(EndProperty); }
        set { SetValue(EndProperty, value); }
    }

    public static readonly BindableProperty StartThumbXProperty = BindableProperty.Create(nameof(StartThumbX), typeof(double), typeof(SkiaSlider), 0.0);
    public double StartThumbX
    {
        get { return (double)GetValue(StartThumbXProperty); }
        set { SetValue(StartThumbXProperty, value); }
    }
    public static readonly BindableProperty EndThumbXProperty = BindableProperty.Create(nameof(EndThumbX), typeof(double), typeof(SkiaSlider), 0.0);
    public double EndThumbX
    {
        get { return (double)GetValue(EndThumbXProperty); }
        set { SetValue(EndThumbXProperty, value); }
    }

    public static readonly BindableProperty StepProperty = BindableProperty.Create(nameof(Step), typeof(double), typeof(SkiaSlider), 1.0); //, BindingMode.TwoWay
    public double Step
    {
        get { return (double)GetValue(StepProperty); }
        set { SetValue(StepProperty, value); }
    }

    public static readonly BindableProperty RangeMinProperty = BindableProperty.Create(nameof(RangeMin), typeof(double), typeof(SkiaSlider), 0.0); //, BindingMode.TwoWay
    public double RangeMin
    {
        get { return (double)GetValue(RangeMinProperty); }
        set { SetValue(RangeMinProperty, value); }
    }

    public static readonly BindableProperty ValueStringFormatProperty = BindableProperty.Create(nameof(ValueStringFormat), typeof(string), typeof(SkiaSlider), "### ### ##0.##"); //, BindingMode.TwoWay

    public string ValueStringFormat
    {
        get { return (string)GetValue(ValueStringFormatProperty); }
        set { SetValue(ValueStringFormatProperty, value); }
    }

    public static readonly BindableProperty MinMaxStringFormatProperty = BindableProperty.Create(nameof(MinMaxStringFormat), typeof(string), typeof(SkiaSlider), "### ### ##0.##"); //, BindingMode.TwoWay

    public string MinMaxStringFormat
    {
        get { return (string)GetValue(MinMaxStringFormatProperty); }
        set { SetValue(MinMaxStringFormatProperty, value); }
    }

    public static readonly BindableProperty AvailableWidthAdjustmentProperty = BindableProperty.Create(nameof(AvailableWidthAdjustment), typeof(double), typeof(SkiaSlider), 0.0);
    public double AvailableWidthAdjustment
    {
        get { return (double)GetValue(AvailableWidthAdjustmentProperty); }
        set { SetValue(AvailableWidthAdjustmentProperty, value); }
    }

    public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(OrientationType), typeof(SkiaSlider),
        OrientationType.Horizontal,
        propertyChanged: NeedDraw);
    /// <summary>
    /// <summary>Gets or sets the orientation. This is a bindable property.</summary>
    /// </summary>
    public OrientationType Orientation
    {
        get { return (OrientationType)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly BindableProperty IgnoreWrongDirectionProperty = BindableProperty.Create(
        nameof(IgnoreWrongDirection),
        typeof(bool),
        typeof(SkiaSlider),
        true);

    /// <summary>
    /// Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity
    /// </summary>
    public bool IgnoreWrongDirection
    {
        get { return (bool)GetValue(IgnoreWrongDirectionProperty); }
        set { SetValue(IgnoreWrongDirectionProperty, value); }
    }

    #endregion

    #region ENGINE

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.IsEither(nameof(Min),
                nameof(MinMaxStringFormat), nameof(Max)))
        {
            var mask = "{0:" + MinMaxStringFormat + "}";
            MinDesc = string.Format(mask, Min).Trim();
            MaxDesc = string.Format(mask, Max).Trim();
        }

        if (propertyName.IsEither(nameof(Width),
            nameof(Min), nameof(Max), nameof(StartThumbX),
            nameof(EndThumbX), nameof(Step), nameof(Start),
            nameof(End), nameof(AvailableWidthAdjustment)))
        {
            if (lockInternal)
                return;

            if (Width > -1)
            {
                Start = Math.Clamp(Start, Min, Max);
                End = Math.Clamp(End, Min, Max);

                if (EnableRange)
                {
                    StepValue = (Max - Min) / (Width + AvailableWidthAdjustment - SliderHeight);
                }
                else
                {
                    StepValue = (Max - Min) / (Width + AvailableWidthAdjustment * 2 - SliderHeight);
                }

                if (!TouchBusy)
                {
                    var mask = "{0:" + ValueStringFormat + "}";
                    if (EnableRange)
                    {
                        SetStartOffsetClamped((Start - Min) / StepValue - AvailableWidthAdjustment);
                        StartDesc = string.Format(mask, Start).Trim();
                    }
                    SetEndOffsetClamped((End - Min) / StepValue);
                    EndDesc = string.Format(mask, End).Trim();
                }
            }
        }

    }




    protected double AdjustToStepValue(double value, double minValue, double stepValue)
    {
        if (stepValue <= 0) return value;

        double relativeValue = value - minValue;
        double adjustedStep = Math.Round(relativeValue / stepValue) * stepValue;
        return adjustedStep + minValue;
    }

    protected virtual void ConvertOffsetsToValues()
    {
        if (EnableRange)
        {
            Start = StepValue * (this.StartThumbX + AvailableWidthAdjustment) + Min;
            End = StepValue * (this.EndThumbX - AvailableWidthAdjustment) + Min;
        }
        else
        {
            End = StepValue * (this.EndThumbX + AvailableWidthAdjustment) + Min;
        }
    }

    protected virtual void RecalculateValues()
    {
        var mask = "{0:" + ValueStringFormat + "}";
        ConvertOffsetsToValues();
        if (EnableRange)
        {
            StartDesc = string.Format(mask, Start).Trim();
        }
        EndDesc = string.Format(mask, End).Trim();
    }

    protected void MoveEndThumbHere(double x)
    {
        if (!ClickOnTrailEnabled)
            return;

        touchArea = RangeZone.End;

        lockInternal = true;
        SetEndOffsetClamped(x);
        lockInternal = false;

        RecalculateValues();
    }

    protected void MoveStartThumbHere(double x)
    {
        if (!ClickOnTrailEnabled)
            return;

        touchArea = RangeZone.Start;

        lockInternal = true;

        SetStartOffsetClamped(x);

        lockInternal = false;

        RecalculateValues();
    }

    void SetStartOffsetClamped(double maybe)
    {
        if (maybe < -AvailableWidthAdjustment)
        {
            StartThumbX = -AvailableWidthAdjustment;
        }
        else
        if (EnableRange && maybe > (EndThumbX - RangeMin / StepValue))
        {
            StartThumbX = EndThumbX - RangeMin / StepValue;
        }
        else
        {
            StartThumbX = maybe;
        }
    }

    void SetEndOffsetClamped(double maybe)
    {
        if (maybe < -AvailableWidthAdjustment)
        {
            EndThumbX = -AvailableWidthAdjustment;
        }
        else
        if (maybe > (Width + AvailableWidthAdjustment) - SliderHeight)
        {
            EndThumbX = (Width + AvailableWidthAdjustment) - SliderHeight;
        }
        else
        if (EnableRange && maybe < (StartThumbX + RangeMin / StepValue))
        {
            EndThumbX = StartThumbX + RangeMin / StepValue;
        }
        else
        {
            EndThumbX = maybe;
        }
    }

    private bool lockInternal;


    private string _StartDesc;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string StartDesc
    {
        get { return _StartDesc; }
        set
        {
            if (_StartDesc != value)
            {
                _StartDesc = value;
                OnPropertyChanged();
            }
        }
    }

    private string _EndDesc;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string EndDesc
    {
        get { return _EndDesc; }
        set
        {
            if (_EndDesc != value)
            {
                _EndDesc = value;
                OnPropertyChanged();
            }
        }
    }

    private string _MinDesc;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string MinDesc
    {
        get { return _MinDesc; }
        set
        {
            if (_MinDesc != value)
            {
                _MinDesc = value;
                OnPropertyChanged();
            }
        }
    }

    private string _MaxDesc;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string MaxDesc
    {
        get { return _MaxDesc; }
        set
        {
            if (_MaxDesc != value)
            {
                _MaxDesc = value;
                OnPropertyChanged();
            }
        }
    }

    private double _StepValue;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public double StepValue
    {
        get
        {
            return _StepValue;
        }
        set
        {
            if (_StepValue != value)
            {
                _StepValue = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _IsPressed;
    public bool IsPressed
    {
        get
        {
            return _IsPressed;
        }
        set
        {
            if (_IsPressed != value)
            {
                _IsPressed = value;
                OnPropertyChanged();
            }
        }
    }


    #endregion
}