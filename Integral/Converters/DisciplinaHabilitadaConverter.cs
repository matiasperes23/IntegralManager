using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.Converters
{
    [System.Windows.Data.ValueConversion(typeof(bool), typeof(string))]
    class DisciplinaHabilitadaConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return "Si";
            else
                return "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
