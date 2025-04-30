using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters;

public class DivideValueConverter : ConverterBase
{
    public override object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var adjust = parameter.ToString().ToDouble();
        if (value is double doubleValue)
        {
            return doubleValue / adjust;
        }
        if (value is float fValue)
        {
            return (float)(fValue / +adjust);
        }
        if (value is decimal decValue)
        {
            return (decimal)((double)decValue / adjust);
        }
        if (value is int intValue)
        {
            return (int)(intValue / adjust);
        }
        return value;
    }
}