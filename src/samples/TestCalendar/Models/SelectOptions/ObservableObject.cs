using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppoMobi.Xam;

public class ObservableObject : INotifyPropertyChanged
{
    public void RaiseProperties()
    {
        var props = this.GetType().GetProperties();
        foreach (var property in props)
        {
            if (property.CanRead)
            {
                OnPropertyChanged(property.Name);
            }
        }
    }


    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}