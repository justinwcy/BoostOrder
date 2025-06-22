using System.Globalization;
using System.Windows.Data;

namespace BoostOrder.Converter
{
    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                if (int.TryParse(strValue, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}
