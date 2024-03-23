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
            if (CheckCanDraw())
            {
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this);

                CanvasView?.Update();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
