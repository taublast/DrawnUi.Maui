using System.Diagnostics;
using DrawnUi.Maui.Draw;
using Sandbox;

namespace AppoMobi.Xam;

public class SmartScroll : SkiaScroll
{
	protected override void PreparePlane(DrawingContext context, Plane plane)
	{
		base.PreparePlane(context, plane);

		return;

		if (plane.Id == "Current")
		{
			MainPage.DebugLayerA.AttachTo = plane.CachedObject;
			Debug.WriteLine("Attached plane Current to debug image");
		}
		else
		if (plane.Id == "Forward")
		{
			MainPage.DebugLayerB.AttachTo = plane.CachedObject;
			Debug.WriteLine("Attached plane Forward to debug image");
		}
		else
		{
			MainPage.DebugLayerC.AttachTo = plane.CachedObject;
			Debug.WriteLine("Attached plane Backward to debug image");
		}
	}
}