using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Infrastructure.Xaml;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Color = Microsoft.Maui.Graphics.Color;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace DrawnUi.Maui.Draw
{
	/// <summary>
	/// Implements ISkiaGestureListener to pass gestures to children
	/// </summary>
	public partial class SkiaShape
	{
		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
			nameof(CornerRadius),
			typeof(CornerRadius),
			typeof(SkiaShape),
			default(CornerRadius),
			propertyChanged: NeedInvalidateMeasure);

		[System.ComponentModel.TypeConverter(typeof(Microsoft.Maui.Converters.CornerRadiusTypeConverter))]
		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public static readonly BindableProperty StrokePathProperty = BindableProperty.Create(
			nameof(StrokePath),
			typeof(double[]),
			typeof(SkiaShape),
			null);

		[TypeConverter(typeof(StringToDoubleArrayTypeConverter))]
		public double[] StrokePath
		{
			get { return (double[])GetValue(StrokePathProperty); }
			set { SetValue(StrokePathProperty, value); }
		}

		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
			nameof(StrokeColor),
			typeof(Color),
			typeof(SkiaShape),
			Colors.Transparent,
			propertyChanged: NeedDraw);

		public Color StrokeColor
		{
			get { return (Color)GetValue(StrokeColorProperty); }
			set { SetValue(StrokeColorProperty, value); }
		}

	}
}
