using System.Diagnostics;
using DrawnUi.Maui.Draw;
using Sandbox;
using SkiaSharp;

namespace AppoMobi.Xam
{


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

    /// <summary>
    /// Neverending scroll
    /// </summary>
    public class MonthsScroll : VirtualScroll
    {
	    protected override void PreparePlane(DrawingContext context, Plane plane)
        {
	        base.PreparePlane(context, plane);

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

		//overrided to created month views
		protected override SkiaLayout GetMeasuredView(int index, SKRect destination, float scale)
        {
	        DateTime monthDate = DateTime.Now.AddMonths(index);

	        var view = CreateMonthView(index, monthDate.Year, monthDate.Month);

	        //must return already measured
			view.Measure(destination.Width / scale, float.PositiveInfinity, scale);

            return view;
        }

        protected SkiaLayout CreateMonthView(int index, int year, int month)
        {
	        //todo whatever, we just working on 
	        var view = new SkiaLayout()
	        {
		        HeightRequest = 30,
		        HorizontalOptions = LayoutOptions.Fill,
		        //BackgroundColor = Colors.Bisque
	        }.WithChildren(new SkiaShape()
	        {
		        Type = ShapeType.Circle,
		        LockRatio = 1,
		        VerticalOptions = LayoutOptions.Fill,
		        HorizontalOptions = LayoutOptions.Center,
		        BackgroundColor = Colors.Aquamarine
	        }.WithContent(new SkiaLabel()
	        {
		        Text = $"{index}",
		        TextColor = Colors.Black,
		        HorizontalOptions = LayoutOptions.Center,
		        VerticalOptions = LayoutOptions.Center
	        }));

	        return view;
        }


	}
}
