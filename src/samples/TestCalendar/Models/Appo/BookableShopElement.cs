using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using AppoMobi.Models;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using Newtonsoft.Json;

namespace TestCalendar.Views;

public class MonthInsideDaysIndex
{
	public float Id { get; set; }
	public int Year { get; set; }
	public int Month { get; set; }
	public int DayIndexStart { get; set; }
	public int DayIndexEnd { get; set; }
}

public class BookableShopElement : INotifyPropertyChanged
{

	#region DAYS CONTAINER

	(List<AppoDay> Days, List<MonthInsideDaysIndex> Indexes) CreateDays(int yearStart, int monthStart, int yearEnd, int monthEnd)
	{
		var year = yearStart;
		var month = monthStart;

		List<AppoDay> ret = new();
		List<MonthInsideDaysIndex> indexes = new();

		while (year <= yearEnd || month <= monthEnd)
		{
			SelectMonth(year, month, null);
			indexes.Add(new()
			{
				Id = AppoMonth.EncodeMonth(year, month),
				Month = month,
				Year = year,
				DayIndexStart = ret.Count,
				DayIndexEnd = ret.Count + Days.Count - 1
			});
			ret.AddRange(Days);

			month++;
			if (month > 12)
			{
				month = 1;
				year++;
			}
		}

		return (ret, indexes);
	}

	public DaysContainer DaysContainer { get; protected set; }

	public void SetupDays(int yearStart, int monthStart, int yearEnd, int monthEnd)
	{
		var data = CreateDays(yearStart, monthStart, yearEnd, monthEnd);
		DaysContainer = new DaysContainer(data.Days, data.Indexes);
	}

	#endregion

	ObservableRangeCollection<AppoDay> _Days = new();
	public ObservableRangeCollection<AppoDay> Days
	{
		get { return _Days; }
		set
		{
			if (_Days != value)
			{
				_Days = value;
				OnPropertyChanged();
			}
		}
	}

	//todo: optimize culture related dynamic getters

	public static string Lang { get; set; } = "en";
	
	public static string PricesMask { get; set; } = "${0}";

	public BookableShopElement(ShopElement element, string defaultInterval, bool autoSelect)
	{
		_autoSelect = autoSelect;

        NeedSeats = 1;
        
        Element = element; //calls InitIntervals inside if has schedules

		_defaultInterval = defaultInterval;
        if (!string.IsNullOrEmpty(defaultInterval) || Element.Schedules.Count==0)
			InitIntervals(_defaultInterval);
    }

    protected string _defaultInterval;
    ShopElement _Element;
    public ShopElement Element
    {
        get { return _Element; }
        set
        {
            if (_Element != value)
            {
                _Element = value;
                if (_Element.Schedules.Any())
                {
                    InitIntervals();
                }
                OnPropertyChanged("Element");
            }
        }
    }

    public decimal TotalPrice
    {
        get
        {
            if (Element == null) return 0;
            return Element.Price * NeedSeats;
        }
    }

    public string CustomerInfo
        
    {
        get
        {
            //todo from Auth
            return "Administrator\r\n+7(916)550-11-33";
        }
    }

    public string Conditions
        
    {
        get { return Element.Conditions; }
    }

    public string TotalPriceDesc
        
    {
        get
        {
            var price = "";
            try
            {
                price = TotalPrice.ToString(PricesMask);
            }
            catch (Exception e)
            {
                price = 0.ToString(PricesMask);
            }
            return $"Total:  " + price;//$"{ResStrings.Total}:  " + price;
        }
    }

    ObservableRangeCollection<FullBookableInterval> _DaylyIntervals = new ();
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

    int _SelectedMonth = DateTime.Now.Month;
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

    int _SelectedYear = DateTime.Now.Year;
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

    public string Id
    {
        get
        {
            if (Element == null) return "";
            return Element.Id;
        }
    }

    public int CurrentIntervalSeats
    {
        get
        {
            try
            {
                if (Element.MaxSeats < 1) throw new Exception();

                var total = Element.MaxSeats - Element.Schedules.Selected.SeatsTaken;
                if (total > -1)
                    return total;
            }
            catch (Exception e)
            {
            }
            if (!DaylyIntervals.Any()) return -2;
            return -1;
        }
    }

    public void Patch(ShopElement source)
    {
        Reflection.MapProperties(source, Element);
        InitIntervals(_defaultInterval);
    }

