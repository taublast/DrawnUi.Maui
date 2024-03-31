using AppoMobi.Maui.Gestures;
using SkiaSharp.Views.Maui;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

//reusing code from https://github.com/nor0x/Maui.ColorPicker

namespace Sandbox.Views.Controls;

public partial class SkiaColorPicker : SkiaLayout
{
    public class GradientPanel : SkiaControl
    {
        //set by parent

        public IList<SKColor> PrimaryColors { get; set; }

        public IList<SKColor> SecondaryColors { get; set; }

        public ColorFlowDirection ColorFlowDirection { get; set; }

        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            base.Paint(ctx, destination, scale, arguments);

            if (PrimaryColors != null && SecondaryColors != null)
            {
                //draw gradients

                // Draw primary gradient color spectrum (Rainbow)
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    // create the gradient shader between base Colors
                    using (var shader = SKShader.CreateLinearGradient(
                        new SKPoint(destination.Left, destination.Top),
                        ColorFlowDirection == ColorFlowDirection.Vertical ?
                            new SKPoint(destination.Right, destination.Top)
                            : new SKPoint(destination.Left, destination.Bottom),
                        PrimaryColors.ToArray(),
                        null,
                        SKShaderTileMode.Clamp))
                    {
                        paint.Shader = shader;
                        ctx.Canvas.DrawRect(destination, paint);
                    }
                }

                // Draw secondary gradient color spectrum (Lighness whatever..)
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;

