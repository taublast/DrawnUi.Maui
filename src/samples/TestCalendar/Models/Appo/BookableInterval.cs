using System.ComponentModel;
using AppoMobi.Models;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;

namespace TestCalendar.Views;

public class BookableInterval : BookableIntervalDto, INotifyPropertyChanged, ISelectableOption
{
    public string Title { get; set; }
    public bool IsReadOnly { get; set; }
    int _SeatsTaken;
    public new int SeatsTaken
    {
        get { return _SeatsTaken; }
        set
        {
            if (_SeatsTaken != value)
            {
                _SeatsTaken = value;
                OnPropertyChanged("SeatsTaken");
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

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    #region ICanBeSelected

    public void Select()
    {
        Selected = true;
    }
    public void Deselect()
    {
        Selected = false;
    }

    #endregion

    #region IHasId

    public string GetId() => Id;
    public void SetId(string id) { Id = id; }

    #endregion

}