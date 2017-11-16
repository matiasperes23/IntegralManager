using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.Converters
{
    [System.Windows.Data.ValueConversion(typeof(double), typeof(string))]
    class MontoConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-UY");
            return ((double)value).ToString("C2", cultura);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
