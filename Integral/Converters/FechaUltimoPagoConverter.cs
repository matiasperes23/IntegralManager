using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.Converters
{
    [System.Windows.Data.ValueConversion(typeof(DateTime?), typeof(string))]
    class FechaUltimoPagoConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-ES");
            DateTime? valor = (DateTime?)value;
            if (!valor.HasValue)
                return "No se registran pagos";
            else
                return String.Format(cultura, "{0:dd MMMM, yyyy}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
