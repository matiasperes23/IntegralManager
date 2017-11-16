using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace Integral.Converters
{
    [ValueConversion(typeof(int), typeof(BitmapImage))]
    public class ImagePathConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string directory = Directory.GetCurrentDirectory();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-ES");
                string id_s = ((long)value).ToString(cultura);
                string img_path = Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                if (!File.Exists(img_path))
                    image.UriSource = new Uri(Path.Combine(directory, @"imagenes\default_user_300.dat"));
                else
                    image.UriSource = new Uri(img_path);

                image.EndInit();
                return image;

            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
