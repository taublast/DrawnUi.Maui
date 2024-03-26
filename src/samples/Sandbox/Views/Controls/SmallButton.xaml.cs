using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

public partial class SmallButton : SkiaButton
{
    public SmallButton()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Clip effects with rounded rect of the frame inside
    /// </summary>
    /// <returns></returns>
    public override SKPath CreateClip(object arguments, bool usePosition)
    {
        if (MainFrame != null)
        {
            return MainFrame.CreateClip(arguments, usePosition);
        }

        return base.CreateClip(arguments, usePosition);
    }

    async void AnimatePress(SkiaControl icon)
    {
        await icon.ScaleToAsync(0.9, 0.9, 50, Easing.CubicOut);
        await icon.ScaleToAsync(1.0, 1.0, 50, Easing.SpringOut);
    }

    public override bool OnDown(TouchActionEventArgs args, SKPoint childOffset, SKPoint childOffsetDirect,
        ISkiaGestureListener wasConsumed)
    {
        this.ScaleToAsync(0.98, 0.95, 16, Easing.CubicOut);

        return base.OnDown(args, childOffset, childOffsetDirect, wasConsumed);
    }


    public override void OnUp()
    {
        this.ScaleToAsync(1.0, 1.0, 32, Easing.SpringOut);

        base.OnUp();
    }



    SkiaLabel MainLabel;
    SkiaShape MainFrame;

    protected override void OnMeasured()
    {
        base.OnMeasured();

        if (MainLabel == null)
        {
            MainLabel = this.FindView<SkiaLabel>("MainLabel");
            MainLabel.Text = Text;
        }
        if (MainFrame == null)
        {
            MainFrame = this.FindView<SkiaShape>("MainFrame");
            this.TransformView = MainFrame;
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(Text))
        {
            if (MainLabel != null)
            {
                MainLabel.Text = this.Text;
            }
        }
    }
}