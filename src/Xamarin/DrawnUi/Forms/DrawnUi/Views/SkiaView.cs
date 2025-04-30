using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace DrawnUi.Views;


public partial class SkiaView : SKCanvasView, ISkiaDrawable
{
	public bool IsHardwareAccelerated => false;

	public SKSurface CreateStandaloneSurface(int width, int height)
	{
		return SKSurface.Create(new SKImageInfo(width, height));
	}

	public Func<SKCanvas, SKRect, bool> OnDraw { get; set; }

	public DrawnView Superview { get; protected set; }

	public void Dispose()
	{
		_surface = null;
		PaintSurface -= OnPaintingSurface;
		Superview = null;
	}

	public SkiaView(DrawnView superview)
	{
		Superview = superview;
		EnableTouchEvents = false;
	}

	public void Disconnect()
	{
		PaintSurface -= OnPaintingSurface;
	}

	bool rendererSet;

	public void Update()
	{
		if (
			Super.EnableRendering && rendererSet && CanvasSize is { Width: > 0, Height: > 0 })
		{
			IsDrawing = true;
			InvalidateSurface();
		}
	}

	protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == "Renderer")
		{
			if (rendererSet)
			{
				//disconnect
				PaintSurface -= OnPaintingSurface;
				Superview?.DisconnectedHandler();
			}
			else
			{
				rendererSet = true;
				//connect
				PaintSurface -= OnPaintingSurface;
				PaintSurface += OnPaintingSurface;
				Superview?.ConnectedHandler();
			}
		}
	}

	SKSurface _surface;
	private DateTime _lastFrame;
	private double _fps;

	public SKSurface Surface
	{
		get
		{
			return _surface;
		}
	}

	public double FPS
	{
		get
		{
			return _reportFps;
		}
	}

	public long FrameTime { get; protected set; }
	private double _fpsAverage;
	private int _fpsCount;
	private long _lastFrameTimestamp;
	private long _nanos;
	private bool _isDrawing;
	static bool maybeLowEnd = true;
	private double _reportFps;

	/// <summary>
	/// Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames.
	/// </summary>
	/// <param name="currentTimestamp">The current timestamp in nanoseconds.</param>
	/// <param name="averageAmount">The number of frames over which to average the FPS. Default is 10.</param>
	void CalculateFPS(long currentTimestamp, int averageAmount = 10)
	{
		// Convert nanoseconds to seconds for elapsed time calculation.
		double elapsedSeconds = (currentTimestamp - _lastFrameTimestamp) / 1_000_000_000.0;
		_lastFrameTimestamp = currentTimestamp;

		double currentFps = 1.0 / elapsedSeconds;

		_fpsAverage = ((_fpsAverage * _fpsCount) + currentFps) / (_fpsCount + 1);
		_fpsCount++;

		if (_fpsCount >= averageAmount)
		{
			_reportFps = _fpsAverage;
			_fpsCount = 0;
			_fpsAverage = 0.0;
		}
	}

	public bool IsDrawing { get; protected set; }


	private void OnPaintingSurface(object sender, SKPaintSurfaceEventArgs paintArgs)
	{
		IsDrawing = true;

		FrameTime = Super.GetCurrentTimeNanos();

		if (Device.RuntimePlatform == Device.Android)
		{
			CalculateFPS(FrameTime);
		}
		else
		{
			CalculateFPS(FrameTime, 60);
		}

		if (OnDraw != null && Super.EnableRendering)
		{
			_surface = paintArgs.Surface;
			bool isDirty = OnDraw.Invoke(paintArgs.Surface.Canvas, new SKRect(0, 0, paintArgs.Info.Width, paintArgs.Info.Height));

			if (Device.RuntimePlatform == Device.Android)
			{
				if (maybeLowEnd && FPS > 120)
				{
					maybeLowEnd = false;
				}

				if (maybeLowEnd && isDirty && _fps < 60) //kick refresh for low-end devices
				{
					InvalidateSurface();
					return;
				}
			}
		}

		IsDrawing = false;
	}

}
