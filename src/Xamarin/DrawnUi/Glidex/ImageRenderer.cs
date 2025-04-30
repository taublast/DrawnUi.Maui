using Android.Content;
using Xamarin.Forms;

[assembly: ExportRenderer (typeof (Image), typeof (Android.Glide.ImageRenderer))]

namespace Android.Glide
{
	public class ImageRenderer : Xamarin.Forms.Platform.Android.FastRenderers.ImageRenderer
	{
		public ImageRenderer (Context context) : base (context) { }

		protected override void Dispose (bool disposing)
		{
			this.CancelGlide ();

			base.Dispose (disposing);
		}
	}
}
