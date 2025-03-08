using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters;

public class ValueOverZeroConverter : ConverterBase
{
    public override object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return $"{value}".ToDouble() > 0.0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }
}