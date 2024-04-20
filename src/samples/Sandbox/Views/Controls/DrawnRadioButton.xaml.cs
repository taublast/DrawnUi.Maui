using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

public partial class DrawnRadioButton
{
    public DrawnRadioButton()
    {
        InitializeComponent();
    }

    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
        SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {

        if (touchAction == TouchActionResult.Tapped)
        {
            var ptsInsideControl = GetOffsetInsideControlInPoints(args.Location, childOffset);
            this.PlayRippleAnimation(Colors.CornflowerBlue, ptsInsideControl.X, ptsInsideControl.Y);
        }

        return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
    }
}