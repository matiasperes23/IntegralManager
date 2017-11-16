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
using Integral.DataTypes;

namespace Integral
{
    /// <summary>
    /// Interaction logic for BalanceWindow.xaml
    /// </summary>
    public partial class BalanceWindow : Window
    {
        private DateTime fechaA, fechaDe;

        public BalanceWindow()
        {
            InitializeComponent();

            dtpMesPagoA.Value = DateTime.Now;
            dtpMesPagoDe.Value = DateTime.Now;

            // Agrego los socios al combobox
            try
            {
                var socios = SociosHandler.Instancia.ObtenerDSP();
                socios.Insert(0, new DataSocioPreview { Nombre = "Todos", Id = -1 });
                comboBoxSocios.ItemsSource = socios;
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
                var disciplinas = DisciplinasHandler.Instancia.ObtenerDisciplinas();
                disciplinas.Insert(0, new Disciplina { Nombre = "Todas", Id = -1 });
                comboBoxDisciplinas.ItemsSource = disciplinas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al cargar disciplinas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(ex.ToString());
                this.Close();
            }



        }

        private void buttonFiltrar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(dtpMesPagoDe.Text) || !dtpMesPagoDe.Value.HasValue
                || string.IsNullOrEmpty(dtpMesPagoA.Text) || !dtpMesPagoA.Value.HasValue)
            {
                System.Windows.MessageBox.Show("Debe seleccionar mes de pago.", "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            long? socioId;
            if(comboBoxSocios.SelectedValue == null ||
                ((DataSocioPreview)comboBoxSocios.SelectedValue).Id == -1)
                socioId = null;
            else
                socioId = ((DataSocioPreview)comboBoxSocios.SelectedValue).Id;

            long? dscId;
            if(((Disciplina)comboBoxDisciplinas.SelectedValue).Id == -1)
                dscId = null;
            else
                dscId = ((Disciplina)comboBoxDisciplinas.SelectedValue).Id;


            fechaDe = new DateTime(dtpMesPagoDe.Value.Value.Year,dtpMesPagoDe.Value.Value.Month,1);
            fechaA = new DateTime(dtpMesPagoA.Value.Value.Year, dtpMesPagoA.Value.Value.Month, 1);
            fechaA = fechaA.AddMonths(1);
            fechaA = fechaA.AddSeconds(-1);

            listaPagosDesglosados.ItemsSource = PagosHandler
                .Instancia.ObtenerMontosFiltrados(socioId, dscId, fechaDe, fechaA);

            actualizarLabels();
        }

        private void actualizarLabels()
        {
            labelPagosDesglosados.Content = listaPagosDesglosados.Items.Count.ToString() + " Pagos Desglosados:";
            double montoTotal = 0;
            HashSet<long> sociosIds = new HashSet<long>();
            foreach(Monto monto in listaPagosDesglosados.Items)
            {
                montoTotal += monto.MontoParcial;
                sociosIds.Add(monto.Pago.SocioId);
            }

            labelCantSociosRegistrados.Content = "Socios Registrados: " + SociosHandler.Instancia.ObtenerCantSociosRegistrados(fechaDe, fechaA);
            labelMontoTotal.Content = "Monto Total: " + montoTotal.ToString("C2", new System.Globalization.CultureInfo("es-UY"));
            labelSociosContemplados.Content = "Socios Contemplados: " + sociosIds.Count.ToString();
        }
    }

    
}
