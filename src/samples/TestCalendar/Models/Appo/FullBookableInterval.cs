using System.ComponentModel;
using System.Globalization;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace AppoMobi.Models
{
   
 
    public class FullBookableInterval : BookableIntervalDto, 
        INotifyPropertyChanged, ISelectableOption
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

        public string Title { get; set; }
        public bool IsReadOnly { get; set; }
        bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string TimeDesc
        {
            get
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(MonthView.Lang);
                return TimeStart.Value.ToShortTimeString();
            }
        }

    }
}
