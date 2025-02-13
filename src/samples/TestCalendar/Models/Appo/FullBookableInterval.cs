using System.ComponentModel;
using System.Globalization;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace AppoMobi.Models
{
   
 
    public class FullBookableInterval : BookableIntervalDto, 
        INotifyPropertyChanged, ISelectableRangeOption
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

		public string Id { get; set; }

        public string ParentId { get; set; }

        public string TimeDesc
        {
            get
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(BookableShopElement.Lang);
                return TimeStart.Value.ToShortTimeString();
            }
        }

    }
}
