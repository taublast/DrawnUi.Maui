using AppoMobi.Xamarin.DrawnUi.iOS;
using DrawnUi.Maui.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawnView), typeof(DrawnViewRenderer))]

namespace AppoMobi.Xamarin.DrawnUi.iOS;

public class DrawnViewRenderer : ViewRenderer
{
    private DrawnView _parent;

    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
        base.OnElementChanged(e);

        if (e.NewElement != null)
        {
            if (Element is DrawnView parent)
            {
                _parent = parent;
                _parent.Handler = this;
            }
        }
        else
        {
            if (_parent != null)
            {
                _parent.Handler = null;
                _parent = null;
            }
        }
    }

}