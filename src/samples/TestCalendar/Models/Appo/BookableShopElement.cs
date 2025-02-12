using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using AppoMobi.Models;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using Newtonsoft.Json;

namespace TestCalendar.Views;

public class BookableShopElement : INotifyPropertyChanged
//*************************************************************
{
    
  
    public BookableShopElement(ShopElement element, string defaultInterval)
        
    {
        NeedSeats = 1;
        Element = element;
        _defaultInterval = defaultInterval;
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
                price = TotalPrice.ToString(MonthView.PricesMask);
            }
            catch (Exception e)
            {
                price = 0.ToString(MonthView.PricesMask);
            }
            return $"Total:  " + price;//$"{ResStrings.Total}:  " + price;
        }
    }

    ObservableRangeCollection<AppoDay> _Days = new ObservableRangeCollection<AppoDay>();
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

    ObservableRangeCollection<FullBookableInterval> _DaylyIntervals = new ObservableRangeCollection<FullBookableInterval>();
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
            day.Id = $"{day.Month}-{day.Day}";

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
        if (Days.Count > 0)
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

    public void SelectDay(AppoDay day)
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

        LoadFullDaylyIntervals();

        OnPropertyChanged("SelectedDateTypeDesc");
        OnPropertyChanged("SelectedDateDesc");
        OnPropertyChanged("FullTitle");
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

    static void SetSelection(IEnumerable<ISelectableOption> list, ISelectableOption selected)
    {
        foreach (var canBeSelected in list)
        {
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
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(MonthView.Lang);
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
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(MonthView.Lang);
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