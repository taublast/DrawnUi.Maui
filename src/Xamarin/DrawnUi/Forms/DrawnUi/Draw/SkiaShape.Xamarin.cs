using AppoMobi.Specials;
using DrawnUi.Infrastructure.Xaml;
using SkiaSharp.Views.Forms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DrawnUi.Draw
{
	/// <summary>
	/// Implements ISkiaGestureListener to pass gestures to children
	/// </summary>
	public partial class SkiaShape : ContentLayout, ISkiaGestureListener
	{
		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
			nameof(CornerRadius),
			typeof(CornerRadius),
			typeof(SkiaShape),
			default(CornerRadius),
			propertyChanged: NeedInvalidateMeasure);

		[System.ComponentModel.TypeConverter(typeof(CornerRadiusTypeConverter))]
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

		[System.ComponentModel.TypeConverter(typeof(StringToDoubleArrayTypeConverter))]
		public double[] StrokePath
		{
			get { return (double[])GetValue(StrokePathProperty); }
			set { SetValue(StrokePathProperty, value); }
		}

		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
			nameof(StrokeColor),
			typeof(Color),
			typeof(SkiaShape),
			Color.Transparent,
			propertyChanged: NeedDraw);

		public Color StrokeColor
		{
			get { return (Color)GetValue(StrokeColorProperty); }
			set { SetValue(StrokeColorProperty, value); }
		}


	}
}
