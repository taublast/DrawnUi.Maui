using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;
using Visibility = Microsoft.UI.Xaml.Visibility;

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
        public void CheckElementVisibility(VisualElement element)
        {
            NeedCheckParentVisibility = false;
            IsHiddenInViewTree = !IsVisibleInViewTree();
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

        private long test;
        private void OnSuperFrame(object sender, EventArgs e)
        {
            if (CheckCanDraw())
            {
                OrderedDraw = true;
                if (NeedCheckParentVisibility)
                {
                    CheckElementVisibility(this);
                }

                CanvasView?.Update();
            }
        }

        protected virtual void DisposePlatform()
        {
            Super.OnFrame -= OnSuperFrame;
        }

    }
}
