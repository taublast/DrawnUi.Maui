#define CHOREOGRAPHER //otherwise will use Looper like Windows 

using Android.Views;
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
        public void CheckElementVisibility(VisualElement element)
        {
            NeedCheckParentVisibility = false;
            IsHiddenInViewTree = !IsVisibleInViewTree();


            //if (element != null)
            //{

            //    if (element.Handler != null)
            //    {
            //        if (element.Handler.PlatformView is Android.Views.View nativeView)
            //        {
            //            if (nativeView.Visibility != Android.Views.ViewStates.Visible)
            //            {
            //                IsHiddenInViewTree = true;
            //                return;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (element.GetVisualElementWindow() == null)
            //        {
            //            IsHiddenInViewTree = true;
            //            return;
            //        }
            //    }


            //}


            //IsHiddenInViewTree = false;
        }

#if CHOREOGRAPHER


        protected virtual void DisposePlatform()
        {
            Super.ChoreographerCallback -= OnChoreographer;
        }

        object lockFrame = new();

        private void OnChoreographer(object sender, EventArgs e)
        {
            lock (lockFrame)
            {
                if (CheckCanDraw())
                {
                    OrderedDraw = true;
                    if (NeedCheckParentVisibility)
                        CheckElementVisibility(this);

                    CanvasView?.Update();
                }
            }
        }


        public virtual void SetupRenderingLoop()
        {
            Super.ChoreographerCallback -= OnChoreographer;
            Super.ChoreographerCallback += OnChoreographer;
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {

        }

#else

        protected virtual void DisposePlatform()
        {
            Looper?.Dispose();
        }

        public virtual void SetupRenderingLoop()
        {
            Looper?.Dispose();
            Looper = new(OnFrame);
            Looper.Start(120);
        }

        protected virtual void PlatformHardwareAccelerationChanged()
        {
            if (Looper != null && Looper.IsRunning)
            {
                SetupRenderingLoop();
            }
        }

        Looper Looper { get; set; }

        public void OnFrame()
        {
            //background thread

            if (CheckCanDraw())
            {
                OrderedDraw = true;
                if (NeedCheckParentVisibility)
                    CheckElementVisibility(this);

                CanvasView?.Update();
            }
        }



#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePlatform()
        {
            IsDirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckCanDraw()
        {
            return //!OrderedDraw

                CanvasView != null && this.Handler != null && this.Handler.PlatformView != null
               //&& !CanvasView.IsDrawing
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
