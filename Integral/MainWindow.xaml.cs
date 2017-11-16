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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Integral.Converters;
using Integral.Handlers;
using Integral.DataTypes;
using System.Windows.Forms;
using System.IO;

namespace Integral
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //SociosHandler.Instancia.Seed();
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-ES");
            textBlockFecha.Text = String.Format(cultura, "{0:dd/MM/yyyy}", DateTime.Now);
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            cargarListaSociosAtrasados();
        }

        private void cargarListaSociosAtrasados()
        {
            try
            {
                lbSociosAtrasados.ItemsSource = SociosHandler.Instancia.ObtenerDSPAtrasados();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                System.Windows.MessageBox.Show("Ha ocurrido un error al cargar socios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miSobreIntegral_Click(object sender, RoutedEventArgs e)
        {
            new AboutIntegralWindow().ShowDialog();
        }

        private void miSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonSocios_Click(object sender, RoutedEventArgs e)
        {
            Socios sociosActivosWindow = new Socios();
            sociosActivosWindow.ShowDialog();
            if (sociosActivosWindow.Actualizar.HasValue && sociosActivosWindow.Actualizar.Value)
            {
                cargarListaSociosAtrasados();
            }
        }

        private void lbSociosAtrasados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSociosAtrasados.SelectedValue != null)
            {
                borderOpcSocio.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                borderOpcSocio.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void buttonVerFicha_Click(object sender, RoutedEventArgs e)
        {
            verFicha(((DataSocioPreview)lbSociosAtrasados.SelectedValue).Id);
        }

        private void buttonDarBaja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Socio socio = SociosHandler.Instancia.ObtenerSocio(((DataSocioPreview)lbSociosAtrasados.SelectedValue).Id);
                SocioDisciplinaWindow sdw = new SocioDisciplinaWindow(socio, true);
                sdw.ShowDialog();
                if (sdw.Actualizar.HasValue && sdw.Actualizar.Value)
                {
                    cargarListaSociosAtrasados();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void lbSociosAtrasados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbSociosAtrasados.SelectedItem != null)
            {
                verFicha(((DataSocioPreview)lbSociosAtrasados.SelectedValue).Id);
            }
        }

        private void verFicha(long socioId)
        {
            try
            {
                Socio socio = SociosHandler.Instancia.ObtenerSocio(socioId);
                lbSociosAtrasados.SelectedItem = null;
                FichaSocio fichaSocio = new FichaSocio(socio);
                fichaSocio.ShowDialog();
                if (fichaSocio.Actualizar.HasValue && fichaSocio.Actualizar.Value)
                {
                    cargarListaSociosAtrasados();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ha ocurrido un error al leer datos socio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
        }

        private void buttonNuevoSocio_Click(object sender, RoutedEventArgs e)
        {
            SocioWindow socioWindow = new SocioWindow(null);
            socioWindow.ShowDialog();
            if (socioWindow.Actualizar.HasValue && socioWindow.Actualizar.Value)
            {
                cargarListaSociosAtrasados();
                verFicha(socioWindow.SocioId);
            }
        }

        private void buttonSociosInactivos_Click(object sender, RoutedEventArgs e)
        {
            SociosInactivosWindow siw = new SociosInactivosWindow();
            siw.ShowDialog();
            if (siw.Actualizar.HasValue && siw.Actualizar.Value)
            {
                cargarListaSociosAtrasados();
            }
        }

        private void buttonAdminDisciplinas_Click(object sender, RoutedEventArgs e)
        {
            DisciplinasWindow disciplinasWindow = new DisciplinasWindow();
            disciplinasWindow.ShowDialog();
            if (disciplinasWindow.Actualizar.HasValue && disciplinasWindow.Actualizar.Value)
            {
                cargarListaSociosAtrasados();
            }
        }

        private void buttonBalance_Click(object sender, RoutedEventArgs e)
        {
            new BalanceWindow().ShowDialog();
        }

        private void miRespaldarBD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Seleccione una carpeta donde copiar la base de datos:";
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    progressBar.Visibility = System.Windows.Visibility.Visible;
                    string destDirName = fbd.SelectedPath + "\\IntegralDataBase";
                    // If the destination directory doesn't exist, create it.
                    if (!Directory.Exists(destDirName))
                    {
                        Directory.CreateDirectory(destDirName);
                    }
                    string destDirImgName = destDirName + "\\imagenes";
                    if (!Directory.Exists(destDirImgName))
                    {
                        Directory.CreateDirectory(destDirImgName);
                    }
                    progressBar.Value = 10;
                    string sourceDirName = Directory.GetCurrentDirectory();
                    // Copio las imagenes
                    DirectoryInfo dirImagenes = new DirectoryInfo(sourceDirName + "\\imagenes");
                    FileInfo[] files = dirImagenes.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        string temppath = System.IO.Path.Combine(destDirImgName, file.Name);
                        file.CopyTo(temppath, true);
                    }
                    progressBar.Value = 80;
                    File.Copy(sourceDirName + "\\integralData.db", destDirName + "\\integralData.db",true);
                    progressBar.Value = 100;
                    System.Windows.MessageBox.Show("La base de datos ha sido respaldada exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    progressBar.Visibility = System.Windows.Visibility.Hidden;
                    progressBar.Value = 0;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ha ocurrido un error al respaldar base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
            }
            


        }


        
    }
}
