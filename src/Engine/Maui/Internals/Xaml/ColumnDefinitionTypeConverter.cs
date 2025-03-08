//Adapted code from  Xamarin.Forms 

using System.ComponentModel;
using System.Globalization;

namespace DrawnUi.Maui.Infrastructure.Xaml;

public class ColumnDefinitionTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object obj)
    {
        if (obj is string value)
        {
            var converter = new GridLengthTypeConverter();
            return new ColumnDefinition { Width = (GridLength)converter.ConvertFromInvariantString(value) };
        }

        throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", obj, typeof(ColumnDefinition)));
    }

}
