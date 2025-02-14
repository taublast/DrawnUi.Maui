
using System.ComponentModel;
using System.Globalization;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;

namespace AppoMobi.Models
{

	public class AppoMonth : BindableObject
	{
		public AppoMonth()
		{
			Month = DateTime.Now.Month;
			Year = DateTime.Now.Year;
			Id = EncodeMonth(Year, Month);
		}

		public (int Year, int Month, float Id) GetNextMonthInfo()
		{
			var month = Month;
			var year = Year;

			month++;
			if (month > 12)
			{
				month = 1;
				year++;
			}

			return (year, month, EncodeMonth(year, month));
		}

		public (int Year, int Month, float Id) GetPreviousMonthInfo()
		{
			var month = Month;
			var year = Year;

			month--;
			if (month < 1)
			{
				month = 12;
				year--;
			}

			return (year, month, EncodeMonth(year, month));
		}

		public float GetNextMonthId()
		{
			var month = Month;
			var year = Year;

			month++;
			if (month > 12)
			{
				month = 1;
				year++;
			}

			return EncodeMonth(year, month);
		}

		public float GetPreviousMonthId()
		{
			var month = Month;
			var year = Year;

			month--;
			if (month < 1)
			{
				month = 12;
				year--;
			}

			return EncodeMonth(year, month);
		}

		public static float EncodeMonth(int year, int month)
		{
			return year + month / 100.0f;
		}

		public static (int Year, int Month) DecodeMonth(float value)
		{
			var year = (int)value;
			var month = (int)((value - year) * 100);
			return (year, month);
		}

		public AppoMonth(int year, int month)
		{
			Month = month;
			Year = year;
			Id = EncodeMonth(Year, Month);
		}

		public float Id { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }

		List<AppoDay> _Days = new ();
		public List<AppoDay> Days
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
	}

	public class AppoDay : ISelectableRangeOption, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string Id { get; set; }

        public string ParentId { get; set; }


        //todo add propertychanged calls
        public DateTime Date { get; set; }

        public int Day
        {
            get { return Date.Day; }
        }
        public string DayDesc {
            get
            {
             return Day.ToString();
            }
        }
        public int Month
        {
            get { return Date.Month; }
        }

        public string DateDesc
        {
            get
            {
                return Date.ToShortDateString();
            }
        }
        public string DowDesc
        {
            get
            {
                //System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var t = DateTimeFormatInfo.CurrentInfo;
                if (t != null)
                {
                    return t.GetDayName(Date.DayOfWeek);
                }
                else
                {
                    return "???";
                }
            }
        }

        public string MonthDesc { get; set; }
        bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    NotifyPropertyChanged(nameof(Selected));
				}
            }
        }

        bool _isSelectionStart;
        public bool SelectionStart
        {
	        get
	        {
		        return _isSelectionStart;
	        }
	        set
	        {
		        if (_isSelectionStart != value)
		        {
			        _isSelectionStart = value;
			        NotifyPropertyChanged(nameof(SelectionStart));
		        }
	        }
        }

        bool _isSelectionEnd;
        public bool SelectionEnd
        {
	        get
	        {
		        return _isSelectionEnd;
	        }
	        set
	        {
		        if (_isSelectionEnd != value)
		        {
			        _isSelectionEnd = value;
			        NotifyPropertyChanged(nameof(SelectionEnd));
		        }
	        }
        }


		bool _Disabled;
        public bool Disabled
        {
            get { return _Disabled; }
            set
            {
                if (_Disabled != value)
                {
                    _Disabled = value;
                    NotifyPropertyChanged("Disabled");
                }
            }
        }

        public string Title
        {
            get
            {
                return DowDesc;
            }
            set
            {

            }
        }

        public bool IsReadOnly { get; }
    }
}
