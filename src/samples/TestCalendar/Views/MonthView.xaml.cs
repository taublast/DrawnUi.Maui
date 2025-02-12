using AppoMobi.Touch;
using System.ComponentModel;
using System.Globalization;
using AppoMobi;
using AppoMobi.Models;
using AppoMobi.Specials;

namespace TestCalendar.Views;

public partial class MonthView : ContentView
{
	public MonthView()
	{
		InitializeComponent();

        Context = new BookableShopElement(new ShopElement(), null);

        BindingContext = Context;

        SetupTime();
	}

    public static string Lang = "ru";
    public static string PricesMask = "{0} руб.";


    #region CONTEXT

    public BookableShopElement Context { get; set; }
    ObservableRangeCollection<AppoDay> _Days = new ();
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

    int _SelectedMonth;
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

    int _SelectedYear;
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

    void OnClicked_Day(object sender, TapEventArgs e)
    {
        if (((AppoDay)((CellDay)sender).BindingContext).Disabled) return; 

        //Context.SelectDay((AppoDay)((CellDay)sender).BindingContext);
        //InitPicker();
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

    void OnTapped_NextPeriod(object sender, TapEventArgs e)
    {
        //Context.SelectNextMonth();
        SetupCalendarArrows();
    }

    void OnTapped_PrevPeriod(object sender, TapEventArgs e)
    {
        //Context.SelectPrevMonth();
        SetupCalendarArrows();
    }

    Color _cLeftArrowColor { get; set; }
    Color _cRightArrowColor { get; set; }

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
        SetupCalendarArrows();
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

        gridDays.BindingContext = Context;
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
            cLeftArrow.TextColor = _cLeftArrowColor;
        }
        else
        {
            cLeftArrow.TextColor = ColorLines;
        }
        //right arrow
        if (Context.CanSelectNextMonth)
        {
            cRightArrow.TextColor = _cRightArrowColor;
        }
        else
        {
            cRightArrow.TextColor = ColorLines;
        }
    }

    public static Color ColorLines = Colors.DarkGrey;
    public static Color ColorBackground = Colors.White;

}