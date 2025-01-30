using System.ComponentModel;
using Android.Content;
using Microsoft.Maui.Controls.Platform;
using SKPaintGLSurfaceEventArgs = SkiaSharp.Views.Android.SKPaintGLSurfaceEventArgs;

namespace DrawnUi.Maui;

public abstract class FixedSKGLViewRendererBase<TFormsView, TNativeView> : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<TFormsView, TNativeView>
	where TFormsView : SkiaViewAccelerated
	where TNativeView : SkiaGLTexture
{


	protected FixedSKGLViewRendererBase(Context context)
		: base(context)
	{
		Initialize();
	}

	private void Initialize()
	{

	}

	private bool measured;

	public GRContext GRContext => Control.GRContext;

	public static object lockUpdate = new();

	protected override void OnElementChanged(ElementChangedEventArgs<TFormsView> e)
	{
		if (e.OldElement != null)
		{
			var oldController = (ISKGLViewController)e.OldElement;

			// unsubscribe from events
			oldController.SurfaceInvalidated -= OnSurfaceInvalidated;
			oldController.GetCanvasSize -= OnGetCanvasSize;
			oldController.GetGRContext -= OnGetGRContext;
		}

		if (e.NewElement != null)
		{
			var newController = (ISKGLViewController)e.NewElement;

			// create the native view
			if (Control == null)
			{
				var view = CreateNativeControl();
				view.PaintSurface += OnPaintSurface;
				SetNativeControl(view);
			}

			// subscribe to events from the user
			newController.SurfaceInvalidated += OnSurfaceInvalidated;
			newController.GetCanvasSize += OnGetCanvasSize;
			newController.GetGRContext += OnGetGRContext;

			// start the rendering
			SetupRenderLoop(false);
		}

		base.OnElementChanged(e);
	}


	protected override TNativeView CreateNativeControl()
	{
		return (TNativeView)Activator.CreateInstance(typeof(TNativeView), new[] { Context });
	}


	protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(sender, e);

		// refresh the render loop
		if (e.PropertyName == SkiaViewAccelerated.HasRenderLoopProperty.PropertyName)
		{
			SetupRenderLoop(false);
		}
		else if (e.PropertyName == SkiaViewAccelerated.EnableTouchEventsProperty.PropertyName)
		{

		}

	}

	protected override void Dispose(bool disposing)
	{
		// detach all events before disposing
		var controller = (ISKGLViewController)Element;
		if (controller != null)
		{
			controller.SurfaceInvalidated -= OnSurfaceInvalidated;
			controller.GetCanvasSize -= OnGetCanvasSize;
			controller.GetGRContext -= OnGetGRContext;
		}

		var control = Control;
		if (control != null)
		{
			control.PaintSurface -= OnPaintSurface;
		}



		base.Dispose(disposing);
	}

	protected abstract void SetupRenderLoop(bool oneShot);

	private SKPoint GetScaledCoord(double x, double y)
	{
		return new SKPoint((float)x, (float)y);
	}


	// the user asked to repaint
	private void OnSurfaceInvalidated(object sender, EventArgs eventArgs)
	{
		// if we aren't in a loop, then refresh once
		if (!Element.HasRenderLoop)
		{
			SetupRenderLoop(true);
		}
	}

	// the user asked for the size
	private void OnGetCanvasSize(object sender, GetPropertyValueEventArgs<SKSize> e)
	{
		e.Value = Control?.CanvasSize ?? SKSize.Empty;
	}

	// the user asked for the current GRContext
	private void OnGetGRContext(object sender, GetPropertyValueEventArgs<GRContext> e)
	{
		e.Value = Control?.GRContext;
	}

	private void OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
	{
		lock (lockUpdate)
		{
			var controller = Element as ISKGLViewController;

			// the control is being repainted, let the user know
			controller?.OnPaintSurface(new SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs(e.Surface, e.BackendRenderTarget));
		}
	}
}