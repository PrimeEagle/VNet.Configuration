using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

#pragma warning disable CS8603 // Possible null reference return.


namespace VNet.Configuration.TypeConverters
{
    public class RangeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is not string str)
                return base.ConvertFrom(context, culture, value);

            var rangeObj = JsonSerializer.Deserialize<Range<double>>(str);

            return rangeObj == null ? base.ConvertFrom(context, culture, value) :
                new Range<double>(rangeObj.Start, rangeObj.End);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.ConvertTo(context, culture, value, destinationType);

            if (value is not IRange<double> range) return base.ConvertTo(context, culture, value, destinationType);
            var rangeObj = new Range<double>(range.Start, range.End);
            
            return JsonSerializer.Serialize(rangeObj);
        }
    }
}