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
using Integral.Handlers;
using Microsoft.Win32;

namespace Integral
{
    /// <summary>
    /// Interaction logic for FichaSocio.xaml
    /// </summary>
    public partial class FichaSocio : Window
    {
        private Socio socio;

        public bool? Actualizar { get; set; }

        public FichaSocio(Socio socio)
        {
            InitializeComponent();

            this.socio = socio;
            Title = "Ficha de " + socio.Nombre;
            gridDatosSocio.DataContext = socio;

            try
            {
                string directory = System.IO.Directory.GetCurrentDirectory();
                string id_s = (socio.Id).ToString();
                string img_path = System.IO.Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");

                if (!System.IO.File.Exists(img_path))
                    buttonCargarImagen.Visibility = System.Windows.Visibility.Visible;
                else
                    buttonCargarImagen.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error cargar imágen.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }

            actualizarListaPagos();
            listaPagos.SelectedItems.Clear();       
            buttonDetallesPago.IsEnabled = false;


            if (socio.Disciplinas.Count == 0) // Socio inactivo
            {
                buttonNuevoPago.IsEnabled = false;
            }
        }

        private void listaPagos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaPagos.SelectedValue != null)
                buttonDetallesPago.IsEnabled = true;
            else
                buttonDetallesPago.IsEnabled = false;
        }

        private void buttonNuevoPago_Click(object sender, RoutedEventArgs e)
        {
            Pago ultimoPago = socio.Pagos.OrderByDescending(p => p.MesPago).FirstOrDefault();
            bool? actualizar = new PagoWindow(socio, ultimoPago, null).ShowDialog();
            if (actualizar.HasValue && actualizar.Value)
            {
                try
                {
                    socio = SociosHandler.Instancia.ObtenerSocio(socio.Id);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al cargar datos de socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
                actualizarListaPagos();
                Actualizar = true;
            }

        }

        private void buttonDetallesPago_Click(object sender, RoutedEventArgs e)
        {
            bool? actualizar = new PagoWindow(socio, null, (Pago)listaPagos.SelectedItem).ShowDialog();
            if (actualizar.HasValue && actualizar.Value)
            {
                try
                {
                    socio = SociosHandler.Instancia.ObtenerSocio(socio.Id);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al cargar datos de socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
                actualizarListaPagos();
                Actualizar = true;
            }
        }

        private void actualizarListaPagos()
        {
            listaPagos.ItemsSource = socio.Pagos.OrderByDescending(p => p.MesPago);
            if (listaPagos.Items.Count == 1)
                labelPagosRegistrados.Content = "1 Pago Registrado:";
            else
                labelPagosRegistrados.Content = listaPagos.Items.Count.ToString() + " Pagos Registrados:";
        }

        private void buttonCargarImagen_Click(object sender, RoutedEventArgs e)
        {
            var camWindow = new CamWindow();

            if (camWindow.ShowDialog() == true)
            {
                var imageBitmap = camWindow.ImageBitmap;
                imageSocio.Source = camWindow.ImageSource;
                string directory = System.IO.Directory.GetCurrentDirectory();
                string id_s = (socio.Id).ToString();
                string img_path = System.IO.Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");
                imageBitmap.Save(img_path);
                Actualizar = true;
                buttonCargarImagen.IsEnabled = false;
                buttonCargarImagen.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void buttonModificarFicha_Click(object sender, RoutedEventArgs e)
        {         
            try
            {
                SocioWindow socioWindow = new SocioWindow(SociosHandler.Instancia.ObtenerSocio(socio.Id));
                socioWindow.ShowDialog();

                if (socioWindow.Actualizar.HasValue && socioWindow.Actualizar.Value)
                {
                    socio = SociosHandler.Instancia.ObtenerSocio(socio.Id);
                    Title = "Ficha de " + socio.Nombre;
                    gridDatosSocio.DataContext = socio;
                    if (socio.Disciplinas.Count == 0) // Socio inactivo
                    {
                        buttonNuevoPago.IsEnabled = false;
                    }
                    else
                    {
                        buttonNuevoPago.IsEnabled = true;
                    }
                    Actualizar = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar datos de socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }

        }
    }
}
