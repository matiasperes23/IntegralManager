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
    /// Interaction logic for DisciplinaWindow.xaml
    /// </summary>
    public partial class DisciplinaWindow : Window
    {

        public bool? Actualizar { get; set; }

        private Disciplina disciplina;

        public DisciplinaWindow(Disciplina disciplinaEdit)
        {
            InitializeComponent();

            if (disciplinaEdit == null) // Nueva Disciplina
            {
                disciplina = new Disciplina();
                disciplina.Habilitada = true;
            }
            else // Modificar Disciplina
            {
                disciplina = disciplinaEdit;
                this.Title = "Editando disciplina " + disciplinaEdit.Nombre;
                buttonIngresarDisciplina.Visibility = System.Windows.Visibility.Hidden;
                buttonCancelar.Visibility = System.Windows.Visibility.Hidden;
                buttonCancelar2.Visibility = System.Windows.Visibility.Visible;
                buttonGuardarCambios.Visibility = System.Windows.Visibility.Visible;
            }

            gridDisciplina.DataContext = disciplina;
        }

        private void buttonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool chequeoPrecondiciones()
        {
            // true -> continuar
            // false -> no continuar

            //Chequeo de precondiciones
            //Se debe obligatoriamente escribir nombre.
            List<string> advertencias = new List<string>();

            if (string.IsNullOrEmpty(textBoxNombre.Text))
                advertencias.Add("Debe ingresar Nombre de la disciplina.");

            // Mensaje de Alto
            if (advertencias.Count > 0)
            {
                System.Windows.MessageBox.Show(string.Join("\n", advertencias), "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            advertencias = new List<string>();

            if (string.IsNullOrEmpty(textBoxProfesor.Text))
                advertencias.Add("No se ha ingresado Profesor.");

            if (string.IsNullOrEmpty(textBoxContactoProfesor.Text))
                advertencias.Add("No se ha ingresado Contacto Profesor.");

            // Mensaje de Confirmación
            if (advertencias.Count > 0)
            {
                return MessageBox.Show(string.Join("\n", advertencias) + "\n\n" + "Esta seguro que desea continuar?",
                    "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                    MessageBoxResult.No) == MessageBoxResult.Yes;
            }

            return true;
        }

        private void buttonGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            if (!chequeoPrecondiciones())
                return;

            try
            {
                DisciplinasHandler.Instancia.ModificarDisciplina(disciplina);
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

        private void buttonIngresarDisciplina_Click(object sender, RoutedEventArgs e)
        {
            if (!chequeoPrecondiciones())
                return;

            try
            {
                DisciplinasHandler.Instancia.AgregarDisciplina(disciplina);

                MessageBox.Show("La disciplina ha sido ingresada exitosamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al ingresar disciplina.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }

            Actualizar = true;
            this.Close();
        }
    }
}
