using System.Globalization;

namespace AppoMobi.Xamarin.DrawnUi.DrawnUi.Converters
{
    public class CompareIntegersConverter : ConverterBase
    {
        public override object OnValueReceived(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ret = true;

            try
            {
                if (value is Enum)
                {
                    int intVar = (int)value.GetType().GetField("value__").GetValue(value);

                    if (intVar == parameter.ToString().ToInteger())
                        return true;
                }

            }
            catch (Exception e)
            {

            }

            try
            {
                var iVisualStep = int.Parse((string)parameter);
                var iStep = (int)value;
                if (iStep != iVisualStep)
                    ret = false;
                return ret;
            }
            catch (Exception e)
            {
            }

            try
            {
                ret = false;
                var ints = ((string)parameter).Split(',');
                foreach (var number in ints)
                {
                    var iStep = int.Parse(number);
                    if (iStep == (int)value)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return ret;
        }


    }
}