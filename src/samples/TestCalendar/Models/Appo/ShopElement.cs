using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppoMobi.Specials;
using AppoMobi.Xam;
using TestCalendar.Views;

namespace AppoMobi.Models
{

    public class ShopElement : MainElementDto, INotifyPropertyChanged
    {
        //public ImageDecoder.ImageInfo ImageMain { get; set; }

        public string CategoryName { get; set; }
        bool _Deleted;
        public bool Deleted
        {
            get { return _Deleted; }
            set
            {
                if (_Deleted != value)
                {
                    _Deleted = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        //override base
        int _MaxSeats;
        public new int MaxSeats
        {
            get { return _MaxSeats; }
            set
            {
                if (_MaxSeats != value)
                {
                    _MaxSeats = value;
                    OnPropertyChanged("MaxSeats");
                }
            }
        }

        public bool HasLink
        {
            get { return !string.IsNullOrEmpty(LinkMoreInfo); }           
        }

        int _IncToRefreshCell;
        public int IncToRefreshCell
        {
            get
            {
                return _IncToRefreshCell;
            }
            set
            {
                _IncToRefreshCell = value;
                OnPropertyChanged();
            }
        }

        decimal _Price;
        public new decimal Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    OnPropertyChanged("Price");
                    OnPropertyChanged("PriceDesc");
                }
            }
        }

        /*
        public string PriceDesc
        {
            get
            {
                var price = "";
                try
                {
                    price = Price.ToString(Core.Current.Info.Strings.PricesMask);
                }
                catch (Exception e)
                {
                    price = 0.ToString(Core.Current.Info.Strings.PricesMask);
                }
                return price;
            }
        }
        */

        public bool HasDescription
        {
            get { return !string.IsNullOrEmpty(Desc); }
        }

