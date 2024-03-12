using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using CompositeTransform = Microsoft.UI.Xaml.Media.CompositeTransform;
using Visibility = Microsoft.UI.Xaml.Visibility;


namespace DrawnUi.Maui.Draw;

public partial class SkiaMauiElement
{

    protected async Task TakeNativeSnapshot(SKSurface skiaSurface)
    {
        if (Element.Handler?.PlatformView is FrameworkElement nativeView)
        {
            if (nativeView.Width > 0 && nativeView.Height > 0)
            {
                var snapshot = await CreateBitmapFromView(nativeView);
                //snapshot is null!
                CachedBitmap.Canvas.DrawBitmap(snapshot, 0, 0);
                CachedBitmap.Canvas.Flush();
                SnapshotReady = true;
            }
        }
    }

    public static async Task<SKBitmap> CreateBitmapFromView(FrameworkElement view)
    {
        try
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(view);

            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            using (var stream = pixelBuffer.AsStream())
            {
                // Create an SKBitmap directly from the pixel data
                var info = new SKImageInfo(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight);
                var skBitmap = new SKBitmap();

                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                skBitmap.InstallPixels(info, handle.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) =>
                {
                    var gch = GCHandle.FromIntPtr((IntPtr)ctx);
                    gch.Free();
                }, GCHandle.ToIntPtr(handle));

                return skBitmap;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Error creating bitmap: {ex}");
            return null;
        }
    }





    public virtual void SetNativeVisibility(bool state)
    {
        if (Element.Handler?.PlatformView is FrameworkElement nativeView)
        {
            nativeView.Visibility = !state ? Visibility.Collapsed : Visibility.Visible;
            //Trace.WriteLine($"Layout Maui SetNativeVisibility : {nativeView.Visibility}");
            IsNativeVisible = nativeView.Visibility == Visibility.Visible;
        }
    }

    protected virtual void LayoutNativeView(VisualElement element)
    {
        if (element.Handler?.PlatformView is FrameworkElement nativeView)
        {
            nativeView.Visibility = !VisualTransformNative.IsVisible ? Visibility.Collapsed : Visibility.Visible;

            IsNativeVisible = nativeView.Visibility == Visibility.Visible;

            if (nativeView.Visibility == Visibility.Visible)
            {
                //UIElement
                nativeView.Width = VisualTransformNative.Rect.Width;
                nativeView.Height = VisualTransformNative.Rect.Height;
                nativeView.Opacity = VisualTransformNative.Opacity;

                // Creating a new CompositeTransform to handle the transforms
                var transform = new CompositeTransform
                {
                    TranslateX = VisualTransformNative.Translation.X,
                    TranslateY = VisualTransformNative.Translation.Y,
                    Rotation = VisualTransformNative.Rotation, // Assuming rotation is in degrees
                    ScaleX = VisualTransformNative.Scale.X,
                    ScaleY = VisualTransformNative.Scale.Y
                };

                nativeView.RenderTransform = transform;


                nativeView.UpdateLayout();

                Windows.Foundation.Size availableSize = new(VisualTransformNative.Rect.Width, VisualTransformNative.Rect.Height);
                nativeView.Measure(availableSize);
                nativeView.Arrange(new Windows.Foundation.Rect(VisualTransformNative.Rect.Left, VisualTransformNative.Rect.Top, nativeView.DesiredSize.Width, nativeView.DesiredSize.Height));

                if (!WasRendered)
                    WasRendered = nativeView.RenderSize.Width > 0;
            }

            nativeView.UpdateLayout(); //required. in maui this is also needed to be called as fix for after IsVisible is set to true sometimes the view just doesn't show up.

            // Super.Log($"[LayoutNativeView] at {VisualTransformNative.Rect.Top}, vis {nativeView.Visibility}, opa {nativeView.Opacity} wid {nativeView.RenderSize.Width}");
        }
    }

    public void UpdateNativeLayout()
    {
        if (Element != null && Element.Handler?.PlatformView is FrameworkElement nativeView)
        {
            nativeView.UpdateLayout();
        }
    }

    protected void RemoveMauiElement(Element element)
    {
        if (element == null)
            return;

        var layout = Superview.Handler?.PlatformView as ContentPanel;
        if (layout != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (element.Handler?.PlatformView is FrameworkElement nativeView)
                {
                    layout.Children.Remove(nativeView);
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
                var view = element.Handler?.PlatformView as UIElement;
                var layout = Superview.Handler?.PlatformView as ContentPanel;
                if (layout != null)
                    layout.Children.Add(view);

                LayoutNativeView(Element);
            });
        }
    }

}