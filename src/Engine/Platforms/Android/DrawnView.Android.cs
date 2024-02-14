using Android.Views;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Maui.Views
{


    public partial class DrawnView
    {
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