        string _Title;
        public new string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }
        }

        string _Desc;
        public new string Desc
        {
            get { return _Desc; }
            set
            {
                if (_Desc != value)
                {
                    _Desc = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasDescription");
                }
            }
        }


        public Color ImagePlaceholderColor
        {
            get
            {
                if (string.IsNullOrEmpty(ImageColor))
                    return Colors.GhostWhite;
                return Color.FromHex(ImageColor);
            }
        }

        string _PriceConverted;
        public string PriceConverted
        {
            get { return _PriceConverted; }
            set
            {
                if (_PriceConverted != value)
                {
                    _PriceConverted = value;
                    OnPropertyChanged("PriceConverted");
                }
            }
        }

        OptionsList<BookableInterval> _Schedules;
        public new OptionsList<BookableInterval> Schedules
        {
            get { return _Schedules; }
            set
            {
                if (_Schedules != value)
                {
                    _Schedules = value;
                    if (_Schedules != null && _Schedules.Any())
                    {
                        //can select.. but check not to reset our selection if already set
                        if (SelectedInterval == null)

                        {
                            //just set first in list as selected
                            SelectedInterval = _Schedules[0];
                        }
                        else
                        {
                            var selected = _Schedules.FirstOrDefault(x => x.Id == SelectedInterval.Id);
                            if (selected != null)
                            {
                                //replace selected with new with same id
                                SelectedInterval = selected;
                            }
                            else
                            {
                                //this selected interval doen't exist anymore.. replace?... yeah
                                SelectedInterval = _Schedules[0];
                            }
                        }
                    }
                    OnPropertyChanged("Schedules");
                }
            }
        }

        BookableInterval _SelectedInterval;
        public BookableInterval SelectedInterval
        {
            get { return _SelectedInterval; }
            set
            {
                if (_SelectedInterval != value)
                {
                    _SelectedInterval = value;
                    OnPropertyChanged("SelectedInterval");
                    OnPropertyChanged("HasImportantNotes");
                }
            }
        }

        //-----------------------------------------------------------------
        public bool HasImportantNotes
        //-----------------------------------------------------------------
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedInterval?.PublicNotes);
            }
        }


        public ShopElement()
        {
            Init();
        }

    //-----------------------------------------------------------------
    public void Init()
    //-----------------------------------------------------------------
    {
        //if (ImageColor.HasNoContent())
        //    ImageColor = BackColors.PlaceholderRemoteImages;
        //    ImageMain = new ImageDecoder.ImageInfo(ImageId, ImageColor.ColorFromHex(), Title?.Default);

        if (Schedules == null)
            Schedules = new OptionsList<BookableInterval>();

        RaiseProperties(
            "HasSchedules");
    }

        //-----------------------------------------------------------------
        public bool HasSchedules
        //-----------------------------------------------------------------
        {
            get
            {
                if (Schedules == null)
                    return false;
                return Schedules.Any();
            }
        }

    //-----------------------------------------------------------------
    public void RaiseManualProperties()
    //-----------------------------------------------------------------
    {
        RaiseProperties(
            "ClosestDate",
            "ClosestDateIsTodayOrTommorow",
            "ClosestDateExplan",
            "AnnouncementColorStart",
            "AnnouncementColor",
            "AnnouncementTextOpacity",
            "ClosestTimeFull",
            "SetToRefresh"
        );
    }


    //-----------------------------------------------------------------
    public string ClosestDate
    //-----------------------------------------------------------------
    {
        get
        {
            try
            {
                if (Schedules.Any())
                {
                    var date = Schedules[0].TimeStart.Value.ToString("d MMMM");
                    return $"{date}";
                }
            }
            catch (Exception e)
            {
            }

            return "None"; //ResStrings.None;
        }
    }


    //-------------------------------------------------------------
    public bool ClosestDateIsTodayOrTommorow
    //-------------------------------------------------------------
    {
        get
        {
            try
            {
                if (Schedules.Any())
                {
                    if (Schedules[0].TimeStart == null) 
                        return false;
                    var todayDate = DateTime.Now;
                    var today = todayDate.Date;
                    var when = Schedules[0].TimeStart.Value.Date;
                    var delta = when - today;
                    var days = delta.Days;
                    if (days < 2) return true;
                }
            }
            catch (Exception e)
            {
            }
            return false;
            }
        }

        public string ColorNormal = "#99000000";
        public string ColorHot = "#dc1e31";

        //-----------------------------------------------------------------
        public Color AnnouncementColor
        //-----------------------------------------------------------------
        {
            get
            {
                Color myColor;
                if (ClosestDateIsTodayOrTommorow)
                {
                    myColor = Color.FromHex(ColorHot);
                }
                else
                {
                    myColor = Color.FromHex(ColorNormal);
                }
                myColor = myColor.MultiplyAlpha((float)myColor.Alpha * 0.9411f);
                //System.Diagnostics.Debug.WriteLine($"[ShopElement]  endc : {myColor}");
                return myColor;
            }
        }

        //-----------------------------------------------------------------
        public double AnnouncementTextOpacity
        //-----------------------------------------------------------------
        {
            get
            {
                double ret=1.0;
                if (ClosestDateIsTodayOrTommorow)
                {
                    ret = 1.0;
                }
                else
                {
                    ret = 0.75;
                }
                return ret;
            }
        }

        public Color AnnouncementColorStart
        {
            get
            {
                var myColor = AnnouncementColor.MultiplyAlpha(0.1f);
                
                return myColor;
            }
        }

        public string ClosestDateExplan
        {
            get
            {
                try
                {
                    if (Schedules.Any())
                    {
                        var date = Schedules[0].ExplainWhen;
                        return $"{date}";
                    }
                }
                catch (Exception e)
                {
                }

                return "Unavalable"; //ResStrings.Unavalable;
            }
        }


        //-----------------------------------------------------------------
        public string ClosestTimeFull
        //-----------------------------------------------------------------
        {
            get
            {
                try
                {
                    if (Schedules.Any())
                    {
                        var date = Schedules[0].TimeStart.Value.ToString("d MMMM");
                        var time = Schedules[0].TimeStart.Value.ToString("HH:mm");
                        return $"{date} в {time}";
                    }
                }
                catch (Exception e)
                {
                }

                return "None";// //ResStrings.None;
            }
        }




    //---------------------------------------------------------------------------------------------------------
    public string NameLatinSort
    //---------------------------------------------------------------------------------------------------------
    {
        get
        {
            return "";
            //if (string.IsNullOrWhiteSpace(TitleLatin) || TitleLatin.Length == 0)
            //    return "?";

            //return TitleLatin[0].ToString().ToUpper(); // Title.ToTitleCase();
            }
    }

    //---------------------------------------------------------------------------------------------------------
    public string NameSort
    //---------------------------------------------------------------------------------------------------------
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Title) || Title?.Length == 0)
                return "?";

            return Title.Left(1).ToUpper(); // Title.ToTitleCase();
            }
    }

    //---------------------------------------------------------------------------------------------------------
    public string CategorySort
    //---------------------------------------------------------------------------------------------------------
    {
        get
        {
            if (string.IsNullOrWhiteSpace(CategoryName) || CategoryName.Length == 0)
                return "?";

            return CategoryName; // Title.ToTitleCase();
            }
    }

    public string NameAll
    {
        get
        {
            return "All by distance"; //AppoMobi.Common.ResX.ResStrings.AllSortedByDistance;
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
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }



}
