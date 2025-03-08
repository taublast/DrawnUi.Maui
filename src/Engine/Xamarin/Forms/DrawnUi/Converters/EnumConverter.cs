using System.Globalization;
using Xamarin.Forms.Xaml;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{
    public class EnumConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var ret = Enum.Parse(targetType, $"{value}");

            return ret;

        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
