using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters;

public class StringNotEmptyConverter : ConverterBase
{
    public override object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string)
        {
            if (string.IsNullOrEmpty((string)value)) return false;
        }
        else
        {
            return false;
        }
        return true;
    }
}