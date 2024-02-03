using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;
using ContentView = Microsoft.Maui.Platform.ContentView;

namespace DrawnUi.Maui.Controls;

public class DrawnUiBasePageHandler : PageHandler
{
    protected override void ConnectHandler(ContentView nativeView)
    {
        base.ConnectHandler(nativeView);
    }

    protected override ContentView CreatePlatformView()
    {
        return new CustomView(); //base.CreatePlatformView();
    }

    public class CustomView : ContentView
    {
        public bool TracksKeyboard => DependencyExtensions.StartupSettings != null &&
                                      DependencyExtensions.StartupSettings.UseDesktopKeyboard;

        public override bool CanBecomeFirstResponder
        {
            get
            {
                if (TracksKeyboard)
                    return true;
                return base.CanBecomeFirstResponder;
            }
        }

        public override void DidUpdateFocus(UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
        {
            base.DidUpdateFocus(context, coordinator);

            if (TracksKeyboard && context.NextFocusedItem == null) Super.RequestMainResponder(this);
        }

        public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
        {
            if (TracksKeyboard)
            {
                var consumed = false;
                foreach (UIPress press in presses)
                {
                    var mapped = KeyboardManager.MapToMaui((int)press.Type);
                    KeyboardManager.KeyboardPressed(mapped);

                    consumed = true;
                }

                if (consumed) return;
            }

            base.PressesBegan(presses, evt);
        }

        public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
        {
            base.PressesEnded(presses, evt);

            if (TracksKeyboard)
                foreach (UIPress press in presses)
                {
                    var mapped = KeyboardManager.MapToMaui((int)press.Type);
                    KeyboardManager.KeyboardReleased(mapped);
                    //Trace.WriteLine($"[KEY] {press.Type}/{(int)press.Type} => {mapped}");
                }
        }
    }
}