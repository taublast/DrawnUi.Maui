#define CHOREOGRAPHER

using Android.Views;
using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Views
{

    public partial class DrawnView
    {

        protected virtual void OnSizeChanged()
        {
            if (Handler != null) //this is basically for clipping SkiaMauiElement
            {
                var layout = this.Handler.PlatformView as Android.Views.ViewGroup;
                layout.SetClipChildren(true);
                layout.ClipBounds = new Android.Graphics.Rect(0, 0, (int)(Width * RenderingScale), (int)(Height * RenderingScale));
            }


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

            if (element != null)
            {


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

                element = element.Parent;
            }


            IsHiddenInViewTree = false;
        }

#if CHOREOGRAPHER

        private static Super.FrameCallback _frameCallback;

        protected virtual void DisposePlatform()
        {
            Choreographer.Instance.RemoveFrameCallback(_frameCallback);
        }

        bool _loopStarting = false;
        bool _loopStarted = false;

        public virtual void SetupRenderingLoop()
        {
            _loopStarting = false;
            _loopStarted = false;

            _frameCallback = new Super.FrameCallback((nanos) =>
            {
                OnFrame();

                Choreographer.Instance.PostFrameCallback(_frameCallback);
            });

            Tasks.StartDelayed(TimeSpan.FromMilliseconds(1), async () =>
            {
                while (!_loopStarted)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (_loopStarting)
                            return;
                        _loopStarting = true;

                        if (MainThread.IsMainThread)
                        {
                            if (!_loopStarted)
                            {
                                _loopStarted = true;
                                Choreographer.Instance.PostFrameCallback(_frameCallback);
                            }
                        }

                        _loopStarting = false;
                    });
                    await Task.Delay(100);
                }

            });

        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {

        }

#else

        protected virtual void DisposePlatform()
        {
            Looper?.Dispose();
        }

        Looper Looper { get; set; }

        public virtual void SetupRenderingLoop()
        {
            Looper = new(OnFrame);
            Looper.Start(GetTargetFps());
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {
            Looper?.SetTargetFps(GetTargetFps());
        }

        int GetTargetFps()
        {
            return 120;
        }

#endif

        private void OnFrame()
        {
            if (CheckCanDraw() && !OrderedDraw)
            {
                OrderedDraw = true;
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
