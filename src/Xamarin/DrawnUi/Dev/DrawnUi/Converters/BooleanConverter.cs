using System.Globalization;
using Xamarin.Forms.Xaml;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{


    public class BooleanConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertNull(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
