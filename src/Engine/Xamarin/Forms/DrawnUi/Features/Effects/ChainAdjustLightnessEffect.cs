using DrawnUi.Maui.Infrastructure.Extensions;

namespace DrawnUi.Maui.Draw;

public class ChainAdjustLightnessEffect : BaseChainedEffect
{
	public static readonly BindableProperty ValueProperty = BindableProperty.Create(
		nameof(Value),
		typeof(float),
		typeof(ChainAdjustBrightnessEffect),
		1f,
		propertyChanged: NeedUpdate);

	public float Value
	{
		get => (float)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public override ChainEffectResult Draw(SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
	{
		if (NeedApply)
		{
			if (Paint == null)
			{
				Paint = new()
				{
					ColorFilter = CreateLightnessFilter(Value)
				};
			}

			ctx.Canvas.SaveLayer(Paint);

			drawControl(ctx);

			return ChainEffectResult.Create(true);
		}

		return base.Draw(destination, ctx, drawControl);
	}

	/// <summary>
	/// -1 -> 0 -> 1
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static SKColorFilter CreateLightnessFilter(float value)
	{
		var brightness = DrawnExtensions.Clamp(value - 1, -1f, 1f);

		float b = brightness;
		float[] colorMatrix = {
			1, 0, 0, 0, b,
			0, 1, 0, 0, b,
			0, 0, 1, 0, b,
			0, 0, 0, 1, 0
		};

		return SKColorFilter.CreateColorMatrix(colorMatrix);
	}



	public override bool NeedApply => base.NeedApply && Value != 1f;
}