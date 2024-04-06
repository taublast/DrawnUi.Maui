using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.UI.Core;

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

            Looper?.Dispose();
            Looper = new(OnFrame);
            if (IsUsingHardwareAcceleration)
            {
                Looper.StartOnMainThread(120);
            }
            else
            {
                Looper.StartOnMainThread(120);
            }
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {
            if (Looper != null && Looper.IsRunning)
            {
                SetupRenderingLoop();
            }
        }

        Looper Looper { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

        object lockOnFrame = new();

        private void OnFrame()
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

        protected virtual void DisposePlatform()
        {
            Looper?.Dispose();
        }


    }
}
