
using Xamarin.Essentials;

namespace DrawnUi.Views
{

    public partial class DrawnView
    {


        protected virtual void OnSizeChanged()
        {
#if ANDROID
            if (Handler != null) //this is basically for clipping SkiaMauiElement
            {
                var layout = this.Handler.PlatformView as Android.Views.ViewGroup;
                layout.SetClipChildren(true);
                layout.ClipBounds = new Android.Graphics.Rect(0, 0, (int)(Width * RenderingScale), (int)(Height * RenderingScale));
            }

#elif WINDOWS
            if (Handler != null) //this is basically for clipping SkiaMauiElement
            {
                var layout = Handler.PlatformView as ContentPanel;
                layout.Clip = new Microsoft.UI.Xaml.Media.RectangleGeometry
                {
                    Rect = new Windows.Foundation.Rect(0, 0, Width, Height)
                };
            }

#elif IOS || MACCATALYST

			if (Handler?.PlatformView is UIKit.UIView nativeView)
			{
				nativeView.ClipsToBounds = true;
			}

#endif

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
            IsHiddenInViewTree = !GetIsVisibleWithParent(this);
            return;

            NeedCheckParentVisibility = false;

            while (element != null)
            {

                bool isVisible = true;

                if (Device.RuntimePlatform == Device.Android)
                {
                    if (element == this)
                    {
                        isVisible = Parent != null && IsVisible && Super.Native.CheckNativeVisibility(this.Handler);
                    }
                    else
                    {
                        isVisible = element.Parent != null && element.IsVisible;
                    }

                    if (!isVisible)
                    {
                        IsHiddenInViewTree = true;
                        return;
                    }
                }
                else
                if (Device.RuntimePlatform == Device.iOS)
                {
                    //WARNING this must be called form UI thread only!
                    if (element == this)
                    {
                        isVisible = Parent != null && IsVisible && Super.Native.CheckNativeVisibility(this.Handler);
                    }
                    else
                    {
                        isVisible = element.Parent != null && element.IsVisible;
                    }

                    if (!isVisible)
                    {
                        IsHiddenInViewTree = true;
                        return;
                    }
                }
#if IOS || MACCATALYST

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
#endif

#if ANDROID
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
#endif

                //            if (parent is View view)
                //{
                //                if (!view.IsVisible)
                //                {
                //                    IsHiddenInViewTree = true;
                //		return;
                //                }
                //            }
                element = element.Parent as VisualElement;
            }

            IsHiddenInViewTree = false;
        }



    }
}
