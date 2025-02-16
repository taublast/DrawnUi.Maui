using System.ComponentModel;
using System.Globalization;
using AppoMobi.Maui.Gestures;
using AppoMobi.Models;
using AppoMobi.Touch;
using DrawnUi.Maui.Draw;

namespace TestCalendar.Drawn
{
	/// <summary>
	/// Presents a single switchable month
	/// </summary>
	public partial class DrawnMultimonthsView : SkiaLayout
	{

		public DynamicOptions Settings { get; set; } = new();

		public DrawnMultimonthsView()
		{
			Type = LayoutType.Column;
			Spacing = 0;
		}

		protected virtual DynamicOptions CreateSettings()
		{
			return new DynamicOptions();
		}


		protected virtual IDrawnDaysController CreateController()
		{
			return new CalendarController(Settings);
		}

		protected override void CreateDefaultContent()
		{
			base.CreateDefaultContent();

			Settings = CreateSettings();
			_controller = CreateController();
			SetBindingContext();
			Build();
		}

		public void SetBindingContext()
		{
			BindingContext = _controller;
		}

		public IDrawnDaysController Context
		{
			get
			{
				return BindingContext as IDrawnDaysController;
			}
		}

		#region PROPERTIES

		//public static readonly BindableProperty LangProperty = BindableProperty.Create(
		//	nameof(Lang),
		//	typeof(string),
		//	typeof(DrawnMultimonthsView),
		//	"en", propertyChanged: (b, o, n) =>
		//	{
		//		if (b is DrawnMultimonthsView control)
		//		{
		//			control.SetupCulture(control.Lang);
		//		}
		//	});

		///// <summary>
		///// Can localize
		///// </summary>
		//public string Lang
		//{
		//	get { return (string)GetValue(LangProperty); }
		//	set { SetValue(LangProperty, value); }
		//}

		public static readonly BindableProperty RangeEnabledProperty = BindableProperty.Create(
			nameof(RangeEnabled),
			typeof(bool),
			typeof(DrawnMultimonthsView),
			true);

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

			if (BindingContext == null && _context!=null)
			{
				_context.SelectionDatesChanged -= OnDatesChanged;
			}

			Build();
		}

		void OnDatesChanged(object? sender, (DateTime? Start, DateTime? End) e)
		{
			SelectionDatesChanged?.Invoke(this, e);
		}

		public event EventHandler<(DateTime? Start, DateTime? End)>? SelectionDatesChanged;

		static void NeedRebuild(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (bindable is DrawnMultimonthsView control)
			{
				control.Build();
			}
		}

		public static readonly BindableProperty ColorAccentProperty = BindableProperty.Create(
			nameof(ColorAccent),
			typeof(Color),
			typeof(DrawnMultimonthsView),
			Colors.Red, propertyChanged: NeedRebuild);

		public Color ColorAccent
		{
			get { return (Color)GetValue(ColorAccentProperty); }
			set { SetValue(ColorAccentProperty, value); }
		}

		public static readonly BindableProperty DayCornerRadiusProperty = BindableProperty.Create(
			nameof(DayCornerRadius),
			typeof(CornerRadius),
			typeof(DrawnMultimonthsView),
			default(CornerRadius), propertyChanged: NeedRebuild);

		public CornerRadius DayCornerRadius
		{
			get { return (CornerRadius)GetValue(DayCornerRadiusProperty); }
			set { SetValue(DayCornerRadiusProperty, value); }
		}

		public static bool UseTimeIntervals = true;
 
		SkiaLayout _layoutHeader;
		SkiaLayout _layoutDaysOfWeek;
		SkiaLayout _layoutDays;
		SkiaMarkdownLabel _labelMonth;
		SkiaScroll _layoutScroll;
		SkiaLayout _stackMonths;

		bool _wasBuilt;
		protected CultureInfo Culture;
		IDrawnDaysController _context;
		IDrawnDaysController _controller;

