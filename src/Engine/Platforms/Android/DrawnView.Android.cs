using Android.Views;
using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Views
{

    public partial class DrawnView
    {

        public virtual void SetupRenderingLoop()
        {
            Super.OnFrame += OnFrame;
        }

        protected virtual void DisposePlatform()
        {

        }

        private void OnFrame(long nanoseconds)
        {
            if (CheckCanDraw())
            {
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this);

                CanvasView?.Update();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

        /*
        protected async Task InvalidateCanvas()
        {
            if (CanvasView == null)
                return;

            //sanity check
            if (CanvasView.CanvasSize is { Width: > 0, Height: > 0 })
            {
                if (NeedCheckParentVisibility)
                {
                    CheckElementVisibility(this);
                }

                if (UpdateMode == UpdateMode.Constant)
                {
                    InvalidatedCanvas++;
                    CanvasView?.Update();
                    return;
                }

                if (CanDraw) //passed checks
                {
                    _isWaiting = true;
                    InvalidatedCanvas++;

                    //cap fps around 120fps
                    var nowNanos = Super.GetCurrentTimeNanos();
                    var elapsedMicros = (nowNanos - CanvasView.FrameTime) / 1_000.0;

                    var needWait =
                        Super.CapMicroSecs
                        - elapsedMicros;
                    if (needWait >= 1)
                    {
                        var ms = (int)(needWait / 1000);
                        if (ms < 1)
                            ms = 1;
                        await Task.Delay(ms);
                    }
                    else
                    {
                        //await Task.Delay(1);
                    }

                    _isWaiting = false;

                    if (!Super.EnableRendering)
                    {
                        OrderedDraw = false;
                        return;
                    }

                    CanvasView?.Update();

                }
                else
                {
                    OrderedDraw = false;
                }

            }
        }
        */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckCanDraw()
        {
            return CanvasView != null
                   && IsDirty
                   && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
                   && IsVisible && Super.EnableRendering;
        }

        protected void OnHandlerChangedInternal()
        {
            if (this.Handler != null)
            {
                //intercept focus other wise native entries above will not unfocus
                if (this.Handler?.PlatformView is Android.Views.View view)
                {
                    view.Focusable = true;
                    view.FocusableInTouchMode = true;
                    if (view is ViewGroup group)
                    {
                        group.DescendantFocusability = DescendantFocusability.BeforeDescendants;
                    }
                }
            }
        }
    }
}
