#nullable enable
using System.Threading;
using Android.Widget;
using Bumptech.Glide;
using Xamarin.Forms;

namespace Android.Glide
{
	/// <summary>
	/// Interface for customizing calls to Glide
	/// </summary>
	public interface IGlideHandler
	{
		/// <summary>
		/// A callback that glidex.forms calls prior to making default calls to Glide
		/// </summary>
		/// <param name="imageView">The Android ImageView</param>
		/// <param name="source">The ImageSource from Xamarin.Forms</param>
		/// <param name="builder">Might be null, the Glide RequestBuilder object to work with</param>
		/// <param name="token">The CancellationToken if you need it</param>
		/// <returns>True if the image was handled. Return false if you need the image to be cleared for you.</returns>
		bool Build (ImageView imageView, ImageSource source, RequestBuilder? builder, CancellationToken token);

		/// <summary>
		/// A callback that glidex.forms calls prior to making default calls to Glide. This version is for IImageSourceHandler calls.
		/// </summary>
		/// <param name="source">The ImageSource from Xamarin.Forms</param>
		/// <param name="builder">Might be null, the Glide RequestBuilder object to work with. Set to null to cancel the request.</param>
		/// <param name="token">The CancellationToken if you need it</param>
		void Build (ImageSource source, ref RequestBuilder? builder, CancellationToken token) { }
	}
}
