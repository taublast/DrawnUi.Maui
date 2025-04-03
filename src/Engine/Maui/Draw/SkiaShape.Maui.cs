using DrawnUi.Draw;
using DrawnUi.Infrastructure.Xaml;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Color = Microsoft.Maui.Graphics.Color;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace DrawnUi.Draw
{
    public partial class SkiaShape
	{
		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
			nameof(CornerRadius),
			typeof(CornerRadius),
			typeof(SkiaShape),
			default(CornerRadius),
			propertyChanged: NeedInvalidateMeasure);

		/// <summary>
		/// Gets or sets the corner radius for the shape when Type is Rectangle.
		/// </summary>
		/// <remarks>
		/// You can specify different corner radii for each corner using the format 
		/// "topLeft,topRight,bottomLeft,bottomRight". For equal corner radius on all 
		/// corners, just provide a single value.
		/// 
		/// In XAML, this can be set using string values that will be automatically converted:
		/// - Single value: "10" (all corners have radius 10)
		/// - Multiple values: "10,20,15,5" (each corner has its own radius)
		/// </remarks>
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

		/// <summary>
		/// Gets or sets the dash pattern for the shape's stroke.
		/// </summary>
		/// <remarks>
		/// Allows for creating dashed or dotted lines by specifying an array of numbers
		/// that define the pattern of dashes and gaps:
		/// 
		/// - A pattern like [3,1] creates dashes of length 3 followed by gaps of length 1
		/// - [5,2,1,2] creates a dash of 5, gap of 2, dash of 1, gap of 2, then repeats
		/// - Empty or null array means a solid line with no dashes
		/// 
		/// In XAML, this property can be set with a comma-separated list of values:
		/// StrokePath="3,1" or StrokePath="5,2,1,2"
		/// </remarks>
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

		/// <summary>
		/// Gets or sets the color of the shape's outline stroke.
		/// </summary>
		/// <remarks>
		/// - Default is Transparent (no visible stroke)
		/// - Must be used with a non-zero StrokeWidth to make the stroke visible
		/// - Can use MAUI Color resources and predefined colors
		/// - Can be combined with StrokeGradient for gradient stroke effects
		/// - Can be animated for dynamic effects
		/// 
		/// The stroke is rendered on top of the fill and any child elements.
		/// </remarks>
		public Color StrokeColor
		{
			get { return (Color)GetValue(StrokeColorProperty); }
			set { SetValue(StrokeColorProperty, value); }
		}

	}
}
