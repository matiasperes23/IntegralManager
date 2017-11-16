using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Integral.Handlers;

namespace Integral
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Elija una foto";
            op.Filter = "Todos los formatos soportados|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                byte[] imageBytes = FotosHandler.Instancia.LoadImageData(op.FileName);

                ImageSource imageSource = FotosHandler.Instancia.CreateImage(imageBytes, 256, 0);
                imageBytes = FotosHandler.Instancia.GetEncodedImageData(imageSource, ".png");

                imageSource = FotosHandler.Instancia.CreateImage(imageBytes, 0, 256);
                imageBytes = FotosHandler.Instancia.GetEncodedImageData(imageSource, ".png");

                FotosHandler.Instancia.SaveImageData(imageBytes, @"imagenes\1.png");

                imgPhoto.Source = imageSource;
            }
        }
    }
}
