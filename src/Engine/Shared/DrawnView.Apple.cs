﻿using CoreAnimation;
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
            Super.DisplayLinkCallback += OnDisplayLink;
        }

        private void OnDisplayLink(object sender, EventArgs e)
        {
            OnFrame();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

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

        public bool CheckCanDraw()
        {
            return CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
                   && IsDirty
                   && !(UpdateLocked && StopDrawingWhenUpdateIsLocked)
                   && IsVisible && Super.EnableRendering;
        }

        protected virtual void DisposePlatform()
        {
            Super.DisplayLinkCallback -= OnDisplayLink;

            //_displayLink?.RemoveFromRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
            //_displayLink?.Dispose();
            //_displayLink = null;
        }
    }
}
