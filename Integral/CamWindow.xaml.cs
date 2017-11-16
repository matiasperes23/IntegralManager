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
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Win32;
using Integral.Handlers;
using WebEye.Controls.Wpf;
using System.Drawing;

namespace Integral
{
    /// <summary>
    /// Interaction logic for CamWindow.xaml
    /// </summary>
    public partial class CamWindow : Window
    {
        private bool mostrandoFoto;


        private byte[] imageBytes;

        public  byte[] ImageBytes { get { return imageBytes; } }

        public Bitmap ImageBitmap { get; set; }

        public ImageSource ImageSource { get; set; }

        private string imagePath, imageDirectory;
        public CamWindow()
        {
            InitializeComponent();

            InitializeComboBox();

            // Create directory for saving image files
            imageDirectory  = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "IntegralManager"); 
            imagePath       = System.IO.Path.Combine(imageDirectory, "IntegralSnapshot.png");

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }
        }

        private void InitializeComboBox()
        {
            VideoDevicesComboBox.ItemsSource = WebCameraControl.GetVideoCaptureDevices();
        }

        

        private void buttonTomarFoto_Click(object sender, RoutedEventArgs e)
        {
            // Take snapshot of webcam video.
            if (!mostrandoFoto)
            {
                ImageBitmap = WebCameraControl.GetCurrentImage();
                ImageBitmap = FotosHandler.Instancia.GetSquareImage(ImageBitmap, new System.Drawing.Size { Height = 480, Width = 480 });
                ImageSource = FotosHandler.Instancia.ImageSourceFromBitmap(ImageBitmap);
                imageSocio.Source = ImageSource;

                imageSocio.Visibility       = System.Windows.Visibility.Visible;
                WebCameraControl.Visibility = System.Windows.Visibility.Hidden;
                mostrandoFoto = true;

                WebCameraControl.StopCapture();
                this.DialogResult = true;
                //buttonTomarFoto.Content = "Retomar";
            }
            else
            {
                try
                {
                    // Display webcam video
                    imageSocio.Visibility = System.Windows.Visibility.Hidden;
                    WebCameraControl.Visibility = System.Windows.Visibility.Visible;
                    mostrandoFoto = false;

                    buttonTomarFoto.Content = "Tomar Foto";

                    WebCameraControl.StartCapture((WebCameraId)VideoDevicesComboBox.SelectedItem);  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error: " + ex.Message);
                }
            }
        }

        private void buttonCargarFoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Elija una foto";
                op.Filter = "Todos los formatos soportados|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png";
                if (op.ShowDialog() == true)
                {
                    imageBytes = FotosHandler.Instancia.LoadImageData(op.FileName);

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        ImageBitmap = new Bitmap(ms);
                    }
                    ImageBitmap = FotosHandler.Instancia.GetSquareImage(ImageBitmap, new System.Drawing.Size { Height = 480, Width = 480 });
                    ImageSource = FotosHandler.Instancia.ImageSourceFromBitmap(ImageBitmap);
                    imageSocio.Source = ImageSource;

                    imageSocio.Visibility   = System.Windows.Visibility.Visible;
                    WebCameraControl.Visibility = System.Windows.Visibility.Hidden;
                    if (!mostrandoFoto)
                    {
                        mostrandoFoto = true;
                        WebCameraControl.StopCapture();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar imágen.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void VideoDevicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Display webcam video
                imageSocio.Visibility = System.Windows.Visibility.Hidden;
                WebCameraControl.Visibility = System.Windows.Visibility.Visible;
                mostrandoFoto = false;
                WebCameraControl.StartCapture((WebCameraId)VideoDevicesComboBox.SelectedItem);       
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error: " + ex.Message);
                Logger.Log(ex.ToString());
            }
        }

        private void buttonElegirFoto_Click(object sender, RoutedEventArgs e)
        {
            if (mostrandoFoto)
                this.DialogResult = true;
            else
                this.DialogResult = false;
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!mostrandoFoto)
                WebCameraControl.StopCapture();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VideoDevicesComboBox.Items.Count > 0)
                {
                    VideoDevicesComboBox.SelectedItem = VideoDevicesComboBox.Items[0];
                }

                // Display webcam video
                mostrandoFoto = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error: " + ex.Message);
                Logger.Log(ex.ToString());
            }
        }

        private void WebCameraControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

           
    }
}
