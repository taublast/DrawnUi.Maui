using CoreGraphics;
using Microsoft.Maui.Platform;
using UIKit;


namespace DrawnUi.Maui.Draw;

public partial class SkiaMauiElement
{

    public bool ShowSnapshot => false;

    protected virtual void LayoutNativeView(VisualElement element)
    {
        if (element.Handler?.PlatformView is UIView nativeView)
        {
            nativeView.ClipsToBounds = true;

            nativeView.Transform = CGAffineTransform.MakeIdentity();
            nativeView.Frame = new CGRect(VisualTransformNative.Rect.Left, VisualTransformNative.Rect.Top, VisualTransformNative.Rect.Width, VisualTransformNative.Rect.Height);
            nativeView.Transform = CGAffineTransform.MakeTranslation(VisualTransformNative.Translation.X, VisualTransformNative.Translation.Y);
            nativeView.Transform = CGAffineTransform.Rotate(nativeView.Transform, VisualTransformNative.Rotation); // Assuming rotation in radians
            nativeView.Transform = CGAffineTransform.Scale(nativeView.Transform, VisualTransformNative.Scale.X, VisualTransformNative.Scale.Y);
            nativeView.Alpha = VisualTransformNative.Opacity;

            //Debug.WriteLine($"Layout Maui : {VisualTransformNative.Opacity} {VisualTransformNative.Translation} {VisualTransformNative.IsVisible}");
        }
    }

    public virtual void SetNativeVisibility(bool state)
    {
        if (Element.Handler?.PlatformView is UIView nativeView)
        {
            var visibility = state ? Visibility.Visible : Visibility.Hidden;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                nativeView.UpdateVisibility(visibility);
            });
        }
    }

    protected void RemoveMauiElement(Element element)
    {
        if (element == null)
            return;

        var layout = Superview.Handler?.PlatformView as UIView;
        if (layout != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (element.Handler?.PlatformView is UIView nativeView)
                {
                    nativeView.RemoveFromSuperview();
                    if (element is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    //todo destroy child handler?
                }
            });
        }
    }


    protected virtual void SetupMauiElement(VisualElement element)
    {
        if (element == null)
            return;

        IViewHandler handler = Superview.Handler;

        if (handler != null)
        {

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (element.Handler == null)
                {
                    //create handler
                    var childHandler = element.ToHandler(handler.MauiContext);
                }
                //add native view to canvas
                var view = element.Handler?.PlatformView as UIView;
                var layout = Superview.Handler?.PlatformView as UIView;
                if (layout != null)
                    layout.AddSubview(view);

                LayoutNativeView(Element);
            });

        }

    }

}
