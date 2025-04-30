using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{
    public class NotConverter : ConverterBase
    {

        public override object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;

            if (value is bool input)
            {
                return !input;
            }
            else
            if (value is int intInput)
            {
                return -intInput;
            }
            else
            if (value is double dblInput)
            {
                return -dblInput;
            }
            else
            if (value is decimal decInput)
            {
                return -decInput;
            }

            return value;
        }
    }
}