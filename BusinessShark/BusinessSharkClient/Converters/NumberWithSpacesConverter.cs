using System.Globalization;

namespace BusinessSharkClient.Converters
{
    public class NumberWithSpacesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IFormattable number)
                return number.ToString("#,0.##", new CultureInfo("fr-FR")); // in French culture, the thousand separator is a space  

            return value?.ToString();
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
    }
}
}
