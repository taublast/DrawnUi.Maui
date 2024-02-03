namespace DrawnUi.Maui.Draw;

public class SkiaShadow : BindableObject, ICloneable
{

    public IDrawnBase Parent { get; set; }

    public object Clone()
    {
        return new SkiaShadow { Blur = Blur, Color = Color, X = X, Y = Y, Opacity = Opacity };
    }
    private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaShadow shadow)
        {
            shadow.Parent?.Update();
        }
    }

    //-------------------------------------------------------------
    // Opacity 
    //-------------------------------------------------------------
    private const string nameOpacity = "Opacity";
    public static readonly BindableProperty OpacityProperty = BindableProperty.Create(nameOpacity,
        typeof(double),
        typeof(SkiaShadow),
        0.5,
        propertyChanged: RedrawCanvas);


    public double Opacity
    {
        get { return (double)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    //-------------------------------------------------------------
    // Color
    //-------------------------------------------------------------
    private const string nameColor = "Color";
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameColor, typeof(Color), typeof(SkiaShadow),
        Colors.Transparent,
        propertyChanged: RedrawCanvas);
    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    //-------------------------------------------------------------
    // X
    //-------------------------------------------------------------
    private const string nameX = "X";
    public static readonly BindableProperty XProperty = BindableProperty.Create(nameX, typeof(double), typeof(SkiaShadow),
        2.0,
        propertyChanged: RedrawCanvas);
    public double X
    {
        get { return (double)GetValue(XProperty); }
        set { SetValue(XProperty, value); }
    }

    //-------------------------------------------------------------
    // Y
    //-------------------------------------------------------------
    private const string nameY = "Y";
    public static readonly BindableProperty YProperty = BindableProperty.Create(nameY, typeof(double), typeof(SkiaShadow),
        2.0,
        propertyChanged: RedrawCanvas);
    public double Y
    {
        get { return (double)GetValue(YProperty); }
        set { SetValue(YProperty, value); }
    }

    //-------------------------------------------------------------
    // Blur
    //-------------------------------------------------------------
    private const string nameBlur = "Blur";
    public static readonly BindableProperty BlurProperty = BindableProperty.Create(nameBlur, typeof(double), typeof(SkiaShadow),
        5.0,
        propertyChanged: RedrawCanvas);
    public double Blur
    {
        get { return (double)GetValue(BlurProperty); }
        set { SetValue(BlurProperty, value); }
    }


}