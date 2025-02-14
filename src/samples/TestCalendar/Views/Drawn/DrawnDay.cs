using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppoMobi.Models;
using AppoMobi.Specials;
using AppoMobi.Touch;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace TestCalendar.Drawn
{
	
	public partial class DrawnDay : SkiaLayout
	{

		static Color SelectionText = Colors.White;
		static Color SelectionBackground = Colors.Red;
		static Color SelectionStroke = Colors.HotPink;

		SkiaLabel _labelText;
		readonly SkiaShape _shape;
		readonly SkiaShape _selectionBackground;

		public DrawnDay()
		{

			HorizontalOptions = LayoutOptions.Fill;

			_shape = new SkiaShape()
			{
				UseCache = SkiaCacheType.Operations,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				LockRatio = 0,
				MinimumWidthRequest = 40,
				Padding = new(12, 6),
				CornerRadius = 16,
				StrokeWidth = 1.0,
				BackgroundColor = TransparentColor,
				Content = new SkiaLabel()
				{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = DrawTextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				//Margin = new(0.25, 0, 0.25, 0.5),
				//Padding = new(8, 0),
				TextColor = Colors.DimGray,
				FontSize = 15
				}.With((c) =>
				{
					_labelText = c;
				})
			};

			_selectionBackground = new SkiaShape()
			{
				Tag="Sel",
				UseCache = SkiaCacheType.Operations,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Parse("#33FF0000")
			};

			Children = new List<SkiaControl>()
			{
				_selectionBackground,
				_shape
			};


			//InitGesturesForCell(SelectionBox);

			_front = _labelText.TextColor;

			SetupCell();
		}

		public static readonly BindableProperty InsideRangeProperty = BindableProperty.Create(
			nameof(InsideRange),
			typeof(bool),
			typeof(DrawnDay),
			false, propertyChanged: (b, o, n) =>
			{
				if (b is DrawnDay control)
				{
					control.StyleCell(control.BindingContext as AppoDay);
				}
			});

		/// <summary>
		/// For internal use, should be set by controller to indicate that start and end are globally set
		/// </summary>
		public bool InsideRange
		{
			get { return (bool)GetValue(InsideRangeProperty); }
			set { SetValue(InsideRangeProperty, value); }
		}

		protected override void OnBindingContextChanged()
		{
			SetupCell();

			base.OnBindingContextChanged();
		}

		Color _back { get; set; } = SelectionText;
		Color _front { get; set; }= Colors.Transparent;
		
		public override void OnDisposing()
		{
			base.OnDisposing();

			if (BindingContext is AppoDay item)
			{
				try { item.PropertyChanged -= OnItemPropertyChanged; }
				catch (Exception e) { }
			}

			BindingContext = null;
		}

		
		public event EventHandler<TapEventArgs> ItemTapped;
		public async void CallItemTapped(object item, TapEventArgs e)
		{
			//this.Animate(AnimationType.Scale);
			//await Task.Delay(250);
			ItemTapped?.Invoke(item, e);
		}

		void OnTapped_Cell(object sender, TapEventArgs e)
		{
			CallItemTapped(this, e); //сагат
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (BindingContext is AppoDay day)
			{
				if (e.PropertyName.IsEither("Selected", "SelectionStart", "SelectionEnd"))
				{
					StyleCell(day);
				}
			}
		}

		public void SetupCell()
		{
			if (BindingContext is not AppoDay item)
			{
				if (BindingContext != null)
				{
					Super.Log("DrawnDay BindingContext is NOT AppoDay");
				}
				return;
			}

			try { item.PropertyChanged -= OnItemPropertyChanged; }
			catch (Exception e) { }

			item.PropertyChanged += OnItemPropertyChanged;
			
			var ee = new PropertyChangedEventArgs("Selected");
			OnItemPropertyChanged(item, ee);

			StyleCell(item);
		}

		void StyleCell(AppoDay item)
		{
			if (item == null)
			{
				return;
			}

			if (item.Disabled)
			{
				_back = Colors.WhiteSmoke;
				_shape.BackgroundColor = _back;
				_front = Colors.DarkGray;
				_labelText.FontAttributes = FontAttributes.None;
				_selectionBackground.IsVisible = false;
			}
			else
			{
				_front = Colors.DimGray;
				_labelText.FontAttributes = FontAttributes.Bold;
			}

			_labelText.TextColor = _front;
			_labelText.Text = item.DayDesc;

			var dateAndTime = DateTime.Now;
			var today = dateAndTime.Date;

			if (item.Date == today)
			{
				_shape.StrokeColor = SelectionStroke;
			}
			else
			{
				_shape.StrokeColor = Colors.Transparent;
			}

			bool selectShape = false;
			if (item.SelectionStart)
			{
				_selectionBackground.HorizontalPositionOffsetRatio = 1.0;
				_selectionBackground.HorizontalFillRatio = 0.5;
				_selectionBackground.IsVisible = InsideRange;
				selectShape = true; 
			}
			else
			if (item.SelectionEnd)
			{
				_selectionBackground.HorizontalPositionOffsetRatio = 0.0;
				_selectionBackground.HorizontalFillRatio = 0.5;
				_selectionBackground.IsVisible = InsideRange;
				selectShape = true;
			}
			else
			{
				_selectionBackground.HorizontalPositionOffsetRatio = 0;
				_selectionBackground.HorizontalFillRatio = 1.0;

				_selectionBackground.IsVisible = item.Selected && InsideRange;
				selectShape = item.Selected && !InsideRange;
			}

			SetupShapeSelection(selectShape);
		}

		void SetupShapeSelection(bool value)
		{
			if (value)
			{
				_shape.BackgroundColor = SelectionBackground;
				_labelText.TextColor = SelectionText;
			}
			else
			{
				_shape.BackgroundColor = Colors.Transparent;
				_labelText.TextColor = _front;
			}
		}
	}
}
