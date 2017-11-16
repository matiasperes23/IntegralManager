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
using System.ComponentModel;
using Integral.DataTypes;

namespace Integral
{
    /// <summary>
    /// Interaction logic for Socios.xaml
    /// </summary>
    public partial class Socios : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private IList<DataSocioPreview> dspreviews;

        public bool? Actualizar { get; set; }

        public Socios()
        {
            InitializeComponent();

            // Agrego los dspreviews activos a la tabla
            try
            {  
                dspreviews = SociosHandler.Instancia.ObtenerDSPActivos();
                listaSocios.ItemsSource = dspreviews;
                listaSocios.SelectedItems.Clear();
                buttonVerFicha.IsEnabled = false;
                buttonDarBaja.IsEnabled = false;
                actualizarLabelResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar socios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
                this.Close();
            }
            
            // Agrego las disciplinas al combobox
            try
            {
                var disciplinas = DisciplinasHandler.Instancia.ObtenerDisciplinasHabilitadas();
                disciplinas.Insert(0, new Disciplina { Nombre = "Todas", Id = -1 });
                comboBoxDisciplinas.ItemsSource = disciplinas;
                WidthDisciplinas = 150;
                NotifyPropertyChanged("WidthDisciplinas"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar disciplinas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void listaSocios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaSocios.SelectedValue != null)
            {
                labelSocioSeleccionado.Content = "";
                gridSocioPreview.DataContext = listaSocios.SelectedItem;
                buttonVerFicha.IsEnabled = true;
                buttonDarBaja.IsEnabled = true;
            }
            else
            {
                gridSocioPreview.DataContext = null;
                labelSocioSeleccionado.Content = "No se selecciono ningún socio.";
                buttonVerFicha.IsEnabled = false;
                buttonDarBaja.IsEnabled = false;
            }
        }

        public double WidthDisciplinas { get; set; }

        private void filtrarListaSocios()
        {
            IList<DataSocioPreview> sociosFiltrados;
            string disciplina = ((Disciplina)comboBoxDisciplinas.SelectedItem).Nombre;
            if (((Disciplina)comboBoxDisciplinas.SelectedItem).Id != -1)
            {
                sociosFiltrados = dspreviews
                    .Where(soc =>
                        (String.IsNullOrEmpty(textBoxNombre.Text) 
                        ||soc.Nombre.IndexOf(textBoxNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        && soc.Disciplinas.Contains(disciplina)
                        ).ToList();
            }
            else
            {
                sociosFiltrados = dspreviews
                    .Where(soc =>
                        (String.IsNullOrEmpty(textBoxNombre.Text) 
                        || soc.Nombre.IndexOf(textBoxNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
            }
            
            listaSocios.ItemsSource = sociosFiltrados;
            listaSocios.SelectedItems.Clear();
            actualizarLabelResultados();
        }

        private void comboBoxDisciplinas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Disciplina)comboBoxDisciplinas.SelectedItem).Nombre != "Todas")
            {
                WidthDisciplinas = 0;
                NotifyPropertyChanged("WidthDisciplinas");
            }
            else
            {
                WidthDisciplinas = 150;
                NotifyPropertyChanged("WidthDisciplinas"); 
            }
            filtrarListaSocios();
        }

        private void textBoxNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            filtrarListaSocios();
        }

        private void buttonVerFicha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                verFicha(SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)listaSocios.SelectedValue).Id));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void listaSocios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listaSocios.SelectedValue != null)
            {
                try
                {
                    verFicha(SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)listaSocios.SelectedValue).Id));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }
        }

        private void verFicha(Socio socio) // Poco eficiente
        {
            FichaSocio ficha = new FichaSocio(socio);
            ficha.ShowDialog();
            if (ficha.Actualizar.HasValue && ficha.Actualizar.Value)
            {
                dspreviews = SociosHandler.Instancia.ObtenerDSPActivos();
                listaSocios.ItemsSource = dspreviews;
                filtrarListaSocios();
                Actualizar = true;
            }
        }

        private void buttonDarBaja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Socio socio = SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)listaSocios.SelectedValue).Id);
                SocioDisciplinaWindow sdw = new SocioDisciplinaWindow(socio,true);
                sdw.ShowDialog();
                if (sdw.Actualizar.HasValue && sdw.Actualizar.Value)
                {
                    dspreviews = SociosHandler.Instancia.ObtenerDSPActivos();
                    listaSocios.ItemsSource = dspreviews;
                    filtrarListaSocios();
                    Actualizar = true;
                    verFicha(SociosHandler.Instancia.ObtenerSocio(socio.Id));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }
    }
}
