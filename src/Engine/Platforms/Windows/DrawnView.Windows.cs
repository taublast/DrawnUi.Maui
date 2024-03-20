using Microsoft.Maui.Handlers;

namespace DrawnUi.Maui.Views
{

    public partial class DrawnView
    {

        public virtual void Update()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Will be called on ui thread on windows
        /// </summary>
        /// <param name="nanoseconds"></param>
        private void OnFrame(long nanoseconds)
        {
            if (CheckCanDraw() && IsDirty)
            {
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this);

                CanvasView?.Update();
            }
        }

        public bool CheckCanDraw()
        {
            return CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
                   && IsDirty
                   && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
                   && IsVisible && Super.EnableRendering;
        }


    }
}
