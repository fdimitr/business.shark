using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSharkClient.Converters
{
    public class SectionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = value as string;
            var current = parameter as string;

            return selected == current
                ? new Color(0.2f, 0.6f, 1.0f, 0.3f) // голубоватый фон, если выбрано
                : Colors.Transparent;           // прозрачный, если нет
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
