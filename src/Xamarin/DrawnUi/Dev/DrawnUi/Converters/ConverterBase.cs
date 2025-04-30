using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{
    public class ConverterBase : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OnValueReceived(value, targetType, parameter, culture);
        }

        public virtual object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}