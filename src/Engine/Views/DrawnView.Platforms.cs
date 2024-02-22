using Microsoft.Maui.Platform;

namespace DrawnUi.Maui.Views;


public partial class DrawnView
{


    protected virtual void OnSizeChanged()
    {

#if ANDROID
        if (Handler != null) //this is basically for clipping SkiaMauiElement
        {
            var layout = this.Handler.PlatformView as Android.Views.ViewGroup;
            layout.SetClipChildren(true);
            layout.ClipBounds = new Android.Graphics.Rect(0, 0, (int)(Width * RenderingScale), (int)(Height * RenderingScale));
        }

#elif WINDOWS
        if (Handler != null) //this is basically for clipping SkiaMauiElement
        {
            var layout = Handler.PlatformView as ContentPanel;
            layout.Clip = new Microsoft.UI.Xaml.Media.RectangleGeometry
            {
                Rect = new Windows.Foundation.Rect(0, 0, Width, Height)
            };
        }

#elif IOS || MACCATALYST

        if (Handler?.PlatformView is UIKit.UIView nativeView)
        {
            nativeView.ClipsToBounds = true;
        }

#endif

        Update();
    }





    /// <summary>
    /// To optimize rendering and not update controls that are inside storyboard that is offscreen or hidden
    /// Apple - UI thread only !!!
    /// If you set 
    /// </summary>
    /// <param name="element"></param>
    public void CheckElementVisibility(Element element)
    {
        NeedCheckParentVisibility = false;

        while (element != null)
        {

#if IOS || MACCATALYST

            //WARNING this must be called form UI thread only!

            if (element.Handler != null)
            {
                if (element.Handler.PlatformView is UIKit.UIView iosView)
                {
                    if (iosView.Hidden)
                    {
                        IsHiddenInViewTree = true;
                        return;
                    }
                }
            }
            else
            {
                if (element.GetVisualElementWindow() == null)
                {
                    IsHiddenInViewTree = true;
                    return;
                }
            }
#endif

#if ANDROID
            if (element.Handler != null)
            {
                if (element.Handler.PlatformView is Android.Views.View nativeView)
                {
                    if (nativeView.Visibility != Android.Views.ViewStates.Visible)
                    {
                        IsHiddenInViewTree = true;
                        return;
                    }
                }
            }
            else
            {
                if (element.GetVisualElementWindow() == null)
                {
                    IsHiddenInViewTree = true;
                    return;
                }
            }
#endif

            //            if (parent is View view)
            //{
            //                if (!view.IsVisible)
            //                {
            //                    IsHiddenInViewTree = true;
            //		return;
            //                }
            //            }
            element = element.Parent;
        }


        IsHiddenInViewTree = false;
    }

    SemaphoreSlim _semaphoreInvalidate = new(1, 1);

    /*
    protected void InvalidateCanvas()
    {
        if (CanvasView == null)
        {
            OrderedDraw = false;
            return;
        }

        var widthPixels = (int)CanvasView.CanvasSize.Width;
        var heightPixels = (int)CanvasView.CanvasSize.Height;
        if (widthPixels > 0 && heightPixels > 0)
        {
            //optimization check
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this); //need ui thread for visibility check for apple
                if (CanDraw) //passed checks
                {
                    InvalidatedCanvas++;

                    //pls wait we are already rendering..
                    while (IsRendering)
                    {
                        await Task.Delay(5);
                    }

                    //we can't cap android with task.delay and get a smooth perf
                    //while other platforms look okay with that approach
#if !ANDROID
                    await _semaphoreInvalidate.WaitAsync();

                    var nowNanos = Super.GetCurrentTimeNanos();
                    var elapsedMicros = (nowNanos - _lastUpdateTimeNanos) / 1_000.0;
                    _lastUpdateTimeNanos = nowNanos;

                    var needWait =
                        Super.CapMicroSecs
#if IOS || MACCATALYST
                                * 2 // apple is double buffered                             
#endif
                        - elapsedMicros;
                    if (needWait < 1)
                        needWait = 1;

                    var ms = (int)(needWait / 1000);
                    if (ms < 1)
                        ms = 1;
                    await Task.Delay(ms);

                    _semaphoreInvalidate.Release();
#endif

                    if (!Super.EnableRendering)
                    {
                        OrderedDraw = false;
                        return;
                    }

                    try
                    {
                        CanvasView?.InvalidateSurface();
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                }
                else
                {
                    OrderedDraw = false;
                }
            });
        }
        else
        {
            OrderedDraw = false;
        }
    }

 
}
*/


}