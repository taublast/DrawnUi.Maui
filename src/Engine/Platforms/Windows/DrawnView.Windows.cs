using Microsoft.Maui.Platform;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Views
{

    public partial class DrawnView
    {

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


                //todo


                element = element.Parent;
            }


            IsHiddenInViewTree = false;
        }


        protected virtual void OnSizeChanged()
        {

            if (Handler != null) //this is basically for clipping SkiaMauiElement
            {
                var layout = Handler.PlatformView as ContentPanel;
                layout.Clip = new Microsoft.UI.Xaml.Media.RectangleGeometry
                {
                    Rect = new Windows.Foundation.Rect(0, 0, Width, Height)
                };
            }

            Update();
        }

        public virtual void SetupRenderingLoop()
        {
            Super.OnFrame -= OnSuperFrame;
            Super.OnFrame += OnSuperFrame;
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckCanDraw()
        {
            return
               CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
               && !OrderedDraw
               && !CanvasView.IsDrawing
               && IsDirty
               && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
               && IsVisible && Super.EnableRendering;
        }

        private void OnSuperFrame(object sender, EventArgs e)
        {
            if (CheckCanDraw())
            {
                OrderedDraw = true;
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this);

                CanvasView?.Update();
            }
        }

        protected virtual void DisposePlatform()
        {
            Super.OnFrame -= OnSuperFrame;
        }

    }
}
