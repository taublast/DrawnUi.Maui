namespace DrawnUi.Draw;


public enum SkiaCacheType
{
	/// <summary>
	/// True and old school
	/// </summary>
	None,

	/// <summary>
	/// Create and reuse SKPicture. Try this first for labels, svg etc. 
	/// Do not use this when dropping shadows or with other effects, better use Bitmap. 
	/// </summary>
	Operations,

	/// <summary>
	/// Will use simple SKBitmap cache type, will not use hardware acceleration.
	/// Slower but will work for sizes bigger than graphics memory if needed.
	/// </summary>
	Image,

	/// <summary>
	/// Using `Image` cache type with double buffering. Best for fast animated scenarios, this must be implemented by a specific control, not all controls support this, will fallback to 'Image' if anything.
	/// </summary>
	ImageDoubleBuffered,

	/// <summary>
	/// DO NOT USE in Xamarin!
	/// </summary>
	ImageComposite,

	/// <summary>
	/// The way to go when dealing with images surrounded by shapes etc.
	/// The cached surface will use the same graphic context as your canvas.
	/// If hardware acceleration is enabled will try to cache as Bitmap inside graphics memory. Will fallback to simple Bitmap cache type if not possible. If you experience issues using it, switch to Memory cache type.
	/// </summary>
	GPU,

}