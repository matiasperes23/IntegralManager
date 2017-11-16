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
using Integral.DataTypes;
using Integral.Handlers;

namespace Integral
{
    /// <summary>
    /// Interaction logic for SociosInactivosWindow.xaml
    /// </summary>
    public partial class SociosInactivosWindow : Window
    {
        public bool? Actualizar { get; set; }

        private IList<DataSocioPreview> dspreviews;

        public SociosInactivosWindow()
        {
            InitializeComponent();

            // Agrego los dspreviews inactivos a la tabla
            try
            {
                dspreviews = SociosHandler.Instancia.ObtenerDSPInactivos();
                listaSocios.ItemsSource = dspreviews;
                listaSocios.SelectedItems.Clear();
                buttonVerFicha.IsEnabled = false;
                buttonDarAlta.IsEnabled = false;
                buttonEliminarSocio.IsEnabled = false;
                actualizarLabelResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar socios inactivos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
                this.Close();
            }
        }

        private void actualizarLabelResultados()
        {
            if (listaSocios.Items.Count == 1)
                labelResultados.Content = "1 resultado.";
            else
                labelResultados.Content = listaSocios.Items.Count.ToString() + " resultados.";
        }

        

        private void buttonVerFicha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                verFicha(((DataSocioPreview)listaSocios.SelectedValue).Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void listaSocios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaSocios.SelectedValue != null)
            {
                labelSocioSeleccionado.Content = "";
                gridSocioPreview.DataContext = listaSocios.SelectedItem;
                buttonVerFicha.IsEnabled = true;
                buttonDarAlta.IsEnabled = true;
                buttonEliminarSocio.IsEnabled = true;
            }
            else
            {
                gridSocioPreview.DataContext = null;
                labelSocioSeleccionado.Content = "No se selecciono ningún socio.";
                buttonVerFicha.IsEnabled = false;
                buttonDarAlta.IsEnabled = false;
                buttonEliminarSocio.IsEnabled = false;
            }
        }

        private void listaSocios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listaSocios.SelectedValue != null)
            {
                verFicha(((DataSocioPreview)listaSocios.SelectedValue).Id);
            }
        }

        private void filtrarListaSocios()
        {
            listaSocios.ItemsSource = dspreviews
                    .Where(soc =>
                        (String.IsNullOrEmpty(textBoxNombre.Text)
                        || soc.Nombre.IndexOf(textBoxNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
            listaSocios.SelectedItems.Clear();
            actualizarLabelResultados();
        }

        private void verFicha(long socioId) // Poco eficiente
        {
            try
            {
                FichaSocio ficha = new FichaSocio(SociosHandler.Instancia.ObtenerSocio(socioId));
                ficha.ShowDialog();
                if (ficha.Actualizar.HasValue && ficha.Actualizar.Value)
                {
                    dspreviews = SociosHandler.Instancia.ObtenerDSPInactivos();
                    listaSocios.ItemsSource = dspreviews;
                    filtrarListaSocios();
                    Actualizar = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void textBoxNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrarListaSocios();
        }

        private void buttonDarAlta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Socio socio = SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)listaSocios.SelectedValue).Id);
                SocioDisciplinaWindow sdw = new SocioDisciplinaWindow(socio, false);
                sdw.ShowDialog();
                if (sdw.Actualizar.HasValue && sdw.Actualizar.Value)
                {
                    dspreviews = SociosHandler.Instancia.ObtenerDSPInactivos();
                    listaSocios.ItemsSource = dspreviews;
                    filtrarListaSocios();
                    Actualizar = true;
                    verFicha(socio.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void buttonEliminarSocio_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Si elimina este socio ya no podrá recuperar sus datos (información personal y pagos). \n\n Esta seguro que desea continuar?",
                    "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                    MessageBoxResult.No) == MessageBoxResult.No)
                return;

            try
            {
                Socio socio = SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)listaSocios.SelectedValue).Id);

                SociosHandler.Instancia.EliminarSocio(socio);

                dspreviews = SociosHandler.Instancia.ObtenerDSPInactivos();
                listaSocios.ItemsSource = dspreviews;
                filtrarListaSocios();
                Actualizar = true;
                MessageBox.Show("El socio ha sido eliminado del sistema exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al eliminar socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }
    }
}
