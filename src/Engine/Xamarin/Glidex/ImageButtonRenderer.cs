using Android.Content;
using Xamarin.Forms;

[assembly: ExportRenderer (typeof (ImageButton), typeof (Android.Glide.ImageButtonRenderer))]

namespace Android.Glide
{
	public class ImageButtonRenderer : Xamarin.Forms.Platform.Android.ImageButtonRenderer
	{
		public ImageButtonRenderer (Context context) : base (context) { }

		protected override void Dispose (bool disposing)
		{
			this.CancelGlide ();

			base.Dispose (disposing);
		}
	}
}
