using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Platform;

namespace DrawnUi.Maui.Views;

public partial class DrawnView
{

#if !ANDROID && !WINDOWS

    SemaphoreSlim _semaphoreOnFrame = new(1, 1);
    long _onFrameTime;

    /// <summary>
    /// UpdateMode.Constant callback
    /// </summary>
    /// <param name="nanoseconds"></param>
    private void OnFrame(long nanoseconds)
    {
        if (!CheckCanDraw() || CanvasView.IsDrawing)
        {
            CanvasView?.PostponeInvalidation();
            return;
        }

        OrderedDraw = true;

        InvalidateCanvas();
    }

    public bool CheckCanDraw()
    {
        if (UpdateLocked && StopDrawingWhenUpdateIsLocked)
            return false;

        return CanvasView != null
               && !IsRendering
               && IsDirty
               && IsVisible && Super.EnableRendering;
    }


    public virtual void Update()
    {
        IsDirty = true;
        if (!OrderedDraw && !IsDirty && CheckCanDraw())
        {
            OrderedDraw = true;
            InvalidateCanvas();
        }
    }

    protected async void InvalidateCanvas()
    {
        IsDirty = true;

        if (CanvasView == null)
        {
            OrderedDraw = false; //todo could never update after
            return;
        }

        if (CanvasView.CanvasSize is { Width: > 0, Height: > 0 })
        {
            if (_isWaiting)  //busy
            {
                NeedRedraw = true;
                OrderedDraw = false;
            }
            else
            {
                NeedRedraw = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    _isWaiting = true;
                    try
                    {
                        if (NeedCheckParentVisibility)
                            CheckElementVisibility(this);

                        if (UpdateMode == UpdateMode.Constant)
                        {
                            InvalidatedCanvas++;
                            CanvasView?.Update();
                            return;
                        }

                        if (!CanvasView.IsDrawing && CanDraw)  //passed checks
                        {
                            InvalidatedCanvas++;

                            //cap fps around 120fps
                            var nowNanos = Super.GetCurrentTimeNanos();
                            var elapsedMicros = (nowNanos - CanvasView.FrameTime) / 1_000.0;

                            var needWait =
                                Super.CapMicroSecs
                                - elapsedMicros;
                            if (needWait >= 1000)
                            {
                                await Task.Delay(TimeSpan.FromMicroseconds(needWait));
                            }
                            else
                            {
                                await Task.Delay(1); //unblock ui thread
                            }

                            if (!Super.EnableRendering)
                            {
                                OrderedDraw = false;
                                return;
                            }

                            CanvasView?.Update();
                            return;
                        }

                        OrderedDraw = false;
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                    finally
                    {
                        _isWaiting = false;
                        if (NeedRedraw) //if we missed previous update
                        {
                            NeedRedraw = false;
                            Task.Run(() =>
                            {
                                Update();
                            }).ConfigureAwait(false);
                        }
                    }
                });


            }
        }
        else
        {
            OrderedDraw = false; //canvas not created yet
            await Task.Delay(30);
            await Task.Run(() =>
            {
                Update();
            }).ConfigureAwait(false);
        }
    }


#endif

    public bool NeedRedraw { get; set; }

    public bool IsDirty
    {
        get
        {
            return _isDirty;// || UpdateMode == UpdateMode.Constant;
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


}