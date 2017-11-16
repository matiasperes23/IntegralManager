using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.Converters
{
    [System.Windows.Data.ValueConversion(typeof(DateTime?), typeof(string))]
    class MesPagoConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-ES");
            string s = String.Format(cultura, "{0:y}", value);
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
