using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

public enum RangeZone
{
    Unknown,
    Start,
    End
}

public class SliderTrail : SkiaShape
{



    public static readonly BindableProperty XPosProperty = BindableProperty.Create(nameof(XPos), typeof(double), typeof(SliderTrail), 0.0); //, BindingMode.TwoWay
    public double XPos
    {
        get { return (double)GetValue(XPosProperty); }
        set { SetValue(XPosProperty, value); }
    }



    public static readonly BindableProperty XPosEndProperty = BindableProperty.Create(nameof(XPosEnd), typeof(double), typeof(SliderTrail), 100.0); //, BindingMode.TwoWay
    public double XPosEnd
    {
        get { return (double)GetValue(XPosEndProperty); }
        set { SetValue(XPosEndProperty, value); }
    }


    public static readonly BindableProperty ModifyXPosEndProperty = BindableProperty.Create(nameof(ModifyXPosEnd), typeof(double), typeof(SliderTrail), 0.0); //, BindingMode.TwoWay
    public double ModifyXPosEnd
    {
        get { return (double)GetValue(ModifyXPosEndProperty); }
        set { SetValue(ModifyXPosEndProperty, value); }
    }

    public static readonly BindableProperty ModifyXPosStartProperty = BindableProperty.Create(nameof(ModifyXPosStart), typeof(double), typeof(SliderTrail), 0.0); //, BindingMode.TwoWay
    public double ModifyXPosStart
    {
        get { return (double)GetValue(ModifyXPosStartProperty); }
        set { SetValue(ModifyXPosStartProperty, value); }
    }

    public static readonly BindableProperty SideOffsetProperty = BindableProperty.Create(nameof(SideOffset), typeof(double), typeof(SliderTrail), 5.0);
    public double SideOffset
    {
        get { return (double)GetValue(SideOffsetProperty); }
        set { SetValue(SideOffsetProperty, value); }
    }


    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(XPosEnd)
            || propertyName == nameof(ModifyXPosEnd)
            || propertyName == nameof(ModifyXPosStart)
            || propertyName == nameof(XPos)
            || propertyName == nameof(SideOffset))
        {
            this.Margin = new Thickness(XPos + SideOffset + ModifyXPosStart, 0, ModifyXPosEnd, 0);
            this.WidthRequest = XPosEnd - XPos + (ModifyXPosEnd + ModifyXPosStart);
            //Debug.WriteLine($"[DEBUG] {XPos} {XPosEnd} -> {XPosEnd - XPos}");
        }

    }

}


public class SliderThumb : SkiaLayout
{

    public static readonly BindableProperty XCenterProperty = BindableProperty.Create(nameof(XCenter), typeof(double), typeof(SliderThumb), 0.0); //, BindingMode.TwoWay
    public double XCenter
    {
        get { return (double)GetValue(XCenterProperty); }
        set { SetValue(XCenterProperty, value); }
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(TranslationX) ||
            propertyName == nameof(Width))
        {
            var width = this.Width;
            XCenter = TranslationX + width / 2;
        }

    }
}