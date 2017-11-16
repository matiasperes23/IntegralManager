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
    /// Interaction logic for DisciplinasWindow.xaml
    /// </summary>
    public partial class DisciplinasWindow : Window
    {

        public bool? Actualizar { get; set; }

        public DisciplinasWindow()
        {
            InitializeComponent();

            cargarListaDisciplinas();
        }

        private void cargarListaDisciplinas()
        {
            try
            {
                listaDisciplinas.ItemsSource = DisciplinasHandler.Instancia.ObtenerDisciplinas();
                listaDisciplinas.SelectedItems.Clear();
                buttonEliminar.IsEnabled = false;
                buttonModificar.IsEnabled = false;
                buttonInhabilitar.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar disciplinas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
                this.Close();
            }
        }

        private void listaDisciplinas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaDisciplinas.SelectedValue != null)
            {
                buttonEliminar.IsEnabled = true;
                buttonModificar.IsEnabled = true;
                buttonInhabilitar.IsEnabled = true;
                if (((Disciplina)listaDisciplinas.SelectedValue).Habilitada) 
                    buttonInhabilitar.Content = "Inhabilitar";
                else
                    buttonInhabilitar.Content = "Habilitar";             
            }
            else
            {
                buttonEliminar.IsEnabled = false;
                buttonModificar.IsEnabled = false;
                buttonInhabilitar.IsEnabled = false;
                buttonInhabilitar.Content = "Inhabilitar";      
            }
        }

        private void buttonNuevaDisciplina_Click(object sender, RoutedEventArgs e)
        {
            DisciplinaWindow disciplinaWindow = new DisciplinaWindow(null);
            disciplinaWindow.ShowDialog();
            if (disciplinaWindow.Actualizar.HasValue && disciplinaWindow.Actualizar.Value)
            {
                Actualizar = true;
                cargarListaDisciplinas();
            }
        }

        private void buttonEliminar_Click(object sender, RoutedEventArgs e)
        {

            Disciplina disciplina = (Disciplina)listaDisciplinas.SelectedValue;
            if (disciplina.NumeroSocios > 0)
            {
                System.Windows.MessageBox.Show("No es posible eliminar una disciplina con socios asociados.", "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            else if (disciplina.NumeroMontos > 0)
            {
                System.Windows.MessageBox.Show("No es posible eliminar una disciplina con pagos asociados.", "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            else
            {
                if (MessageBox.Show("Si elimina esta disciplina ya no podrá recuperar sus datos. \n\n Está seguro que desea continuar?",
                "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.No)
                    return;
            }

            try
            {
                DisciplinasHandler.Instancia.EliminarDisciplina(disciplina);
                cargarListaDisciplinas();
                Actualizar = true;
                MessageBox.Show("La disciplina ha sido eliminada del sistema exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al eliminar disciplina.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void buttonModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Disciplina disciplina = DisciplinasHandler.Instancia.ObtenerDisciplina(((Disciplina)listaDisciplinas.SelectedValue).Id);
                DisciplinaWindow disciplinaWindow = new DisciplinaWindow(disciplina);
                disciplinaWindow.ShowDialog();
                if (disciplinaWindow.Actualizar.HasValue && disciplinaWindow.Actualizar.Value)
                {
                    Actualizar = true;
                    cargarListaDisciplinas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar disciplina.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void buttonInhabilitar_Click(object sender, RoutedEventArgs e)
        {
            Disciplina disciplina = (Disciplina)listaDisciplinas.SelectedValue;
            if (disciplina.Habilitada)
            {
                if (disciplina.NumeroSocios > 0)
                {
                    if (MessageBox.Show("Si inhabilita esta disciplina, la misma será desvinculada de todos sus socios. \n\n Está seguro que desea continuar?",
                            "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                            MessageBoxResult.No) == MessageBoxResult.No)
                        return;

                    if (MessageBox.Show(disciplina.NumeroSocios.ToString() + " socios serán desvinculados de esta disciplina. \n\n Está seguro que desea continuar?",
                            "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,
                            MessageBoxResult.No) == MessageBoxResult.No)
                        return;
                }

                try
                {
                    DisciplinasHandler.Instancia.InhabilitarDisciplina(disciplina);
                    cargarListaDisciplinas();
                    Actualizar = true;
                    MessageBox.Show("La disciplina ha sido inhabilitada exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al inhabilitar disciplina.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }
            else
            {
                try
                {
                    DisciplinasHandler.Instancia.HabilitarDisciplina(disciplina);
                    cargarListaDisciplinas();
                    Actualizar = true;
                    MessageBox.Show("La disciplina ha sido habilitada exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al habilitar disciplina.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }

        }
    }
}
