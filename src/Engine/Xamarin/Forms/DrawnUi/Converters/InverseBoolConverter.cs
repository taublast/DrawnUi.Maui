using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnValueReceived(value, targetType, parameter, culture);
        }

        public virtual object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }


    }
}
