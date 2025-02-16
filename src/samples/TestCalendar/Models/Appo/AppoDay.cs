
using System.ComponentModel;
using System.Globalization;
using DrawnUi.Maui.Draw;

namespace AppoMobi.Models
{
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
			        NotifyPropertyChanged(nameof(Selected));
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
