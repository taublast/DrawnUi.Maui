using System.Globalization;
using AppoMobi.Models;
using AppoMobi.Specials;
using AppoMobi.Xam;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace TestCalendar.Drawn;

/// <summary>
/// Calendar control ViewModel
/// </summary>
public class CalendarController : BindableObject, IDrawnDaysController
{

	//todo
	IEnumerable<AppoDay> SelectedMonthDays { get; }

	public CalendarController()
	{
		var testData = AppoHelper.GetDefaultConstraints();
		
		var intervals =
			AppoHelper.GetAvailable(DateTime.Now.AddDays(90), DateTime.Now, testData, TimeSpan.FromHours(6));

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
		var element = new ShopElement()
		{
			Schedules = new OptionsList<BookableInterval>(booked)
		};

		var context = new BookableShopElement(element, null, false);

		Context = context;      
		
		//add 1 year
		var startMonth = DateTime.Now.Month;
		var startYear = DateTime.Now.Year;
		Context.SetupDays(startYear, startMonth, startYear+1, startMonth);

		AppoMonth? current = null;
		foreach (var monthInfo in Context.DaysContainer.Indexes)
		{
			var month = new AppoMonth(monthInfo.Year, monthInfo.Month);
			var days = Context.DaysContainer.GetDaysForMonth(monthInfo.Id);
			month.Days = days;

			Months.Add(month); 
			
			if (current == null)
			{
				current = month;
			}
		}
		CurrentMonth = current;
		
	}

	protected CultureInfo Culture;

	public void SetupCulture(string lang)
	{
		Culture = CultureInfo.CreateSpecificCulture(lang);
		CultureInfo.CurrentCulture = Culture;
		Thread.CurrentThread.CurrentCulture = Culture;
		Thread.CurrentThread.CurrentUICulture = Culture;
	}

	public List<string> WeekDaysNames { get; set; } = new();

	public List<string> GetWeekDaysShortNames()
	{
		var names = Culture.DateTimeFormat.AbbreviatedDayNames;
		bool startsOnSunday = Culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday;

		if (!startsOnSunday)
		{
			string sunday = names[0];
			for (int i = 0; i < names.Length - 1; i++)
			{
				names[i] = names[i + 1];
			}
			names[names.Length - 1] = sunday;
		}

		WeekDaysNames = names.ToList();

		return WeekDaysNames;
	}

	bool RangeEnabled => true;

	public string Lang => BookableShopElement.Lang;

	public string PricesMask => "{0} руб.";


	#region Selection

	public void SelectDay(AppoDay day)
	{
		if (day == null)
		{
			Context.DaysContainer.SelectDay(null);
			return;
		}

		//in case this day reference is  not inside Days
		day.Selected = true;
		day.SelectionStart = true;
		day.SelectionEnd = false;

		//affects UI
		Context.DaysContainer.SelectDay(day);

		//todo!!!
		//LoadFullDaylyIntervals();
		
		//OnPropertyChanged("SelectedDateTypeDesc");
		//OnPropertyChanged("SelectedDateDesc");
		//OnPropertyChanged("FullTitle");
	}

	AppoDay? _dayStart;
	AppoDay? DayStart
	{
		get
		{
			return _dayStart;
		}
		set
		{
			if (_dayStart != value)
			{
				_dayStart = value;
				OnPropertyChanged();
				RaiseRangeProperties();
			}
		}
	}

	AppoDay? _dayEnd;
	AppoDay? DayEnd
	{
		get
		{
			return _dayEnd;
		}
		set
		{
			if (_dayEnd != value)
			{
				_dayEnd = value;
				OnPropertyChanged();
				RaiseRangeProperties();
			}
		}
	}

	void RaiseRangeProperties()
	{
		OnPropertyChanged(nameof(IsSelecting));
		OnPropertyChanged(nameof(RangeSelected));
	}

	public virtual void SelectStart(AppoDay? value)
	{
		DayEnd = null;
		if (value == DayStart)
		{
			RaiseRangeProperties();
		}
		else
		{
			DayStart = value;
		}

		InitDaySelection(value);
	}

	void InitDaySelection(AppoDay value)
	{
		SelectDay(value);
		InitSeatsPicker();
	}

	public virtual void SelectEnd(AppoDay? value)
	{
		if (value == DayStart)
			return;

		var end = value;
		var start = DayStart;

		if (start != null)
		{
			var selected = SelectDays(start, end);
			if (selected.Start > 0)
			{
				DayEnd = end;
			}
		}
	}

