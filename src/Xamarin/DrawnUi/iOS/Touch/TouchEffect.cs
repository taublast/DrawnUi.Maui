using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using TouchEffect = AppoMobi.Framework.iOS.Touch.TouchEffect;

[assembly: ResolutionGroupName("AMFEffects")]
[assembly: ExportEffect(typeof(TouchEffect), "TouchEffect")]
namespace AppoMobi.Framework.iOS.Touch
{
    public class TouchEffect : PlatformEffect
    {
        UIView view;

        TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {


            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            Forms.Touch.TouchEffect touchEffect = (Forms.Touch.TouchEffect)Element.Effects.FirstOrDefault(e => e is Forms.Touch.TouchEffect);


            if (touchEffect != null && view != null)
            {
                touchEffect.Attach();

                formsEffect = touchEffect;

                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, touchEffect);

                view.AddGestureRecognizer(touchRecognizer);

                //panRecognizer = new PanGestureRecognizer(Element, view, touchEffect);
                //view.AddGestureRecognizer(panRecognizer);

                formsEffect.Disposing += OnFormsDisposing;
            }
        }

        Forms.Touch.TouchEffect formsEffect;

        private void OnFormsDisposing(object sender, EventArgs e)
        {
            OnDetached();
        }

        protected override void OnDetached()
        {
            if (formsEffect != null)
            {
                formsEffect.Disposing -= OnFormsDisposing;
                formsEffect?.Dispose();
                formsEffect = null;
            }

            if (touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                view.RemoveGestureRecognizer(touchRecognizer);
            }

            //if (panRecognizer != null)
            //{
            //    // Clean up the TouchRecognizer object
            //    panRecognizer.Detach();

            //    // Remove the TouchRecognizer from the UIView
            //    view.RemoveGestureRecognizer(panRecognizer);
            //}

        }
    }
}