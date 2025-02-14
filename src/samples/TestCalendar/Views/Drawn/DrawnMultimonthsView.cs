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

		public DrawnMultimonthsView()
		{
			Type = LayoutType.Column;
			Spacing = 0;
			BackgroundColor = Colors.WhiteSmoke;

			Build();
		}

		public IDrawnDaysController Context
		{
			get
			{
				return BindingContext as IDrawnDaysController;
			}
		}

		#region PROPERTIES

		public static readonly BindableProperty LangProperty = BindableProperty.Create(
			nameof(Lang),
			typeof(string),
			typeof(DrawnMultimonthsView),
			"en", propertyChanged: (b, o, n) =>
			{
				if (b is DrawnMultimonthsView control)
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
			typeof(DrawnMultimonthsView),
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
		public static PresentationType PresentationType = PresentationType.SingleDate;
		SkiaLayout _layoutHeader;
		SkiaLayout _layoutDaysOfWeek;
		SkiaLayout _layoutDays;
		SkiaMarkdownLabel _labelMonth;
		//DrawnDataGrid _gridDays;
		SkiaMarkdownLabel cDow0;
		SkiaMarkdownLabel cDow1;
		SkiaMarkdownLabel cDow2;
		SkiaMarkdownLabel cDow3;
		SkiaMarkdownLabel cDow4;
		SkiaMarkdownLabel cDow5;
		SkiaMarkdownLabel cDow6;
		SkiaMarkdownLabel _cLeftArrow;
		SkiaMarkdownLabel _cRightArrow;
		SkiaScroll _layoutScroll;
		SkiaLayout _stackMonths;

		bool _wasBuilt;
		protected CultureInfo Culture;

		public static Color ColorTextActive = Colors.White;
		public static Color ColorText = Colors.DimGrey;
		public static Color ColorGrid = Colors.Gainsboro;
		public static Color ColorBackground = Colors.White;
		public static Color ColorLines = Colors.DarkGrey;
		public static Color ColorCellBackground = Colors.White;

		public double DayHeight = 44;


		public void Build()
		{
			if (!_wasBuilt && BindingContext != null)
			{
				_wasBuilt = true;

				// MONTH CONTROL
				_layoutHeader = new SkiaLayout()
				{
					Type = LayoutType.Grid,
					HeightRequest = 44,
					ColumnSpacing = 0.5,
					RowSpacing = 0.5,
					Margin = new Thickness(8),
					BackgroundColor = ColorBackground,
					HorizontalOptions = LayoutOptions.Fill,
					Children = new List<SkiaControl>()
				{
	                // PREV MONTH
	                new ContentLayout()
					{
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
					Type = LayoutType.Grid,
					ColumnSpacing = 0.5,
					RowSpacing = 0,
					Margin = new Thickness(8, 0, 8, 0),
					Children = new List<SkiaControl>()
				{
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "пн",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow0 = c;
						})
					}.WithColumn(0),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "вт",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow1 = c;
						})
					}.WithColumn(1),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "ср",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow2 = c;
						})
					}.WithColumn(2),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "чт",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow3 = c;
						})
					}.WithColumn(3),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "пт",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow4 = c;
						})
					}.WithColumn(4),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "сб",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow5 = c;
						})
					}.WithColumn(5),
					new ContentLayout()
					{
						HorizontalOptions = LayoutOptions.Fill,
						Content = new SkiaMarkdownLabel()
						{
							Text = "вс",
							Margin = new Thickness(0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center,
							FontSize = 15,
							TextColor = ColorText
						}.With((c) =>
						{
							cDow6 = c;
						})
					}.WithColumn(6),

				}
				}
				.WithColumnDefinitions("*,*,*,*,*,*,*");

				// CONTENT
				_layoutScroll = new SkiaScroll()
				{
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

							// MONTH TEMPLATE for AppoMonth
							var tpl = new SkiaLayout()
							{
								UseCache = SkiaCacheType.Operations,
								Type = LayoutType.Column,
								Margin = new Thickness(8, 0, 8, 8),
								Children = new List<SkiaControl>()
								{
									//Month Name
									new SkiaMarkdownLabel()
									{
										TextColor =ColorText,
										FontSize = 18,
										UseCache = SkiaCacheType.Operations,
									}.With((c) =>
									{
										c.SetBinding(SkiaLabel.TextProperty, new Binding("Name"));
									}),
									new DaysGrid()
									{
										BackgroundColor = ColorBackground,
										Columns = 7,
										Margin = new Thickness(1, 1, 0.5, 0.5),
										ColumnSpacing = 0,
										RowSpacing = 4,

										//day template
										ItemTemplate = new DataTemplate(() =>
										{
											var cell = new DrawnDay()
												{
													HeightRequest = 40,
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
										c.SetBinding(SkiaLayout.ItemsSourceProperty, new Binding("Days"));
									})
								}
							};
							
							//label.SetBinding(SkiaLabel.TextProperty, new Binding("."));
							return tpl;
						})
					}.With((c) =>
					{
						_stackMonths = c;
						if (c.BindingContext is IDrawnDaysController controller)
						{
							var check = controller.Months;
						}
						c.SetBinding(SkiaLayout.ItemsSourceProperty, new Binding("Months"));
					}));

				Children.Add(_layoutHeader);
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

			//Context.InitIntervals();

			var ci = CultureInfo.CreateSpecificCulture(Context.Lang);
			Thread.CurrentThread.CurrentCulture = ci;
			//DependencyService.Get<ILocalize>().SetLocale(Lang); 
			string[] names = ci.DateTimeFormat.AbbreviatedDayNames;
			cDow0.Text = names[1];
			cDow1.Text = names[2];
			cDow2.Text = names[3];
			cDow3.Text = names[4];
			cDow4.Text = names[5];
			cDow5.Text = names[6];
			cDow6.Text = names[0];

			//_gridDays.BindingContext = Context;
			
			_labelMonth.Text = Context.CurrentMonthDesc;

			SetupCalendarArrows();
			//gridIntervals.BindingContext = Context;
		}

		protected void SetupCulture(string lang)
		{
			Culture = CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);



		}

		/// <summary>
		/// Days itemssource is set here.
		/// </summary>
		protected void SetupCalendarArrows()
		{
			if (Context.CurrentMonth != null)
			{
				//if (Context.CurrentMonth.Days.Any())
				//{
				//	var startingdow = (int)Context.CurrentMonth.Days.First().Date.DayOfWeek;
				//	if (startingdow == 0)
				//		_gridDays.StartColumn = 6;
				//	else
				//	if (startingdow > 1)
				//		_gridDays.StartColumn = startingdow - 1;
				//}

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

				//todo could scroll to item ?
				//_gridDays.ItemsSource = Context.CurrentMonth.Days;
			}

			//	
		}



	}
}
