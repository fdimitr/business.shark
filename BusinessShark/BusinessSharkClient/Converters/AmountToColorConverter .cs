using System.Globalization;

namespace BusinessSharkClient.Converters
{
    public class AmountToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double data)
            {
                if (data < 0)
                    return Colors.Red;
                else if (data > 0)
                    return Colors.Green;
                else
                    return Colors.Black;
            }

            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
