using Android.Graphics;
using Android.Views;
using Microsoft.Maui.Platform;
using View = Android.Views.View;


namespace DrawnUi.Maui.Draw;

public partial class SkiaMauiElement
{
    protected async Task TakeNativeSnapshot(SKSurface skiaSurface)
    {
        if (Element.Handler?.PlatformView is View nativeView)
        {
            if (nativeView.Width > 0 && nativeView.Height > 0)
            {
                using var snapshot = CreateBitmapFromView(nativeView);
                CachedBitmap.Canvas.DrawBitmap(snapshot, 0, 0);
                // Make sure to call Flush() to ensure that the drawing commands are executed
                CachedBitmap.Canvas.Flush();
                SnapshotReady = true;
            }
        }
    }

    public SKBitmap CreateBitmapFromView(Android.Views.View view)
    {
        Bitmap bitmap = Bitmap.CreateBitmap(view.Width, (int)(view.Height), Bitmap.Config.Argb8888);
        var canvas = new Android.Graphics.Canvas(bitmap);

        //canvas.DrawColor(Android.Graphics.Color.Red); //debug snapshot with color

        view.Draw(canvas);

        // Convert the Android Bitmap to a SkiaSharp SKBitmap
        using (var stream = new MemoryStream())
        {
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Position = 0;
            return SKBitmap.Decode(stream);
        }
    }


    public virtual void SetNativeVisibility(bool state)
    {
        if (Element.Handler?.PlatformView is View nativeView)
        {
            nativeView.Visibility = state ? ViewStates.Visible : ViewStates.Invisible;
        }
    }

    protected virtual void LayoutMauiElementUnsafe(VisualElement element)
    {
        LayoutMauiElement(VisualTransformNative.Rect.Width / RenderingScale, VisualTransformNative.Rect.Height / RenderingScale);

        Super.Log($"[ELEM] {VisualTransformNative.Rect}");

        if (element.Handler?.PlatformView is View nativeView)
        {
            Super.Log($"[ELEM] has View, tY: {VisualTransformNative.Translation.Y}");

            nativeView.TranslationX = VisualTransformNative.Translation.X;
            nativeView.TranslationY = VisualTransformNative.Translation.Y;
            nativeView.Rotation = VisualTransformNative.Rotation;
            nativeView.ScaleX = VisualTransformNative.Scale.X;
            nativeView.ScaleY = VisualTransformNative.Scale.Y;
            nativeView.Alpha = VisualTransformNative.Opacity;

            //int widthMeasureSpec = View.MeasureSpec.MakeMeasureSpec((int)VisualTransformNative.Rect.Width, MeasureSpecMode.Exactly);
            //int heightMeasureSpec = View.MeasureSpec.MakeMeasureSpec((int)VisualTransformNative.Rect.Height, MeasureSpecMode.Exactly);
            //nativeView.Measure(widthMeasureSpec, heightMeasureSpec);

            nativeView.Layout((int)VisualTransformNative.Rect.Left, (int)VisualTransformNative.Rect.Top, (int)VisualTransformNative.Rect.Right, (int)VisualTransformNative.Rect.Bottom);
        }
    }

    protected void RemoveMauiElement(Element element)
    {
        if (element == null)
            return;

        var layout = Superview.Handler?.PlatformView as ViewGroup;
        if (layout != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (element.Handler?.PlatformView is View nativeView)
                {
                    layout.RemoveView(nativeView);
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
        {
            Super.Log($"[ELEM] SetupMauiElement exit");
            return;
        }

        IViewHandler handler = Superview.Handler;

        if (handler != null)
        {
            element.BindingContext = this.BindingContext;

            //lock (lockLayout)
            {
                if (element.Handler == null)
                {
                    //create handler
                    //Super.Log($"[ELEM] creating handler..");
                    var childHandler = element.ToHandler(handler.MauiContext);
                    NeedsLayoutNative = true;

                    Tasks.StartDelayed(TimeSpan.FromMilliseconds(50), () =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Element.InvalidateMeasureNonVirtual(Microsoft.Maui.Controls.Internals.InvalidationTrigger.HorizontalOptionsChanged);
                        });
                    });

                }

                //add native view to canvas
                var view = element.Handler?.PlatformView as Android.Views.View;

                //Super.Log($"[ELEM] 2 {view} => {Element.Handler}");

                LayoutMauiElement(false); //apply transforms etc before showing for the first time

                var layout = Superview.Handler?.PlatformView as ViewGroup;
                if (layout != null)
                    layout.AddView(view);
            }



        }

    }

}

public class MakeInputTransparent : Java.Lang.Object, View.IOnTouchListener
{
    public bool OnTouch(View v, MotionEvent e)
    {
        return false;
    }
}
