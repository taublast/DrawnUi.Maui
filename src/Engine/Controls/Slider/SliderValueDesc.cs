using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

public class SliderValueDesc : SkiaShape
{

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SliderValueDesc), string.Empty); //, BindingMode.TwoWay
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }


    public static readonly BindableProperty XCenterProperty = BindableProperty.Create(nameof(XCenter), typeof(double), typeof(SliderValueDesc), 0.0); //, BindingMode.TwoWay
    public double XCenter
    {
        get { return (double)GetValue(XCenterProperty); }
        set { SetValue(XCenterProperty, value); }
    }



    public static readonly BindableProperty XMaxLimitProperty = BindableProperty.Create(nameof(XMaxLimit), typeof(double), typeof(SliderValueDesc), 0.0); //, BindingMode.TwoWay
    public double XMaxLimit
    {
        get { return (double)GetValue(XMaxLimitProperty); }
        set { SetValue(XMaxLimitProperty, value); }
    }


    public static readonly BindableProperty XMinLimitProperty = BindableProperty.Create(nameof(XMinLimit), typeof(double), typeof(SliderValueDesc), 0.0); //, BindingMode.TwoWay
    public double XMinLimit
    {
        get { return (double)GetValue(XMinLimitProperty); }
        set { SetValue(XMinLimitProperty, value); }
    }



    public static readonly BindableProperty RightXProperty = BindableProperty.Create(nameof(RightX), typeof(double), typeof(SliderValueDesc), 0.0); //, BindingMode.TwoWay
    public double RightX
    {
        get { return (double)GetValue(RightXProperty); }
        set { SetValue(RightXProperty, value); }
    }


    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(TranslationX))
        {
            RightX = TranslationX + Width;
        }
        else
        if (propertyName == nameof(XCenter) ||
            propertyName == nameof(XMinLimit) ||
            propertyName == nameof(XMaxLimit) ||
            propertyName == nameof(Width))
        {
            var width = this.Width;
            var maybe = XCenter - width / 2;

            if (maybe < XMinLimit)
            {
                maybe = XMinLimit;
            }
            else
            if (maybe >= XMaxLimit - Width)
            {
                maybe = XMaxLimit - Width;
            }

            this.TranslationX = maybe;
        }
    }

}