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

namespace Integral
{
    /// <summary>
    /// Interaction logic for SocioDisciplinaWindow.xaml
    /// </summary>
    public partial class SocioDisciplinaWindow : Window // Dar de baja socio - Dar de alta socio
    {
        private Socio socio;
        private bool bajaAlta;

        public bool? Actualizar { get; set; }

        public SocioDisciplinaWindow(Socio socio, bool bajaAlta) 
        {
            InitializeComponent();

            this.socio = socio;
            this.bajaAlta = bajaAlta;

            if (bajaAlta) // Dar de baja
            {
                this.Title = "Dar de baja: " + socio.Nombre;
                labelAltaBaja.Content = "Seleccione las disciplinas en las que desea dar de BAJA a " + socio.Nombre + ":";

                listBoxDisciplinas.ItemsSource = socio.Disciplinas.OrderBy(dsc => dsc.Nombre);
            }
            else // Dar de alta
            {
                this.Title = "Dar de alta: " + socio.Nombre;
                labelAltaBaja.Content = "Seleccione las disciplinas en las que desea dar de ALTA a " + socio.Nombre + ":";

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
        }

        private void buttonGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxDisciplinas.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("Debe seleccionar alguna disciplina para continuar.", 
                    "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (bajaAlta) // Dar de baja
            {
                List<string> advertencias = new List<string>();

                if (listBoxDisciplinas.SelectedItems.Count == socio.Disciplinas.Count)
                {
                    advertencias.Add("El socio será ingresado a la lista de socios inactivos.");
                }

                if (advertencias.Count > 0)
                {
                    if (MessageBox.Show(string.Join("\n", advertencias) + "\n\n" + "Esta seguro que desea continuar?",
                        "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                        MessageBoxResult.No) == MessageBoxResult.No)
                        return;
                }

                try
                {
                    foreach (Disciplina dsc in listBoxDisciplinas.SelectedItems)
                    {
                        DisciplinasHandler.Instancia.QuitarSocioDisciplina(dsc, socio);
                    }

                    MessageBox.Show("Los datos han sido modificados exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al ingresar socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }

            }
            else // Dar de alta
            {
                try
                {
                    foreach (Disciplina dsc in listBoxDisciplinas.SelectedItems)
                    {
                        DisciplinasHandler.Instancia.AgregarSocioDisciplina(dsc, socio);
                    }

                    MessageBox.Show("Los datos han sido modificados exitosamente. El socio ha sido ingresado a la lista de socios activos. ", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al ingresar socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }

            Actualizar = true;
            this.Close();
        }

        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
