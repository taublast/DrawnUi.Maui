using CoreAnimation;
using Foundation;
using Microsoft.Maui.Handlers;
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


                element = element.Parent;
            }


            IsHiddenInViewTree = false;
        }

        protected virtual void OnSizeChanged()
        {
            if (Handler?.PlatformView is UIKit.UIView nativeView)
            {
                nativeView.ClipsToBounds = true;
            }

            Update();
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {

        }

        public virtual void SetupRenderingLoop()
        {
            Super.DisplayLinkCallback -= OnDisplayLink;
            Super.DisplayLinkCallback += OnDisplayLink;
        }

        private void OnDisplayLink(object sender, EventArgs e)
        {
            if (CheckCanDraw())
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

        public bool CheckCanDraw()
        {
            return
                !OrderedDraw &&
                IsDirty &&
                CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
                    && !CanvasView.IsDrawing
                   && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
                   && IsVisible && Super.EnableRendering;
        }

        protected virtual void DisposePlatform()
        {
            Super.DisplayLinkCallback -= OnDisplayLink;
        }
    }
}
