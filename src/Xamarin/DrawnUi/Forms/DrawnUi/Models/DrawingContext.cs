using DrawnUi.Views;
using Xamarin.Essentials;

namespace DrawnUi.Draw;

public class SkiaDrawingContext
{
	public SKCanvas Canvas { get; set; }
	public SKSurface Surface { get; set; }
	public float Width { get; set; }
	public float Height { get; set; }
	public long FrameTimeNanos { get; set; }
	public DrawnView Superview { get; set; }
	public string Tag { get; set; }

	/// <summary>
	/// Recording cache
	/// </summary>
	public bool IsVirtual { get; set; }

	/// <summary>
	/// Reusing surface from previous cache
	/// </summary>
	public bool IsRecycled { get; set; }

	public SkiaDrawingContext Clone()
	{
		return new SkiaDrawingContext()
		{
			Superview = Superview,
			Width = Width,
			Height = Height,
			Canvas = this.Canvas,
			Surface = this.Surface,
			FrameTimeNanos = this.FrameTimeNanos,
		};
	}

	public SkiaDrawingContext CreateForRecordingImage(SKSurface surface, SKSize size)
	{
		return new SkiaDrawingContext()
		{
			IsVirtual = true,
			Width = size.Width,
			Height = size.Height,
			Superview = Superview,
			Canvas = surface.Canvas,
			Surface = surface,
			FrameTimeNanos = this.FrameTimeNanos,
		};
	}

	public SkiaDrawingContext CreateForRecordingOperations(SKPictureRecorder recorder, SKRect cacheRecordingArea)
	{
		return new SkiaDrawingContext()
		{
			IsVirtual = true,
			Width = cacheRecordingArea.Width,
			Height = cacheRecordingArea.Height,
			Superview = Superview,
			Canvas = recorder.BeginRecording(cacheRecordingArea),
			Surface = this.Surface,
			FrameTimeNanos = this.FrameTimeNanos,
		};
	}

	public static float DeviceDensity
	{
		get
		{
			return (float)DeviceDisplay.MainDisplayInfo.Density;
		}
	}




}
