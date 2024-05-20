using SkiaSharp;

namespace DrawnUi.Maui.Camera;

public interface ICameraViewRenderer
{
	void TakePicture();

	Task<SKBitmap> TakePictureForSkia();

	void FlashScreen();

	//void AddFileToGallery(string filename);

	Task<bool> SaveJpgStreamToGallery(Stream stream, string filename, double cameraSavedRotation, string exifFromAnotherFilename = null);
}