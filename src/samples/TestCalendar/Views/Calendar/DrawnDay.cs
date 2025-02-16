using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
 

		SkiaLabel _labelText;
		readonly SkiaShape _shape;
		readonly SkiaShape _selectionBackground;

		public DynamicOptions Settings;

		public DrawnDay(DynamicOptions settings)
		{
			Settings = settings;

			_back = Settings.DaySelectionText;

			HorizontalOptions = LayoutOptions.Fill;

			_shape = new SkiaShape()
			{
				UseCache = SkiaCacheType.Operations,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				LockRatio = 0,
				MinimumWidthRequest = 40,
				Padding = new(12, 6),
				CornerRadius = Settings.SelectionRadius,
				StrokeWidth = 1.0,
				BackgroundColor = TransparentColor,
				Content = new SkiaLabel()
				{
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = DrawTextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					TextColor = Settings.DayText,
					FontFamily = Settings.DayFont,
					FontSize = Settings.DayTextSize
				}.Adjust((c) =>
				{
					_labelText = c;
				})
			};

			_selectionBackground = new SkiaShape()
			{
				UseCache = SkiaCacheType.Operations,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Settings.DayRangeSelectionBackground
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

		Color _back { get; set; }  
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

		public bool IsLeftUnclosed(AppoDay? item)
		{
			if (item==null || item.SelectionStart)
				return false;

			// first day of the month.
			if (item.Date.Day == 1)
				return true;

			// first day of the week per current culture.
			var culture = Settings.Culture ?? Thread.CurrentThread.CurrentCulture;
			DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
			if (item.Date.DayOfWeek == firstDayOfWeek)
				return true;

			return false;
		}

		public bool IsRightUnclosed(AppoDay? item)
		{
			if (item==null || item.SelectionEnd)
				return false;

			// last day of the month.
			int daysInMonth = DateTime.DaysInMonth(item.Date.Year, item.Date.Month);
			if (item.Date.Day == daysInMonth)
				return true;

			// last day of the week per current culture.
			var culture = Settings.Culture ?? Thread.CurrentThread.CurrentCulture;
			DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
			DayOfWeek lastDayOfWeek = (DayOfWeek)(((int)firstDayOfWeek + 6) % 7);
			if (item.Date.DayOfWeek == lastDayOfWeek)
				return true;

			return false;
		}

		static LinearGradientBrush _leftGradient;
		LinearGradientBrush GetLeftRangeGradient()
		{
			if (_leftGradient == null)
			{
				_leftGradient = new LinearGradientBrush()
				{
					StartPoint = new(0, 0),
					EndPoint = new(1, 0),
					GradientStops = new GradientStopCollection()
					{
						new (Settings.ColorBackground, 0),
						new (Settings.DayRangeSelectionBackground, 0.5f),
						new (Settings.DayRangeSelectionBackground, 1.0f),
					}
				};
			}
			return _leftGradient;
		}

		static LinearGradientBrush _rightGradient;
		LinearGradientBrush GetRightRangeGradient()
		{
			if (_rightGradient == null)
			{
				_rightGradient = new LinearGradientBrush()
				{
					StartPoint = new(1, 0),
					EndPoint = new(0, 0),
					GradientStops = new GradientStopCollection()
					{
						new (Settings.ColorBackground, 0),
						new (Settings.DayRangeSelectionBackground, 0.5f),
						new (Settings.DayRangeSelectionBackground, 1.0f),
					}
				};
			}
			return _rightGradient;
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
				_back = Colors.Transparent;
				_shape.BackgroundColor = _back;
				_front = Settings.DayDisabledText;
				_labelText.FontAttributes = Settings.DayDisabledTextAttributes;
				_selectionBackground.IsVisible = false;
			}
			else
			{
				_front = Settings.DayText;
				_labelText.FontAttributes = Settings.DayTextAttributes;
			}

			_labelText.TextColor = _front;
			_labelText.Text = item.DayDesc;

			var dateAndTime = DateTime.Now;
			var today = dateAndTime.Date;

			bool selectShape = false;
			bool resetShape = true;
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
				selectShape = item.Selected && !InsideRange;
				_selectionBackground.HorizontalPositionOffsetRatio = 0;
				_selectionBackground.HorizontalFillRatio = 1.0;
				var highlight = item.Selected && InsideRange;
				_selectionBackground.IsVisible = highlight;

				bool noBackground = true;
				if (Settings.StyleRangeSelection != RangeSelectionEdgesStyle.None)
				{
					if (!selectShape && highlight)
					{
						if (IsLeftUnclosed(item))
						{
							if (Settings.StyleRangeSelection == RangeSelectionEdgesStyle.Fade)
							{
								noBackground = false;
								_selectionBackground.Background = GetLeftRangeGradient();
							}
							else if (Settings.StyleRangeSelection == RangeSelectionEdgesStyle.Round)
							{
								//todo
								resetShape = false;
								_shape.StrokeColor = Colors.Transparent;
								_shape.BackgroundColor = Settings.DayRangeSelectionBackground;
								_selectionBackground.HorizontalPositionOffsetRatio = 1.0;
								_selectionBackground.HorizontalFillRatio = 0.5;
							}
						}
						else
						if (IsRightUnclosed(item))
						{
							if (Settings.StyleRangeSelection == RangeSelectionEdgesStyle.Fade)
							{
								noBackground = false;
								_selectionBackground.Background = GetRightRangeGradient();
							}
							else if (Settings.StyleRangeSelection == RangeSelectionEdgesStyle.Round)
							{
								//todo
								resetShape = false;
								_shape.StrokeColor = Colors.Transparent;
								_shape.BackgroundColor = Settings.DayRangeSelectionBackground;
								_selectionBackground.HorizontalPositionOffsetRatio = 0.0;
								_selectionBackground.HorizontalFillRatio = 0.5;
							}
						}
					}
				}
				if (noBackground)
				{
					_selectionBackground.Background = null;
				}

			}

			//selected shape
			if (selectShape)
			{
				_shape.BackgroundColor = Settings.DaySelectionBackground;
				_shape.StrokeColor = Settings.DaySelectionStroke;
				_labelText.TextColor = Settings.DaySelectionText;
			}
			else 
			{
				if (resetShape)
				{
					if (item.Date == today)
					{
						_shape.StrokeColor = Settings.DayTodayStroke;
						_shape.BackgroundColor = Settings.DayTodayBackground;
					}
					else
					{
						_shape.StrokeColor = Colors.Transparent;
						_shape.BackgroundColor = Colors.Transparent;
					}
				}
				_labelText.TextColor = _front;
			}
		}
 

	}
}
