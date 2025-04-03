
using System.ComponentModel;
using System.Globalization;

namespace DrawnUi.Infrastructure.Xaml;

public class StringToDoubleArrayTypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            string[] parts = stringValue.Split(',');
            double[] result = new double[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (double.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double partValue))
                {
                    result[i] = partValue;
                }
            }

            return result;
        }

        return base.ConvertFrom(context, culture, value);
    }
}