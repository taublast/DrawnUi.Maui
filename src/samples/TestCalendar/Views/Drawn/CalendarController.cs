using AppoMobi.Models;
using AppoMobi.Specials;
using AppoMobi.Xam;
using Polly;
using TestCalendar.Views;

namespace TestCalendar.Drawn;

public class CalendarController : BindableObject, IDrawnDaysController
{

	//todo
	IEnumerable<AppoDay> SelectedMonthDays { get; }

	public CalendarController()
	{
		var testData = AppoHelper.GetDefaultConstraints();
		
		var intervals =
			AppoHelper.GetAvailable(DateTime.Now.AddDays(90), DateTime.Now, testData, TimeSpan.FromHours(2));

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
		
		var month = new AppoMonth(SelectedYear, SelectedMonth);
		month.Days.ReplaceRange(context.Days);
		Months.Add(month);
		CurrentMonth = month;
	}

	bool RangeEnabled => true;

	public string Lang => BookableShopElement.Lang;

	public string PricesMask => "{0} руб.";


	#region Selection

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
		Context.SelectDay(value);
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
			var selected = Context.SelectDays(start, end);
			if (selected.Start > 0)
			{
				DayEnd = end;
			}
		}
	}

	/// <summary>
	/// Is in range mode and have first date already selected, waiting to select ending date
	/// </summary>
	public bool IsSelecting => RangeEnabled && DayStart != null && DayEnd == null;

	public bool CanSelectNextMonth => Context.CanSelectNextMonth;
	public bool CanSelectPrevMonth => Context.CanSelectPrevMonth;
	public string CurrentMonthDesc => Context.CurrentMonthDesc;

	public void SelectNextMonth()
	{
		if (CanSelectNextMonth)
		{
			Context.SelectNextMonth();

			var month = Months
				.FirstOrDefault(x => x.Year == Context.SelectedYear && x.Month == Context.SelectedMonth);
			if (month==null)
			{
				month = new AppoMonth(SelectedYear, SelectedMonth);
				month.Days.ReplaceRange(Context.Days);
				Months.Add(month);
			}
			CurrentMonth = month;
		}
	}

	public void SelectPrevMonth()
	{
		if (CanSelectPrevMonth)
		{
			Context.SelectPrevMonth();

			var month = Months
				.FirstOrDefault(x => x.Year == Context.SelectedYear && x.Month == Context.SelectedMonth);
			if (month == null)
			{
				month = new AppoMonth(SelectedYear, SelectedMonth);
				month.Days.ReplaceRange(Context.Days);
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

	public AppoMonth? CurrentMonth { get; protected set; }

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

	public int SelectedMonth => Context.SelectedMonth;

	public int SelectedYear => Context.SelectedYear;

	#endregion


}