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

	public enum PresentationType
	{
		SingleDate,
		DatesRange
	}

	public partial class DrawnMonth : SkiaLayout
	{

		public static bool UseTimeIntervals = true;
		public static PresentationType PresentationType = PresentationType.SingleDate;

		int _SelectedYear;
		int _SelectedMonth;
		readonly SkiaLayout _layoutHeader;
		readonly SkiaLayout _layoutDaysOfWeek;
		readonly SkiaLayout _layoutDays;
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

		
		public static Color ColorAccent = Colors.Goldenrod;
		public static Color ColorTextActive = Colors.White;
		public static Color ColorText = Colors.DimGrey;
		public static Color ColorGrid = Colors.Gainsboro;
		public static Color ColorBackground = Colors.WhiteSmoke;
		public static Color ColorLines = Colors.DarkGrey;
		public static Color ColorCellBackground = Colors.White;

		public static float DayCornerRadius = 14;
		public double DayHeight = 44;


		public DrawnMonth()
		{

			Type = LayoutType.Column;
			Spacing = 0;
			BackgroundColor = Colors.WhiteSmoke;

			//We have 2 main parts, a header and days.

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
							HorizontalOptions = LayoutOptions.CenterAndExpand,
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
							var cell = new DrawnDay()
							{
								HeightRequest = DayHeight,
								CornerRadius = DayCornerRadius
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



			var context = new BookableShopElement(new ShopElement(), null);

			var testData = AppoHelper.GetDefaultConstraints();

			var intervals =
				AppoHelper.GetAvailable(DateTime.Now.AddDays(-90), DateTime.Now, testData, TimeSpan.FromHours(2));

			var booked = new List<BookableInterval>();
			foreach (var interval in intervals)
			{
				var add = new BookableInterval()
				{
					Id = Guid.NewGuid().ToString("N")
				};
				Reflection.MapProperties(interval, add);
				booked.Add(add);
			}

			context.Element.Schedules = new OptionsList<BookableInterval>(booked);
			context.InitIntervals();

			Context = context; //new BookableShopElement(new ShopElement(), null);

			BindingContext = Context;

			SetupTime();
		}

		public static string Lang = "ru";
		public static string PricesMask = "{0} руб.";


		#region CONTEXT

		public BookableShopElement Context { get; set; }

		ObservableRangeCollection<AppoDay> _Days = new();
		public ObservableRangeCollection<AppoDay> Days
		{
			get { return _Days; }
			set
			{
				if (_Days != value)
				{
					_Days = value;
					OnPropertyChanged("Days");
				}
			}
		}

		ObservableRangeCollection<FullBookableInterval> _DaylyIntervals = new();

		public ObservableRangeCollection<FullBookableInterval> DaylyIntervals
		{
			get { return _DaylyIntervals; }
			set
			{
				if (_DaylyIntervals != value)
				{
					_DaylyIntervals = value;
					OnPropertyChanged();
				}
			}
		}



		public int SelectedMonth
		{
			get { return _SelectedMonth; }
			set
			{
				if (_SelectedMonth != value)
				{
					_SelectedMonth = value;
					OnPropertyChanged();
					OnPropertyChanged("SelectedMonthDesc");
				}
			}
		}

		public int SelectedYear
		{
			get { return _SelectedYear; }
			set
			{
				if (_SelectedYear != value)
				{
					_SelectedYear = value;
					OnPropertyChanged();
					OnPropertyChanged("SelectedMonthDesc");
				}
			}
		}

		#endregion

		void OnTappedDay(AppoDay day)
		{
			if (day == null || day.Disabled)
			{
				return;
			}

			Context.SelectDay(day);
			
			InitSeatsPicker();
		}

		void InitSeatsPicker()
		{
			//todo

			/*
			PickerSeats.Items.Clear();

			var b = 13;
			if (Context.CurrentIntervalSeats > 0)
			{
				if (b > Context.CurrentIntervalSeats + 1)
					b = Context.CurrentIntervalSeats + 1;

				for (int a = 1; a < b; a++)
				{
					PickerSeats.Items.Add(a.ToString());
				}
			}
			else
			if (Context.CurrentIntervalSeats == -1) //Many
			{
				for (int a = 1; a < b; a++)
				{
					PickerSeats.Items.Add(a.ToString());
				}
			}

			// -2 or 0 = not available

			if (!PickerSeats.Items.Any())
			{
				PickerSeats.Items.Add(ResStrings.Unavalable);
			}
			*/

		}

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

		void SetupTime()
		{

			//Context.InitIntervals();

			var ci = CultureInfo.CreateSpecificCulture(Lang);
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

			_gridDays.BindingContext = Context;
			
			_labelMonth.Text = Context.CurrentMonthDesc;

			SetupCalendarArrows();
			//gridIntervals.BindingContext = Context;
		}

		protected void SetupCalendarArrows()
		{
			if (Context.Days.Any())
			{
				var startingdow = (int)Context.Days.First().Date.DayOfWeek;
				//if (startingdow == 0)
				//    gridDays.StartColumn = 6;
				//else
				//if (startingdow > 1)
				//    gridDays.StartColumn = startingdow - 1;
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

			_gridDays.ItemsSource = Context.Days;
			//	
		}



	}
}