                    // create the gradient shader between secondary colors
                    using (var shader = SKShader.CreateLinearGradient(
                        new SKPoint(destination.Left, destination.Top),
                        ColorFlowDirection == ColorFlowDirection.Vertical ?
                            new SKPoint(destination.Left, destination.Bottom)
                            : new SKPoint(destination.Right, destination.Top),
                        SecondaryColors.ToArray(),
                        null,
                        SKShaderTileMode.Clamp))
                    {
                        paint.Shader = shader;
                        ctx.Canvas.DrawRect(destination, paint);
                    }
                }
            }


        }


    }

    protected GradientPanel Panel { get; set; }

    public void SetupGradientPanel()
    {
        if (Panel == null)
        {
            Panel = new()
            {
                ZIndex = -1,
                UseCache = SkiaCacheType.Image,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            if (Views.All(x => x != Panel))
            {
                AddSubView(Panel);
            }
        }

        if (!_colorsSet)
        {
            _colorsSet = true;
            Panel.ColorFlowDirection = this.ColorFlowDirection;
            Panel.PrimaryColors = GetPrimaryLayerColors();
            Panel.SecondaryColors = GetSecondaryLayerColors(ColorSpectrumStyle);
        }
    }

    protected virtual void UpdateColors()
    {
        _colorsSet = false;
        SetupGradientPanel();
    }

    protected virtual void UpdateIndicator()
    {
        var x = ((float)DrawingRect.Left + DrawingRect.Width * (float)PointerRingPositionXOffsetRatio);
        var y = ((float)DrawingRect.Top + DrawingRect.Height * (float)PointerRingPositionYOffsetRatio);
        _lastTouchPoint = new SKPoint(x, y);

        SetupIndicator();
    }

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        SetupGradientPanel();
        SetupIndicator();
    }

    protected SkiaShape Indicator { get; set; }

    public void SetupIndicator()
    {
        if (Indicator == null)
        {
            Indicator = new()
            {
                UseCache = SkiaCacheType.Image,
                StrokeColor = Microsoft.Maui.Graphics.Colors.White,
                StrokeWidth = 4,
                Type = ShapeType.Circle,
                LockRatio = 1,
                HeightRequest = PointerRingDiameterUnits
            };
            if (Views.All(x => x != Indicator))
            {
                AddSubView(Indicator);
            }
        }

        Indicator.TranslationX = (DrawingRect.Width * PointerRingPositionXOffsetRatio) / RenderingScale - PointerRingDiameterUnits / 2f;
        Indicator.TranslationY = (DrawingRect.Height * PointerRingPositionYOffsetRatio) / RenderingScale - PointerRingDiameterUnits / 2f;
    }

    protected override int RenderViewsList(IEnumerable<SkiaControl> skiaControls, SkiaDrawingContext context, SKRect destination, float scale,
        bool debug = false)
    {
        var count = 0;
        foreach (var child in skiaControls)
        {
            if (child != null)
            {
                if (child == Indicator)
                {
                    //just get the pixels color under before drawing it
                    SKColor touchPointColor = SKColor.Empty;

                    context.Superview.CanvasView.Surface.Canvas.Flush();
                    var snapshot = context.Superview.CanvasView.Surface.Snapshot(new((int)destination.Left, (int)destination.Top, (int)destination.Right, (int)destination.Bottom));
                    var touchX = (int)_lastTouchPoint.X;
                    var touchY = (int)_lastTouchPoint.Y;
                    using (var bitmap = SKBitmap.FromImage(snapshot))
                    {
                        // Ensure x and y are within the bounds of the image
                        if (touchX >= 0 && touchY >= 0 && touchX < bitmap.Width && touchY < bitmap.Height)
                        {
                            touchPointColor = bitmap.GetPixel(touchX, touchY);
                            PickedColor = touchPointColor.ToMauiColor();
                        }
                    }

                }
                child.OnBeforeDraw();
                if (child.CanDraw) //still visible 
                {
                    count++;
                    child.Render(context, destination, scale);
                }
            }
        }

        return count;
    }



    #region GESTURES

    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
        SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {

        if (touchAction == TouchActionResult.Down || touchAction == TouchActionResult.Panning)
        {
            var point = TranslateInputOffsetToPixels(args.Location, childOffset);

            //e.Location.X / canvasSize.Width;
            PointerRingPositionXOffsetRatio = (point.X - DrawingRect.Left) / DrawingRect.Width;
            //e.Location.Y / canvasSize.Height;
            PointerRingPositionYOffsetRatio = (point.Y - DrawingRect.Top) / DrawingRect.Height;

            return this;
        }

        return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
    }

    private SKPoint _lastTouchPoint = new SKPoint();
    private bool _checkPointerInitPositionDone = false;



    #endregion

    /// <summary>
    /// Occurs when the Picked Color changes
    /// </summary>
    public event EventHandler<Color>? PickedColorChanged;

    public static readonly BindableProperty PickedColorProperty
        = BindableProperty.Create(
            nameof(PickedColor),
            typeof(Color),
            typeof(SkiaColorPicker), Microsoft.Maui.Graphics.Colors.Transparent,
            propertyChanged: (bindable, value, newValue) =>
            {
                if (bindable is SkiaColorPicker control)
                {
                    control.PickedColorChanged?.Invoke(control, (Color)newValue);
                }
            });

    /// <summary>
    /// Get the current Picked Color
    /// </summary>
    public Color PickedColor
    {
        get { return (Color)GetValue(PickedColorProperty); }
        private set { SetValue(PickedColorProperty, value); }
    }

    public static readonly BindableProperty ColorSpectrumStyleProperty
     = BindableProperty.Create(
         nameof(ColorSpectrumStyle),
         typeof(ColorSpectrumStyle),
         typeof(SkiaColorPicker),
         ColorSpectrumStyle.TintToHueToShadeStyle,
         BindingMode.Default, null,
         propertyChanged: (bindable, value, newValue) =>
         {
             if (newValue != null)
                 ((SkiaColorPicker)bindable).Update();
             else
                 ((SkiaColorPicker)bindable).ColorSpectrumStyle = default;
         });

    /// <summary>
    /// Set the Color Spectrum Gradient Style
    /// </summary>
    public ColorSpectrumStyle ColorSpectrumStyle
    {
        get { return (ColorSpectrumStyle)GetValue(ColorSpectrumStyleProperty); }
        set { SetValue(ColorSpectrumStyleProperty, value); }
    }

    #region COLORS

    static List<Color> defaultColors = new()
    {
        new Color(255, 0, 0), // Red
        new Color(255, 255, 0), // Yellow
        new Color(0, 255, 0), // Green (Lime)
        new Color(0, 255, 255), // Aqua
        new Color(0, 0, 255), // Blue
        new Color(255, 0, 255), // Fuchsia
        new Color(255, 0, 0), // Red
    };

    public static readonly BindableProperty ColorsProperty = BindableProperty.Create(
        nameof(Colors),
        typeof(IList<Color>),
        typeof(SkiaColorPicker),
        defaultValueCreator: (instance) =>
        {
            var created = new ObservableCollection<Color>(defaultColors);
            //ColorsPropertyChanged(instance, null, created);
            return created;
        },
        validateValue: (bo, v) => v is IList<Color>,
        propertyChanged: ColorsPropertyChanged,
        coerceValue: CoerceColors);


    public IList<Color> Colors
    {
        get => (IList<Color>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    private static object CoerceColors(BindableObject bindable, object value)
    {
        if (!(value is ReadOnlyCollection<Color> readonlyCollection))
        {
            return value;
        }

        return new ReadOnlyCollection<Color>(
            readonlyCollection.Select(s => s)
                .ToList());
    }

    private static void ColorsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {

        if (bindable is SkiaColorPicker control)
        {
            if (oldvalue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= control.OnPropertyColorsCollectionChanged;
            }
            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += control.OnPropertyColorsCollectionChanged;
            }

            control.UpdateColors();
        }
    }

    private void OnPropertyColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.Update();
    }

    #endregion



    public static readonly BindableProperty ColorFlowDirectionProperty
        = BindableProperty.Create(
            nameof(ColorFlowDirection),
            typeof(ColorFlowDirection),
            typeof(SkiaColorPicker),
            ColorFlowDirection.Vertical,
            BindingMode.Default, null,
            propertyChanged: (bindable, value, newValue) =>
            {
                if (newValue != null)
                    ((SkiaColorPicker)bindable).UpdateColors();
                else
                    ((SkiaColorPicker)bindable).ColorFlowDirection = default;
            });

    /// <summary>
    /// Sets the Color List flow Direction
    /// Horizontal or Verical
    /// </summary>
    public ColorFlowDirection ColorFlowDirection
    {
        get { return (ColorFlowDirection)GetValue(ColorFlowDirectionProperty); }
        set { SetValue(ColorFlowDirectionProperty, value); }
    }


    public static readonly BindableProperty PointerRingDiameterUnitsProperty
        = BindableProperty.Create(
            nameof(PointerRingDiameterUnits),
            typeof(double),
            typeof(SkiaColorPicker),
            16.0,
            BindingMode.Default,
            validateValue: (bindable, value) =>
            {
                return (((double)value > -1) && ((double)value <= 1));
            },
            propertyChanged: (bindable, value, newValue) =>
            {
                if (newValue != null)
                    ((SkiaColorPicker)bindable).UpdateIndicator();
                else
                    ((SkiaColorPicker)bindable).PointerRingDiameterUnits = default;
            });

    /// <summary>
    /// Sets the Picker Pointer Ring Diameter
    /// In device-independent Points
    /// </summary>
    public double PointerRingDiameterUnits
    {
        get { return (double)GetValue(PointerRingDiameterUnitsProperty); }
        set { SetValue(PointerRingDiameterUnitsProperty, value); }
    }

    public static readonly BindableProperty PointerRingBorderUnitsProperty
        = BindableProperty.Create(
            nameof(PointerRingBorderUnits),
            typeof(double),
            typeof(SkiaColorPicker),
            0.3,
            BindingMode.Default,
            validateValue: (bindable, value) =>
            {
                return (((double)value > -1) && ((double)value <= 1));
            },
            propertyChanged: (bindable, value, newValue) =>
            {
                if (newValue != null)
                    ((SkiaColorPicker)bindable).Update();
                else
                    ((SkiaColorPicker)bindable).PointerRingBorderUnits = default;
            });

    /// <summary>
    /// Sets the Picker Pointer Ring Border Size
    /// Value must be between 0-1
    /// Calculated against pixel size of Picker Pointer
    /// </summary>
    public double PointerRingBorderUnits
    {
        get { return (double)GetValue(PointerRingBorderUnitsProperty); }
        set { SetValue(PointerRingBorderUnitsProperty, value); }
    }

    public static readonly BindableProperty PointerRingPositionXOffsetRatioProperty
        = BindableProperty.Create(
            nameof(PointerRingPositionXOffsetRatio),
            typeof(double),
            typeof(SkiaColorPicker),
            0.5,
            BindingMode.OneTime,
            coerceValue: (b, v) =>
            {
                var value = (double)v;
                return Math.Clamp(value, 0, 1);
            },
            propertyChanged: (bindable, value, newValue) =>
            {
                if (newValue != null)
                {
                    ((SkiaColorPicker)bindable).UpdateIndicator();
                }
                else
                    ((SkiaColorPicker)bindable).ColorFlowDirection = default;
            });

    /// <summary>
    /// Sets the Picker Pointer X position
    /// Value must be between 0-1
    /// Calculated against the View Canvas Width value
    /// </summary>
    public double PointerRingPositionXOffsetRatio
    {
        get { return (double)GetValue(PointerRingPositionXOffsetRatioProperty); }
        set { SetValue(PointerRingPositionXOffsetRatioProperty, value); }
    }

    public static readonly BindableProperty PointerRingPositionYOffsetRatioProperty
        = BindableProperty.Create(
            nameof(PointerRingPositionYOffsetRatio),
            typeof(double),
            typeof(SkiaColorPicker),
            0.5,
            BindingMode.OneTime,
            coerceValue: (b, v) =>
            {
                var value = (double)v;
                return Math.Clamp(value, 0, 1);
            },
            propertyChanged: (bindable, value, newValue) =>
            {
                if (newValue != null)
                {
                    ((SkiaColorPicker)bindable).UpdateIndicator();
                }
                else
                    ((SkiaColorPicker)bindable).ColorFlowDirection = default;
            });

    private bool _colorsSet;

    /// <summary>
    /// Sets the Picker Pointer Y position
    /// Value must be between 0-1
    /// Calculated against the View Canvas Width value
    /// </summary>
    public double PointerRingPositionYOffsetRatio
    {
        get { return (double)GetValue(PointerRingPositionYOffsetRatioProperty); }
        set { SetValue(PointerRingPositionYOffsetRatioProperty, value); }
    }

    protected virtual List<SKColor> GetPrimaryLayerColors()
    {
        var colors = new System.Collections.Generic.List<SKColor>();
        foreach (var color in Colors)
            colors.Add(color.ToSKColor());
        return colors;
    }

    protected virtual List<SKColor> GetSecondaryLayerColors(ColorSpectrumStyle colorSpectrumStyle)
    {
        switch (colorSpectrumStyle)
        {
        case ColorSpectrumStyle.HueOnlyStyle:
        return new()
        {
                SKColors.Transparent
        };
        case ColorSpectrumStyle.HueToShadeStyle:
        return new()
        {
                SKColors.Transparent,
                SKColors.Black
        };
        case ColorSpectrumStyle.ShadeToHueStyle:
        return new()
        {
                SKColors.Black,
                SKColors.Transparent
        };
        case ColorSpectrumStyle.HueToTintStyle:
        return new()
        {
                SKColors.Transparent,
                SKColors.White
        };
        case ColorSpectrumStyle.TintToHueStyle:
        return new()
        {
                SKColors.White,
                SKColors.Transparent
        };
        case ColorSpectrumStyle.TintToHueToShadeStyle:
        return new()
        {
                SKColors.White,
                SKColors.Transparent,
                SKColors.Black
        };
        case ColorSpectrumStyle.ShadeToHueToTintStyle:
        return new()
        {
                SKColors.Black,
                SKColors.Transparent,
                SKColors.White
        };
        default:
        return new()
        {
                SKColors.Transparent,
                SKColors.Black
        };
        }
    }


}

public enum ColorSpectrumStyle
{
    HueOnlyStyle,
    HueToShadeStyle,
    ShadeToHueStyle,
    HueToTintStyle,
    TintToHueStyle,
    TintToHueToShadeStyle,
    ShadeToHueToTintStyle
}

public enum ColorFlowDirection
{
    Horizontal,
    Vertical
}
