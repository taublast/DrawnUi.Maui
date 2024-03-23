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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Will be called on ui thread on windows
        /// </summary>
        /// <param name="nanoseconds"></param>
        private void OnFrame(long nanoseconds)
        {
            // interestingly at app startup there can be
            // a case when even using MainThread.BeginInvokeOnMainThread
            // we get no main thread yet so we avoid crash with a check:
            if (MainThread.IsMainThread)
            {
                if (CheckCanDraw() && IsDirty)
                {
                    if (NeedCheckParentVisibility)
                        CheckElementVisibility(this);

                    CanvasView?.Update();
                }
            }
        }

        public bool CheckCanDraw()
        {
            return CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
                   && IsDirty
                   && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
                   && IsVisible && Super.EnableRendering;
        }

        protected virtual void DisposePlatform()
        {

        }
    }
}
