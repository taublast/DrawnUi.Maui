using Android.Content;
using Android.Opengl;
using AppoMobi.Xamarin.DrawnUi.Droid;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using SKFormsView = DrawnUi.Views.SkiaViewAccelerated;//SkiaSharp.Views.Forms.SKGLView;
using SKNativeView = AppoMobi.Xamarin.DrawnUi.Droid.Native.SkiaGLTexture;

[assembly: ExportRenderer(typeof(SKFormsView), typeof(FixedSKGLViewRenderer))]
namespace AppoMobi.Xamarin.DrawnUi.Droid;

public class FixedSKGLViewRenderer : FixedSKGLViewRendererBase<SKFormsView, SKNativeView>
{
	public FixedSKGLViewRenderer(Context context)
		: base(context)
	{
	}

	protected override void SetupRenderLoop(bool oneShot)
	{
		if (oneShot)
		{
			Control.RequestRender();
		}

		Control.RenderMode = Element.HasRenderLoop
			? Rendermode.Continuously
			: Rendermode.WhenDirty;
	}

	protected override SKNativeView CreateNativeControl()
	{
		var view = GetType() == typeof(FixedSKGLViewRenderer)
			? new SKNativeView(Context)
			: base.CreateNativeControl();

		// Force the opacity to false for consistency with the other platforms
		view.SetOpaque(false);

		return view;
	}
}