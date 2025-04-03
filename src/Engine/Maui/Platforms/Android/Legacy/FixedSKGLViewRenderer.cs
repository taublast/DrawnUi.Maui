using Android.Content;
using Android.Opengl;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Compatibility;
using SkiaSharp.Views.Maui.Platform;
using System;
using System;
using SKFormsView = DrawnUi.Views.SkiaViewAccelerated;
using SKNativeView = DrawnUi.SkiaGLTexture;

//[assembly: ExportRenderer(typeof(SKFormsView), typeof(FixedSKGLViewRenderer))]
// =>
// handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(FixedSKGLViewRenderer));
namespace DrawnUi;

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
		var view = GetType() == typeof(SKGLViewRenderer)
			? new SKNativeView(Context)
			: base.CreateNativeControl();

		// Force the opacity to false for consistency with the other platforms
		view.SetOpaque(false);

		return view;
	}
}