		public void Build()
		{
			if (!_wasBuilt && BindingContext != null)
			{
				_wasBuilt = true;

				//SetupCulture(Lang);

				_context = Context;

				_context.SelectionDatesChanged += OnDatesChanged;

				BackgroundColor = Settings.ColorBackground;

				// DAYS OF WEEK 
				_layoutDaysOfWeek = new SkiaLayout()
				{
					Type = LayoutType.Row,
					Spacing = 0,
					HorizontalOptions = LayoutOptions.Fill,
					Padding = new(0, 8),
					Margin = new Thickness(8, 0, 8, 0),
					ItemTemplate = new DataTemplate(() =>
					{
						var cell = new SkiaLayout()
						{
							HorizontalOptions = LayoutOptions.Fill,
							HorizontalFillRatio = 0.142857, // 1/7
							Children = new List<SkiaControl>()
							{
								new SkiaMarkdownLabel()
								{
									Margin = new Thickness(0),
									VerticalOptions = LayoutOptions.Center,
									HorizontalOptions = LayoutOptions.Fill,
									HorizontalTextAlignment = DrawTextAlignment.Center,
									FontSize = Settings.WeekDayTextSize,
									FontFamily = Settings.WeekDayFont,
									FontAttributes = FontAttributes.Bold,
									TextColor = Settings.ColorWeekDay
								}.Adjust((label) =>
								{
									label.SetBinding(SkiaLabel.TextProperty, new Binding("."));
								})
							}
						};
						return cell;
					})
				};

				// CONTENT
				_layoutScroll = new SkiaScroll()
				{
					FrictionScrolled = 0.95f,
					//ChangeDistancePanned = 0.9f,
					HorizontalOptions = LayoutOptions.Fill,
					MinimumHeightRequest = 100
				}
				.WithContent(new SkiaLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Type = LayoutType.Column,
						Spacing = 4,
						ItemTemplate = new DataTemplate(() =>
						{
							var tpl = new SkiaLayout()
							{
								UseCache = SkiaCacheType.ImageDoubleBuffered,
								Type = LayoutType.Column,
								Spacing = 12,
								Margin = new Thickness(8, 16, 8, 0),
								Children = new List<SkiaControl>()
								{
									//Month Name
									new SkiaLabel()
									{
										Margin = new Thickness(8, 0, 8, 8),
										FontFamily = Settings.MonthNameFont,
										TextColor =Settings.ColorMonthName,
										FontSize = Settings.MonthTextSize,
										FontAttributes = FontAttributes.Bold,
										UseCache = SkiaCacheType.Operations,
									}.Adjust((c) =>
									{
										// type is AppoMonth
										c.SetBinding(SkiaLabel.TextProperty, new Binding("Name"));
									}),
									new DaysGrid()
									{
										BackgroundColor = Settings.ColorDaysBackground,
										Columns = 7,
										Margin = new Thickness(1, 1, 0.5, 0.5),
										ColumnSpacing = 0,
										RowSpacing = 4,

										//day template
										ItemTemplate = new DataTemplate(() =>
										{
											var cell = new DrawnDay(Settings)
												{
													HeightRequest = 40,
													//HorizontalOptions = LayoutOptions.Center,
													//CornerRadius = DayCornerRadius
												}
												.Adjust((c) =>
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
									}.Adjust((c) =>
									{
										c.SetBinding(SkiaLayout.ItemsSourceProperty, new Binding("Days"));
									})
								}
							};
							
							//label.SetBinding(SkiaLabel.TextProperty, new Binding("."));
							return tpl;
						})
					}.Adjust((c) =>
					{
						_stackMonths = c;
						if (c.BindingContext is IDrawnDaysController controller)
						{
							var check = controller.Months;
						}
						c.SetBinding(SkiaLayout.ItemsSourceProperty, new Binding("Months"));
					}));

				//Children.Add(_layoutHeader);
				Children.Add(_layoutDaysOfWeek);
				Children.Add(_layoutScroll);
			}

			if (BindingContext != null)
			{
				SetupTime();
			}
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

			SetupCalendarArrows();
		}

		/// <summary>
		/// Days itemssource is set here.
		/// </summary>
		protected void SetupCalendarArrows()
		{
			 
		}



	}
}