    public void InitIntervals(string defaultInterval = null)
    {
        //not checking schedules for null..
        if (!Element.Schedules.Any())
        {
            Days.Clear();;
            DaylyIntervals.Clear();
            var today = DateTime.Now;
            SelectMonth(today.Year, today.Month);
            OnPropertyChanged("BookingTimeDesc");
            OnPropertyChanged("BookingDateTimeDesc");
            OnPropertyChanged("BookingDateDesc");
            IntervalsReady = true;
            return;
        }

        //initial load selects first month of the loaded period
        var year = Element.Schedules.First().TimeStart.Value.Year;
        var month = Element.Schedules.First().TimeStart.Value.Month;

        BookableInterval interval = null;

        if (!string.IsNullOrEmpty(defaultInterval))
        {
            interval = Element.Schedules.FirstOrDefault(x => x.Id == defaultInterval);
            if (interval != null)
            {
                //initial load selects first month of the loaded period
                year = interval.TimeStart.Value.Year;
                month = interval.TimeStart.Value.Month;
            }
        }

        SelectMonth(year, month, interval);

        IntervalsReady = true;
    }

    public bool SelectMonth(int year, int month, BookableInterval interval = null)
    {
        SelectedMonth = month;
        SelectedYear = year;

        var InThisMonth = Element.Schedules.Where(x => x.TimeStart.Value.Month == month && x.TimeStart.Value.Year == year)
            .ToList();

        // if (!InThisMonth.Any()) return false;

        //build intervals
        var days = new List<AppoDay>();
        var maxday = DateTime.DaysInMonth(year, month);
        for (int a = 1; a < maxday + 1; a++)
        {
            var day = new AppoDay();
            day.Date = new DateTime(year, month, a);
			
            //day.Id = $"{day.Month}-{day.Day}"; legacy appo, will not work with multi-months engine
			day.Id = $"{a:00}.{month:00}.{year}";

			IEnumerable<BookableIntervalDto> intervals = null;
            try
            {
                intervals = InThisMonth.Where(x => x.TimeStart.Value.Day == day.Day && x.TimeStart.Value.Month == day.Month);
                if (!intervals.Any())
                {
                    day.Disabled = true;
                }
                else
                {
					//yay
					var stop = 4;
                }
            }
            catch (Exception e)
            {
                day.Disabled = true;
            }
            days.Add(day);
        }

        Days.ReplaceRange(days);

        //select
        if (_autoSelect && Days.Count > 0)
        {
            bool standart = true;
            if (interval != null)
            {
                try
                {
                    var matchDay = interval.TimeStart.Value.Day;
                    var day = Days.FirstOrDefault(x => x.Day == matchDay);
                    SelectDay(day, interval.TimeStart.Value.Hour, interval.TimeStart.Value.Minute);
                    standart = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            //else
            if (standart)//Days.Selected == null || true)
            {
                var day = Days.FirstOrDefault(x => x.Disabled != true);
                SelectDay(day);
            }
            //else
            //{
            //    //make sure old selection is applied to new list
            //    var sel = Days.FirstOrDefault(x => x.Id == Days.Selected.Id);
            //    if (sel != null)
            //    {
            //        sel.Selected = true;
            //    }
            //}

        }

        //    if (redraw)
        //   RedrawItems(DaysBuffer, ref _Days, true);

        OnPropertyChanged("MonthSelected");
        return true;
    }
    /// <summary>
    /// param Hour to set default selection
    /// </summary>
    /// <param name="hour"></param>
    
    public void LoadFullDaylyIntervals(int hour = -1, int minute = -1)
    {
        //todo YEAR
        IEnumerable<BookableIntervalDto> tmp = null;
        var error = false;
        try
        {
            var selected = Days.First(x => x.Selected);
            tmp = Element.Schedules.Where(x => x.TimeStart.Value.Day == selected.Day && x.TimeStart.Value.Month == selected.Month);
        }
        catch (Exception e)
        {
            error = true;
        }

        if (!error)
        {
            if (tmp.Any())
            {
                var DaylyIntervalsBuffer = JsonConvert.DeserializeObject<IEnumerable<FullBookableInterval>>(JsonConvert.SerializeObject(tmp));

                var c = 0;
                foreach (var interval in DaylyIntervalsBuffer)
                {
                    c++;
                    //use id provided by server
                    //interval.Id = $"{Days.Selected.Id}-{c}";
                    interval.ParentId = Element.Id;
                }

                DaylyIntervals.ReplaceRange(DaylyIntervalsBuffer);

                //select
                FullBookableInterval selectInterval = null;
                if (DaylyIntervals.Any())
                {
                    if (hour > -1 && minute > -1)
                    {
                        selectInterval = DaylyIntervals.FirstOrDefault(x => x.TimeStart.Value.Hour == hour && x.TimeStart.Value.Minute == minute);
                    }
                    else
                    if (hour > -1)
                    {
                        selectInterval = DaylyIntervals.FirstOrDefault(x => x.TimeStart.Value.Hour == hour);
                    }
                    else
                    {
                        selectInterval = DaylyIntervals[0];
                    }
                    SelectInterval(selectInterval);
                }
            }
            else
            {
                DaylyIntervals.Clear();
            }
        }

        OnPropertyChanged("DaylyIntervals");
        OnPropertyChanged("BookingTimeDesc");
        OnPropertyChanged("BookingDateTimeDesc");
        OnPropertyChanged("BookingDateDesc");
        OnPropertyChanged("SelectedDateTypeDesc");
        OnPropertyChanged("SelectedDateDesc");
        OnPropertyChanged("FullTitle");
        OnPropertyChanged("CurrentIntervalSeats");
        OnPropertyChanged("CurrentIntervalSeatsDesc");
        OnPropertyChanged("HasDailyIntervals");
        OnPropertyChanged("SelectedTimeDesc");
        OnPropertyChanged("SelectedDowDesc");

    }

    public void SelectDay(AppoDay day, int hour, int minute)
    {
        if (day == null)
        {
            SetSelection(Days, null);
            return;
        }
        foreach (var member in Days)
        {
            member.Selected = false;
        }
        day.Selected = true;
        SetSelection(Days, day);

        LoadFullDaylyIntervals(hour, minute);

        OnPropertyChanged("SelectedDateTypeDesc");
        OnPropertyChanged("SelectedDateDesc");
        OnPropertyChanged("FullTitle");
    }

	/// <summary>
	/// Call this on UI-thread.
	/// Sets day as selected, in case of ranged mode this sets it as selection start only
	/// </summary>
	/// <param name="day"></param>
    public void SelectDay(AppoDay day)
    {
        if (day == null)
        {
            SetSelection(Days, null);
            return;
        }

        //in case this day reference is  not inside Days
        day.Selected = true;
        day.SelectionStart = true;
        day.SelectionEnd = false;

        //affects UI
		SetSelection(Days, day);
		
		LoadFullDaylyIntervals();
        OnPropertyChanged("SelectedDateTypeDesc");
        OnPropertyChanged("SelectedDateDesc");
        OnPropertyChanged("FullTitle");
    }

	/// <summary>
	/// Call this on UI-thread.
	/// Select a ranged days interval, stat and end can fall in different months. If just one of parameters is null the whole existing range will become unselected and then if start is not null then SelectDay(start) will be invoked internally.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	public (int Start, int End) SelectDays(AppoDay start, AppoDay end)
	{

		//in case this day reference is  not inside Days
		if (start != null)
		{
			start.Selected = true;
			start.SelectionStart = true;
			start.SelectionEnd = false;
		}
		if (end != null)
		{
			end.Selected = true;
			end.SelectionStart = false;
			end.SelectionEnd = true;
		}

		//affects UI
		var indexes = SetSelection(Days, start, end);

		LoadFullDaylyIntervals();
		OnPropertyChanged("SelectedDateTypeDesc");
		OnPropertyChanged("SelectedDateDesc");
		OnPropertyChanged("FullTitle");

		return indexes;
	}

	public void SelectInterval(FullBookableInterval time)
    {
        if (time == null) return;
        foreach (var member in DaylyIntervals)
        {
            member.Selected = false;
        }
        time.Selected = true;
        SetSelection(DaylyIntervals, time);

        Element.Schedules.SelectOne(time.Id);
        OnPropertyChanged("HasImportantNotes");
    }

    static void SetSelection(IEnumerable<ISelectableRangeOption> list, ISelectableRangeOption? selected)
    {
        foreach (var canBeSelected in list)
        {
	        canBeSelected.SelectionStart = false;
	        canBeSelected.SelectionEnd = false;
            
	        if (selected != null && selected.Id == canBeSelected.Id)
            {
                canBeSelected.Selected = true;
			}
            else
            {
                canBeSelected.Selected = false;
            }
        }
    }

	/// <summary>
	/// Set range selection. As the selection could be inverted that returns the adjusted start and end IDs.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <returns></returns>
    static (int Start, int End) SetSelection(IEnumerable<ISelectableRangeOption> list, ISelectableRangeOption? start, ISelectableRangeOption? end)
    {
	    var startIndex = -1;
	    var endIndex = -1;
	    var iteration = -1;
	    var direction = -1;
	    foreach (var canBeSelected in list)
	    {
		    iteration++;
			if (end != null && canBeSelected.Id == end.Id)
			{
				endIndex = iteration;
			}
		    if (start != null && start.Id == canBeSelected.Id)
		    {
			    canBeSelected.Selected = true;
			    canBeSelected.SelectionStart = true;
			    canBeSelected.SelectionEnd = false;
			    startIndex = iteration;
				if (endIndex < 0)
			    {
				    direction = 1;
			    }
		    }
		    else
		    {
			    canBeSelected.Selected = false;
			    canBeSelected.SelectionEnd = false;
			    canBeSelected.SelectionStart = false;
		    }
	    }

	    if (endIndex>=0)
	    {
		    iteration = -1;
		    bool insideRange = false;
		    if (direction < 0)
		    {
			    (endIndex, startIndex) = (startIndex, endIndex);
		    }
			foreach (var canBeSelected in list)
			{
				iteration++;
				if (iteration > endIndex)
				{
					insideRange = false;
				}
			    if (iteration == startIndex)
			    {
				    canBeSelected.Selected = true;
				    canBeSelected.SelectionStart = true;
				    canBeSelected.SelectionEnd = false;
				    insideRange = true;
			    }
			    else
			    if (iteration == endIndex)
			    {
				    canBeSelected.Selected = true;
				    canBeSelected.SelectionStart = false;
				    canBeSelected.SelectionEnd = true;
			    }
			    else
			    if (insideRange)
			    {
					canBeSelected.Selected = true;
				    canBeSelected.SelectionEnd = false;
				    canBeSelected.SelectionStart = false;
			    }
		    }


	    }

	    return (startIndex, endIndex);
    }

	public bool HasImportantNotes
    {
        get
        {
            return !string.IsNullOrEmpty(Element.Schedules?.Selected?.PublicNotes);
        }
    }

    public DateTime GetPrevMonth()
    {
        var dt = new DateTime(SelectedYear, SelectedMonth, 1);
        return new DateTime(dt.AddMonths(-1).Year, dt.AddMonths(-1).Month, 1);
    }
    
    public DateTime GetNextMonth()
    {
        var dt = new DateTime(SelectedYear, SelectedMonth, 1);
        return new DateTime(dt.AddMonths(1).Year, dt.AddMonths(1).Month, 1);
    }
    
    public bool SelectNextMonth()
    {
        var dayone = GetNextMonth();
        var year = dayone.Year;
        var month = dayone.Month;
        return SelectMonth(year, month);
    }
    
    public bool SelectPrevMonth()
    {
        var dayone = GetPrevMonth();
        var year = dayone.Year;
        var month = dayone.Month;
        return SelectMonth(year, month);
    }
    
    public bool CanSelectPrevMonth
        
    {
        get
        {
            var dayone = GetPrevMonth();
            var year = dayone.Year;
            var month = dayone.Month;
            //if (ShowTimeFrom.Year > year || ShowTimeTo.Year < year || (ShowTimeFrom.Month > month && ShowTimeFrom.Year == year) || (ShowTimeTo.Month < month && ShowTimeTo.Year == year))
            //    return false;
            if (!Element.Schedules.Any(x => x.TimeStart.Value.Month == month && x.TimeStart.Value.Year == year))
                return false;
            return true;
        }
    }
    
    public bool CanSelectNextMonth
    {
        get
        {
            var dayone = GetNextMonth();
            var year = dayone.Year;
            var month = dayone.Month;
            //if (ShowTimeFrom.Year > year || ShowTimeTo.Year < year || (ShowTimeFrom.Month > month && ShowTimeFrom.Year == year) || (ShowTimeTo.Month < month && ShowTimeTo.Year == year))
            //    return false;
            if (!Element.Schedules.Any(x => x.TimeStart.Value.Month == month && x.TimeStart.Value.Year == year))
                return false;
            return true;
        }
    }

    public bool HasDailyIntervals
    {
        get
        {
            return DaylyIntervals.Any();
        }
    }
    
    public bool CanMakeOrder
    {
        get
        {
            return DaylyIntervals.Any();
        }
    }

    public string CurrentIntervalSeatsDesc
    {
        get
        {
            var ret = CurrentIntervalSeats;
            if (ret == -2)
                return "None";//ResStrings.None;
            else
            if (ret < 0)
                return "Many";//ResStrings.Many;
            return CurrentIntervalSeats.ToString();
        }
    }

    public string SelectedDateTypeDesc
    {
        get
        {
            try
            {
                var available = Days.Where(x => !x.Disabled).ToList();
                var selected = Days.First(x => x.Selected);
                if (selected.Id == available[0].Id)
                {
                    return "Closest date";//ResStrings.ClosestDate;
                }
            }
            catch (Exception e)
            {
            }
            return "Date";//ResStrings.Date;//"Выбранная дата";
        }
    }

    public string SelectedTimeDesc
    {
        get
        {
            try
            {
                var selected = Days.FirstOrDefault(x => x.Selected);
                var selectedInterval = DaylyIntervals.FirstOrDefault(x => x.Selected);

                if (selected != null && selectedInterval != null)
                {
                    return selectedInterval.TimeStart.Value.ToShortTimeString();//.ToString("HH:mm");
                }
            }
            catch (Exception e)
            {
            }

            return "None";//ResStrings.None;
        }
    }

    public string FullTitle
    {
        get
        {
            try
            {
                return Element.Title.ToUpperInvariant()
                       + "\r\n" + SelectedDateDesc + ", " + SelectedDowDesc + ", "
                       + "\r\n" + SelectedTimeDesc;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }
    }

    public string SelectedDowDesc
    {
        get
        {
            try
            {
                var selected = Days.FirstOrDefault(x => x.Selected);
                if (selected != null)
                {
                    var t = DateTimeFormatInfo.CurrentInfo;
                    if (t != null)
                    {
                        return t.GetDayName(selected.Date.DayOfWeek);
                    }
                    else
                    {
                        return "???";
                    }
                }
            }
            catch (Exception e)
            {
            }
            return "";
        }
    }

    public string SelectedDateDesc
    {
        get
        {
            var selected = Days.FirstOrDefault(x => x.Selected);
            if (selected != null)
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Lang);
                var date = selected.Date.ToString("d MMMM");
                return $"{date}";
            }
            return "Unavailable";//ResStrings.Unavalable;//"Нет доступных";
        }
    }

    public string CurrentMonthDesc
    {
        get
        {
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(Lang);
                var time = new DateTime(SelectedYear, SelectedMonth, 1);
                var desc = time.ToString("MMMM yyyy").ToTitleCase();
                return desc;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }

    public void RaiseManualProperties()
    {
        RaiseProperties(
            "SelectedDateDesc",
            "SelectedDowDesc",
            "FullTitle",
            "SelectedDateTypeDesc",
            "CurrentMonthDesc",
            "SelectedDateTypeDesc",
            "CurrentIntervalSeatsDesc",
            "HasDailyIntervals",
            "CanMakeOrder"
        );
    }

    int _NeedSeats;
    public int NeedSeats
    {
        get { return _NeedSeats; }
        set
        {
            if (_NeedSeats != value)
            {
                _NeedSeats = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
                OnPropertyChanged("TotalPriceDesc");
                OnPropertyChanged("NeedSeatsDesc");
            }
        }
    }
    
    public string NeedSeatsDesc
    {
        get
        {
            return $"Ned seats: {NeedSeats}";
            //return string.Format(ResStrings.NumberOfSeats0, NeedSeats);
        }
    }

    bool _IntervalsReady;
    readonly bool _autoSelect;

    public bool IntervalsReady
    {
        get { return _IntervalsReady; }
        set
        {
            if (_IntervalsReady != value)
            {
                _IntervalsReady = value;
                OnPropertyChanged();
            }
        }
    }

    #region INTERFACE

    public void RaiseProperties(params object[] raiseProperties)
    {
        if (raiseProperties != null)
        {
            foreach (var prop in raiseProperties)
            {
                OnPropertyChanged(prop.ToString());
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

}