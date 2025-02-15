using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppoMobi;
using AppoMobi.Maui.Gestures;
using AppoMobi.Models;
using AppoMobi.Specials;
using AppoMobi.Touch;
using AppoMobi.Xam;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace TestCalendar.Drawn
{

	/// <summary>
	/// Presents a single switchable month
	/// </summary>
	public partial class DrawnMonthView : SkiaLayout
	{

		public DynamicOptions Settings { get; set; } = new();

		public IDrawnDaysController Context
		{
			get
			{
				return BindingContext as IDrawnDaysController;
			}
		}

		protected void SetupCulture(string lang)
		{
			if (Context != null)
			{
				Context.SetupCulture(lang);
			}
		}

		#region PROPERTIES

		public static readonly BindableProperty LangProperty = BindableProperty.Create(
			nameof(Lang),
			typeof(string),
			typeof(DrawnMonthView),
			"en", propertyChanged: (b, o, n) =>
			{
				if (b is DrawnMonthView control)
				{
					control.SetupCulture(control.Lang);
				}
			});

		/// <summary>
		/// Can localize
		/// </summary>
		public string Lang
		{
			get { return (string)GetValue(LangProperty); }
			set { SetValue(LangProperty, value); }
		}

		public static readonly BindableProperty RangeEnabledProperty = BindableProperty.Create(
			nameof(RangeEnabled),
			typeof(bool),
			typeof(DrawnMonthView),
			false);

		/// <summary>
		/// Whether can allow to select days range instead of single day
		/// </summary>
		public bool RangeEnabled
		{
			get { return (bool)GetValue(RangeEnabledProperty); }
			set { SetValue(RangeEnabledProperty, value); }
		}


		#endregion

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			Build();
		}

		static void NeedRebuild(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (bindable is DrawnMonthView control)
			{
				control.Build();
			}
		}

		public static readonly BindableProperty ColorAccentProperty = BindableProperty.Create(
			nameof(ColorAccent),
			typeof(Color),
			typeof(DrawnMonthView),
			Colors.Red, propertyChanged: NeedRebuild);

		public Color ColorAccent
		{
			get { return (Color)GetValue(ColorAccentProperty); }
			set { SetValue(ColorAccentProperty, value); }
		}

		public static readonly BindableProperty DayCornerRadiusProperty = BindableProperty.Create(
			nameof(DayCornerRadius),
			typeof(CornerRadius),
			typeof(DrawnMonthView),
			default(CornerRadius), propertyChanged: NeedRebuild);

		public CornerRadius DayCornerRadius
		{
			get { return (CornerRadius)GetValue(DayCornerRadiusProperty); }
			set { SetValue(DayCornerRadiusProperty, value); }
		}

		public static bool UseTimeIntervals = true;
		public static PresentationType PresentationType = PresentationType.SingleDate;
		SkiaLayout _layoutHeader;
		SkiaLayout _layoutDaysOfWeek;
		SkiaLayout _layoutDays;
		SkiaMarkdownLabel _labelMonth;
		DrawnDataGrid _gridDays;
		SkiaMarkdownLabel cDow0;
		SkiaMarkdownLabel cDow1;
		SkiaMarkdownLabel cDow2;
		SkiaMarkdownLabel cDow3;
		SkiaMarkdownLabel cDow4;
		SkiaMarkdownLabel cDow5;
		SkiaMarkdownLabel cDow6;
		SkiaMarkdownLabel _cLeftArrow;
		SkiaMarkdownLabel _cRightArrow;

		public static Color ColorTextActive = Colors.White;
		public static Color ColorText = Colors.DimGrey;
		public static Color ColorGrid = Colors.Gainsboro;
		public static Color ColorBackground = Colors.WhiteSmoke;
		public static Color ColorLines = Colors.DarkGrey;
		public static Color ColorCellBackground = Colors.White;

		public double DayHeight = 44;

		bool _wasBuilt;


		public void Build()
		{
			if (!_wasBuilt)
			{
				_wasBuilt = true;

				SetupCulture(Lang);

				// MONTH CONTROL
				_layoutHeader = new SkiaLayout()
				{
					Type = LayoutType.Grid,
					HeightRequest = 44,
					ColumnSpacing = 0.5,
					RowSpacing = 0.5,
					Margin = new Thickness(8),
					BackgroundColor = ColorLines,
					HorizontalOptions = LayoutOptions.Fill,
					Children = new List<SkiaControl>()
				{
	                // PREV MONTH
	                new ContentLayout()
					{
						BackgroundColor = ColorBackground,
						UseCache = SkiaCacheType.Operations,
						Margin = new Thickness(1.5, 0.5, 0, 1),
						HorizontalOptions = LayoutOptions.Fill,
						VerticalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "➝",
							Margin = new Thickness(0, 2.5, 0, 0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 18,
							Rotation = 180
						}.With((c) =>
						{
							_cLeftArrow = c;
						})
					}.With((c) =>
					{
						c.OnGestures = (args, info) =>
						{
							if (args.Type == TouchActionResult.Tapped)
							{
								OnTappedPrevPeriod();
								return c;
							}
							return null;
						};
					}),
	                
	                // MONTH NAME
	                new ContentLayout()
					{
						UseCache = SkiaCacheType.Operations,
						BackgroundColor = ColorBackground,
						Margin = new Thickness(0.5, 1, 0, 1),
						HorizontalOptions = LayoutOptions.Fill,
						VerticalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = string.Empty,
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c)=>
						{
							_labelMonth = c;
						})
					}.WithColumn(1),

	                // NEXT MONTH
	                new ContentLayout()
					{
						UseCache = SkiaCacheType.Operations,
						BackgroundColor = ColorBackground,
						Margin = new Thickness(0.5, 1, 1.5, 1),
						HorizontalOptions = LayoutOptions.Fill,
						VerticalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "➝",
							Margin = new Thickness(0, 0, 0, 2.5),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 18
						}.With((c) =>
						{
							_cRightArrow = c;
						})
					}.WithColumn(2)
					.With((c) =>
					{
						c.OnGestures = (args, info) =>
						{
							if (args.Type == TouchActionResult.Tapped)
							{
								OnTappedNextPeriod();
								return c;
							}
							return null;
						};
					})
				}
				}
				.WithColumnDefinitions("40,*,40");

				// DAYS OF WEEK 
				_layoutDaysOfWeek = new SkiaLayout()
				{
					Type = LayoutType.Row,
					Spacing = 0,
					HorizontalOptions = LayoutOptions.Fill,
					Margin = new Thickness(8, 0, 8, 0),
					ItemTemplate = new DataTemplate(() =>
					{
						var cell = new ContentLayout()
						{
							HorizontalOptions = LayoutOptions.Fill,
							HorizontalFillRatio = 0.142857, // 1/7
							Content = new SkiaMarkdownLabel()
							{
								Margin = new Thickness(0),
								VerticalOptions = LayoutOptions.Center,
								HorizontalOptions = LayoutOptions.Fill,
								HorizontalTextAlignment = DrawTextAlignment.Center,
								FontSize = 13,
								MaxLines = 1,
								FontAttributes = FontAttributes.Bold,
								LineBreakMode = LineBreakMode.NoWrap,
								TextColor = Settings.DayText
							}.With((label) =>
							{
								label.SetBinding(SkiaLabel.TextProperty, new Binding("."));
							})
						};
						return cell;
					})
				};
				
				// CONTENT:

				// DAYS GRID
				_layoutDays = new SkiaLayout()
				{
					BackgroundColor = ColorGrid,
					Margin = new Thickness(8, 0, 8, 8),
					Children = new List<SkiaControl>()
					{
						new DrawnDataGrid()
						{
							BackgroundColor = ColorBackground,
							Columns = 7,
							Margin = new Thickness(1, 1, 0.5, 0.5),
							ColumnSpacing = 0,
							RowSpacing = 0,
							ItemTemplate = new DataTemplate(() =>
							{
								var cell = new DrawnDay(Settings)
								{
									//HeightRequest = DayHeight,
									//HorizontalOptions = LayoutOptions.Center,
									//CornerRadius = DayCornerRadius
								}
								.With((c) =>
								{
									c.OnGestures = (args, info) =>
									{
										if (args.Type == TouchActionResult.Tapped)
										{
											OnTappedDay(c.BindingContext as AppoDay);
											return c;
										}
										return null;
									};
								});

								cell.SetBinding(DrawnDay.InsideRangeProperty, new Binding(nameof(Context.RangeSelected), source: Context));

								return cell;
							})
						}.With((c) =>
						{
							_gridDays = c;
						})
					}
				};

				Children.Add(_layoutHeader);
				Children.Add(_layoutDaysOfWeek);
				Children.Add(_layoutDays);
			}

			if (BindingContext != null)
			{
				SetupTime();
			}
		}

		public DrawnMonthView()
		{
			Type = LayoutType.Column;
			Spacing = 0;
			BackgroundColor = Colors.WhiteSmoke;
			
			Build();
		}

		void OnTappedDay(AppoDay day)
		{
			if (day == null || day.Disabled)
			{
				return;
			}

			if (Context.IsSelecting)
			{
				Context.SelectEnd(day);
			}
			else
			{
				Context.SelectStart(day);
			}
		}

		#region SINGLE MONTH WRAPPER

		void OnClicked_Interval(object sender, TapEventArgs e)
		{
			/*
			  Context.SelectInterval((FullBookableInterval)((CellTimeSlot)sender).BindingContext);

			  Context.RaiseProperties(
				  "CurrentIntervalSeats",
				  "SelectedTimeDesc",
				  "CurrentIntervalSeatsDesc",
				  "BookingDateDesc",
				  "BookingDateTimeDesc",
				  "BookingTimeDesc",
				  "Element"); //new very important

			  InitPicker();
			  */
		}

		void OnTappedNextPeriod()
		{
			if (Context.CanSelectNextMonth)
			{
				Context.SelectNextMonth();
				SetupCalendarArrows();
			}
		}

		void OnTappedPrevPeriod()
		{
			if (Context.CanSelectPrevMonth)
			{
				Context.SelectPrevMonth();
				SetupCalendarArrows();
			}
		}

		Color _cLeftArrowColor { get; set; } = ColorText;
		Color _cRightArrowColor { get; set; } = ColorText;

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "MonthSelected")
			{
				SetupCalendarArrows();
			}
			else
			if (e.PropertyName == "IntervalsReady")
			{
				SetupTime();
			}
		}

		#endregion

		void SetupTime()
		{

			_layoutDaysOfWeek.ItemsSource = Context.GetWeekDaysShortNames();
		
			//Context.InitIntervals();

			//_gridDays.BindingContext = Context;
			
			_labelMonth.Text = Context.CurrentMonthDesc;

			SetupCalendarArrows();
			//gridIntervals.BindingContext = Context;
		}

		/// <summary>
		/// Days itemssource is set here.
		/// </summary>
		protected void SetupCalendarArrows()
		{
			if (Context.CurrentMonth != null)
			{
				if (Context.CurrentMonth.Days.Any())
				{
					var startingdow = (int)Context.CurrentMonth.Days.First().Date.DayOfWeek;
					if (startingdow == 0)
						_gridDays.StartColumn = 6;
					else
					if (startingdow > 1)
						_gridDays.StartColumn = startingdow - 1;
				}

				//left arrow
				if (Context.CanSelectPrevMonth)
				{
					_cLeftArrow.TextColor = _cLeftArrowColor;
				}
				else
				{
					_cLeftArrow.TextColor = ColorLines;
				}
				//right arrow
				if (Context.CanSelectNextMonth)
				{
					_cRightArrow.TextColor = _cRightArrowColor;
				}
				else
				{
					_cRightArrow.TextColor = ColorLines;
				}

				_labelMonth.Text = Context.CurrentMonthDesc;

				_gridDays.ItemsSource = Context.CurrentMonth.Days;
			}

			//	
		}



	}
}
