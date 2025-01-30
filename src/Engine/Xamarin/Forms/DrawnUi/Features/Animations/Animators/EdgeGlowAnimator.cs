namespace DrawnUi.Maui.Draw;

public enum GlowPosition
{
	Top,
	Bottom
}

public class EdgeGlowAnimator : RenderingAnimator
{
	public EdgeGlowAnimator(IDrawnBase control) : base(control)
	{
		IsPostAnimator = true;
		Speed = 500;
		mMinValue = 0;
		mMaxValue = 1;
		Color = SKColor.Parse("#FFFFFF");
		Easing = Easing.CubicIn;
		Height = 50.0;  // Set the initial height of the glow
		GlowPosition = GlowPosition.Top;  // Default to top glow
	}

	protected static long count;

	public SKColor Color { get; set; }
	public double Height { get; set; }
	public double X { get; set; } // X coordinate of the touch point
	public double Y { get; set; } // Y coordinate of the touch point

	public GlowPosition GlowPosition { get; set; }

	protected override bool OnRendering(IDrawnBase control, SkiaDrawingContext context, double scale)
	{
		if (IsRunning)
		{
			var color = Color;
			using (SKPaint paint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				Color = color.WithAlpha((byte)(Height * 5)),  // Modify the alpha based on the height
			})
			{
				void Draw()
				{
					float y = GlowPosition == GlowPosition.Top ? 0 : (float)(control.DrawingRect.Height - Height);
					var rect = new SKRect((float)X, y, (float)(X + Height), (float)(y + Height));
					context.Canvas.DrawRoundRect(rect, 20, 20, paint);
				}

				if (control.ClipEffects)
				{

					using (SKPath clipInsideParent = new SKPath())
					{
						using var clipContent = control.CreateClip(null, true);
						clipContent.Offset((float)(control.TranslationX * scale), (float)(control.TranslationY * scale));
						clipInsideParent.AddPath(clipContent);

						var count = context.Canvas.Save();

						control.ClipSmart(context.Canvas, clipInsideParent);

						Draw();

						context.Canvas.RestoreToCount(count);
					}
				}
				else
				{
					Draw();
				}

				return true;
			}
		}

		return false;
	}

	protected override double TransformReportedValue(long deltaT)
	{
		var progress = base.TransformReportedValue(deltaT);
		Height = 50 * (1 - progress);  // The height shrinks as the progress increases
		return progress;
	}

	// Rest of the code...
}

