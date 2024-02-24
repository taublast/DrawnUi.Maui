using Microsoft.Maui.Platform;

namespace DrawnUi.Maui.Views
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
        public void CheckElementVisibility(Element element)
        {
            NeedCheckParentVisibility = false;

            while (element != null)
            {

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
                element = element.Parent;
            }


            IsHiddenInViewTree = false;
        }

   

    }
}