	public (int Start, int End) SelectDays(AppoDay start, AppoDay end)
	{
		//in case this day reference is  not inside Days
		//todo re-enable
		//if (start != null)
		//{
		//	start.Selected = true;
		//	start.SelectionStart = true;
		//	start.SelectionEnd = false;
		//}
		//if (end != null)
		//{
		//	end.Selected = true;
		//	end.SelectionStart = false;
		//	end.SelectionEnd = true;
		//}

		//affects UI
		var indexes = Context.DaysContainer.SelectDays(start, end);

		//todo
		//LoadFullDaylyIntervals();
		//OnPropertyChanged("SelectedDateTypeDesc");
		//OnPropertyChanged("SelectedDateDesc");
		//OnPropertyChanged("FullTitle");

		SendSelectionChanged();

		return indexes;
	}

	public event EventHandler<IEnumerable<AppoDay>>? SelectionChanged;

	public IEnumerable<AppoDay> GetSelection()
	{
		if (Context == null || Context.DaysContainer == null)
		{
			return new List<AppoDay>().AsReadOnly();
		}
		return Context.DaysContainer.Days.Where(x => x.Selected).ToList().AsReadOnly();
	}

	public void SendSelectionChanged()
	{
		if (SelectionChanged != null)
		{
			SelectionChanged.Invoke(this, GetSelection());
		}
	}

	/// <summary>
	/// Is in range mode and have first date already selected, waiting to select ending date
	/// </summary>
	public bool IsSelecting => RangeEnabled && DayStart != null && DayEnd == null;

	public bool CanSelectNextMonth
	{
		get
		{
			if (CurrentMonth == null)
				return false;

			var next = CurrentMonth.GetNextMonthId();
			return Context.DaysContainer.ContainsMonth(next);
		}
	}

	public bool CanSelectPrevMonth
	{
		get
		{
			if (CurrentMonth == null)
				return false;

			var next = CurrentMonth.GetPreviousMonthId();
			return Context.DaysContainer.ContainsMonth(next);
		}
	}

	public string CurrentMonthDesc
	{
		get
		{
			if (CurrentMonth == null)
				return string.Empty;

			var time = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
			var desc = time.ToString("MMMM yyyy").ToTitleCase();
			return desc;
		}
	} 

	public void SelectNextMonth()
	{
		if (CurrentMonth == null)
			return;

		var monthInfo = CurrentMonth.GetNextMonthInfo();
		if (Context.DaysContainer.ContainsMonth(monthInfo.Id))
		{
			var month = Months
				.FirstOrDefault(x => x.Year == monthInfo.Year && x.Month == monthInfo.Month);

			if (month == null)
			{
				month = new AppoMonth(monthInfo.Year, monthInfo.Month);
				var days = Context.DaysContainer.GetDaysForMonth(monthInfo.Id);
				month.Days = days;
				Months.Add(month);
			}

			CurrentMonth = month;
		}
	}

	public void SelectPrevMonth()
	{
		if (CurrentMonth == null)
			return;

		var monthInfo = CurrentMonth.GetPreviousMonthInfo();
		if (Context.DaysContainer.ContainsMonth(monthInfo.Id))
		{
			var month = Months
				.FirstOrDefault(x => x.Year == monthInfo.Year && x.Month == monthInfo.Month);

			if (month == null)
			{
				month = new AppoMonth(monthInfo.Year, monthInfo.Month);
				var days = Context.DaysContainer.GetDaysForMonth(monthInfo.Id);
				month.Days = days;
				Months.Insert(0, month);
			}

			CurrentMonth = month;
		}
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


	public bool RangeSelected
	{
		get => RangeEnabled && DayStart != null && DayEnd != null;
		set => OnPropertyChanged();
	}

	#endregion

	#region CONTEXT

	public BookableShopElement Context { get; set; }

	AppoMonth? _month;
	public AppoMonth? CurrentMonth 
	{
		get
		{
			return _month;
		}
		protected set
		{
			if (_month != value)
			{
				_month = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedYear));
				OnPropertyChanged(nameof(SelectedMonth));
			}
		}
	}

	public int SelectedMonth
	{
		get
		{
			if (CurrentMonth == null)
				return 1;

			return CurrentMonth.Month;
		}
	}

	public int SelectedYear
	{
		get
		{
			if (CurrentMonth == null)
				return DateTime.Now.Year;

			return CurrentMonth.Year;
		}
	}

	ObservableRangeCollection<AppoMonth> _months = new();
	public ObservableRangeCollection<AppoMonth> Months
	{
		get { return _months; }
		set
		{
			if (_months != value)
			{
				_months = value;
				OnPropertyChanged();
			}
		}
	}

	#endregion


}