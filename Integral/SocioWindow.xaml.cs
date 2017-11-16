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
using System.IO;
using Microsoft.Win32;
using System.Drawing;

namespace Integral
{
    /// <summary>
    /// Interaction logic for SocioWindow.xaml
    /// </summary>
    public partial class SocioWindow : Window // Nuevo Socio - Editar Socio
    {
        private Socio socio;
        //private byte[] imageBytes;
        private Bitmap imageBitmap;

        public bool? Actualizar { get; set; }
        public long SocioId { get; set; }

        public SocioWindow(Socio editSocio) 
        {
            InitializeComponent();

            if (editSocio == null) // nuevo socio
            {
                socio = new Socio();
                socio.Id = 0; // Para conversion de imagen
                socio.FechaInscripcion = DateTime.Now;
                gridDatosSocio.DataContext = this.socio;

                try
                {
                    listBoxDisciplinas.ItemsSource = DisciplinasHandler.Instancia.ObtenerDisciplinasHabilitadas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al cargar disciplinas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }
            else // modificar socio
            {
                this.socio = editSocio;
                gridDatosSocio.DataContext = socio;
                this.Title = "Editando ficha de " + socio.Nombre;
                buttonCancelar2.Visibility      = System.Windows.Visibility.Visible;
                buttonGuardarCambios.Visibility = System.Windows.Visibility.Visible;
                buttonCancelar.Visibility       = System.Windows.Visibility.Hidden;
                buttonIngresarSocio.Visibility  = System.Windows.Visibility.Hidden;

                try
                {

                    IList<Disciplina> listDisciplinas = DisciplinasHandler.Instancia.ObtenerDisciplinasHabilitadas();
                    listBoxDisciplinas.ItemsSource = listDisciplinas;
                    listBoxDisciplinas.SelectedItemsOverride = listDisciplinas
                        .Where(dsc => socio.Disciplinas
                            .Any(dsc2 => dsc2.Id == dsc.Id))
                            .ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al cargar disciplinas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }
        }

        private bool chequeoPrecondiciones()
        {
            // true -> continuar
            // false -> no continuar

            //Chequeo de precondiciones
            //Se debe obligatoriamente escribir nombre y apellido, fecha, y edad.
            List<string> advertencias = new List<string>();

            if (string.IsNullOrEmpty(textBoxNombre.Text))
                advertencias.Add("Debe ingresar Nombre y Apellido del nuevo socio.");

            if (string.IsNullOrEmpty(dtpFechaIngreso.Text) || !dtpFechaIngreso.Value.HasValue)
                advertencias.Add("Debe seleccionar Fecha de Ingreso.");

            if (string.IsNullOrEmpty(iupEdad.Text) || !iupEdad.Value.HasValue)
                advertencias.Add("Debe ingresar Edad del nuevo socio.");

            // Mensaje de Alto
            if (advertencias.Count > 0)
            {
                System.Windows.MessageBox.Show(string.Join("\n", advertencias), "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            advertencias = new List<string>();

            if (string.IsNullOrEmpty(textBoxDireccion.Text))
                advertencias.Add("No se ha ingresado Dirección.");

            if (string.IsNullOrEmpty(textBoxTelCel.Text))
                advertencias.Add("No se ha ingresado Tel. o Cel.");

            if (string.IsNullOrEmpty(textBoxEmergenciaMedica.Text))
                advertencias.Add("No se ha ingresado Emergencia Médica.");

            if (listBoxDisciplinas.SelectedItems.Count == 0)
                advertencias.Add("No se ha seleccionado ninguna Disciplina (el socio se ingresara a la lista de socios inactivos).");

            // Mensaje de Confirmación
            if (advertencias.Count > 0)
            {
                return MessageBox.Show(string.Join("\n", advertencias) + "\n\n" + "Esta seguro que desea continuar?",
                    "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                    MessageBoxResult.No) == MessageBoxResult.Yes;
            }

            return true;
        }

        private void buttonIngresarSocio_Click(object sender, RoutedEventArgs e)
        {
            if (!chequeoPrecondiciones())
                return;

            try
            {
                SociosHandler.Instancia.AgregarSocio(socio);

                foreach (Disciplina dsc in listBoxDisciplinas.SelectedItems)
                {
                    DisciplinasHandler.Instancia.AgregarSocioDisciplina(dsc,socio);
                }

                if (imageBitmap != null)
                {
                    string directory = System.IO.Directory.GetCurrentDirectory();
                    string id_s = (socio.Id).ToString();
                    string img_path = System.IO.Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");
                    imageBitmap.Save(img_path);
                }

                MessageBox.Show("El socio ha sido ingresado exitosamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al ingresar socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }

            Actualizar = true;
            SocioId = socio.Id;
            this.Close();
        }

        private void buttonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonTomarImagen_Click(object sender, RoutedEventArgs e)
        {
            var camWindow = new CamWindow();

            if (camWindow.ShowDialog() == true)
            {
                imageBitmap = camWindow.ImageBitmap;
                //imageBytes = camWindow.ImageBytes;
                imageSocio.Source = camWindow.ImageSource;
            }
        }

        private void buttonGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            if (!chequeoPrecondiciones())
                return;

            try
            {
                foreach (Disciplina dsc in socio.Disciplinas.ToList())
                {
                    DisciplinasHandler.Instancia.QuitarSocioDisciplina(dsc, socio);
                }

                foreach (Disciplina dsc in listBoxDisciplinas.SelectedItems)
                {
                    DisciplinasHandler.Instancia.AgregarSocioDisciplina(dsc, socio);
                }

                SociosHandler.Instancia.ModificarSocio(socio);

                if (imageBitmap != null)
                {
                    string directory = System.IO.Directory.GetCurrentDirectory();
                    string id_s = (socio.Id).ToString();
                    string img_path = System.IO.Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");
                    imageBitmap.Save(img_path);
                }

                MessageBox.Show("Los datos han sido modificados exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al guardar cambios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }

            Actualizar = true;
            this.Close();
        }
    }
}
