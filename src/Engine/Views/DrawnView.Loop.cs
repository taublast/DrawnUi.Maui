using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Platform;

namespace DrawnUi.Maui.Views;


public partial class DrawnView
{

    public bool IsDirty
    {
        get
        {
            return _isDirty;
        }

        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                OnPropertyChanged();
            }
        }
    }
    bool _isDirty;


    public bool CheckCanDraw()
    {
        if (UpdateLocked && StopDrawingWhenUpdateIsLocked)
            return false;

        return CanvasView != null
               && InvalidatedCanvas < 2 //this can go more with cache doublebufering - background rendering.. todo redesign
               && !IsRendering
               && IsDirty
               && IsVisible;
    }

    public virtual void Update()
    {
        IsDirty = true;
        if (!OrderedDraw && CheckCanDraw())
        {
            OrderedDraw = true;
            Task.Run(() => //do not ever try to change this! i know you want to remove it or add a ConfigureAwait but just don't :)
            {
                InvalidateCanvas();
            });
        }
    }

#if ANDROID

    protected void InvalidateCanvas()
    {
        if (CanvasView == null)
            return;

        //sanity check
        var widthPixels = (int)CanvasView.CanvasSize.Width;
        var heightPixels = (int)CanvasView.CanvasSize.Height;
        if (widthPixels > 0 && heightPixels > 0)
        {
            if (UpdateMode == UpdateMode.Constant)
            {
                InvalidatedCanvas++;
                CanvasView?.InvalidateSurface();
                return;
            }

            //optimization check
            if (NeedCheckParentVisibility)
            {
#if ANDROID || WINDOWS

                CheckElementVisibility(this);
                PassedChecks();

#else
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CheckElementVisibility(this); //need ui thread for visibility check for apple
                        PassedChecks();
                    });
#endif
            }
            else
            {
                PassedChecks();
            }

            void PassedChecks()
            {
                if (CanDraw) //passed checks
                {
                    InvalidatedCanvas++;
                    MainThread.BeginInvokeOnMainThread(async () => //if we don't use main thread on android maui bindings just die
                    {
                        CanvasView?.InvalidateSurface();
                    });
                }
                else
                {
                    OrderedDraw = false;
                }
            }
        }
        else
        {
            OrderedDraw = false;
        }
    }

#else

    protected void InvalidateCanvas()
    {
        IsDirty = true;

        if (CanvasView == null)
        {
            OrderedDraw = false;
            return;
        }

        //sanity check
        var widthPixels = (int)CanvasView.CanvasSize.Width;
        var heightPixels = (int)CanvasView.CanvasSize.Height;
        if (widthPixels > 0 && heightPixels > 0)
        {

            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (NeedCheckParentVisibility)
                        CheckElementVisibility(this);

                    Continue();
                }
                catch (Exception e)
                {
                    Super.Log(e);
                }

            });


            void Continue()
            {

                lock (lockIsWaiting)
                {
                    if (!CanvasView.IsDrawing && CanDraw && !_isWaiting)  //passed checks
                    {
                        _isWaiting = true;
                        InvalidatedCanvas++;
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            try
                            {
                                //cap fps around 120fps
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
                                if (!Super.EnableRendering)
                                {
                                    OrderedDraw = false;
                                    return;
                                }
                                CanvasView?.InvalidateSurface(); //very rarely could throw on windows here if maui destroys view when navigating, so we secured with try-catch
                            }
                            catch (Exception e)
                            {
                                Super.Log(e);
                            }
                            finally
                            {
                                _isWaiting = false;
                            }

                        });
                        return;
                    }

                    OrderedDraw = false;
                }

            }

        }
        else
        {
            OrderedDraw = false; //canvas not created yet
        }
    }

#endif
}