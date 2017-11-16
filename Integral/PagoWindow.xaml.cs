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
    /// Interaction logic for PagoWindow.xaml
    /// </summary>
    public partial class PagoWindow : Window
    {
        private double montoTotal = 0;
        private Socio socio;
        private Pago pago;
        private Pago ultimoPago;

        public PagoWindow(Socio socio, Pago ultimoPago, Pago pago)
        {
            InitializeComponent();

            this.socio = socio;
            this.ultimoPago = ultimoPago;
            if (pago == null) // Nuevo Pago
            {
                dtpMesPago.Value = DateTime.Now;
                dtpFechaIngreso.Value = DateTime.Now;

                foreach (Disciplina dsc in socio.Disciplinas.OrderBy(dsc => dsc.Nombre))
                {
                    double montoParcial = 0;
                    if(ultimoPago != null)
                    {
                        foreach(Monto mon in ultimoPago.Montos)
                            if(dsc.Id == mon.Disciplina.Id)
                                montoParcial = mon.MontoParcial;
                    }

                    listMontos.Items.Add(new Monto { DisciplinaId = dsc.Id, Disciplina = dsc, MontoParcial = montoParcial });
                }
                listMontos.SelectAll();
                listMontos.Focus();
            }
            else // Detalles de pago
            {
                //Deshabilitar controles
                dtpMesPago.IsEnabled = false;
                dtpFechaIngreso.IsEnabled = false;
                listMontos.IsEnabled = false;
                buttonCancelar.Visibility = System.Windows.Visibility.Hidden;
                buttonIngresarPago.Visibility = System.Windows.Visibility.Hidden;
                buttonCerrar.Visibility = System.Windows.Visibility.Visible;
                buttonEliminar.Visibility = System.Windows.Visibility.Visible;
                this.Title = "Pago Nº " + pago.Id.ToString();

                this.pago = pago;

                dtpMesPago.Value = pago.MesPago;
                dtpFechaIngreso.Value = pago.Fecha;

                foreach (Monto mon in pago.Montos)
                {
                    listMontos.Items.Add(mon);
                }
                listMontos.SelectAll();
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listMontos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            actualizarMontoTotal();
        }

        private void DoubleUpDown_LostFocus(object sender, RoutedEventArgs e)
        {
            Xceed.Wpf.Toolkit.DoubleUpDown dud = (Xceed.Wpf.Toolkit.DoubleUpDown)sender;
            if (string.IsNullOrEmpty(dud.Text)){
                dud.Value = dud.DefaultValue;
                dud.Text = 0.ToString("C2", new System.Globalization.CultureInfo("es-UY"));
            }
            
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            actualizarMontoTotal();
        }

        private void actualizarMontoTotal()
        {
            montoTotal = 0;
            foreach (Monto mon in listMontos.SelectedItems)
            {
                montoTotal += mon.MontoParcial;
            }
            labelMontoTotal.Content = "Monto Total = " + montoTotal.ToString("C2", new System.Globalization.CultureInfo("es-UY"));
        }

        private void buttonIngresarPago_Click(object sender, RoutedEventArgs e)
        {
            // Chequeo de precondiciones
            List<string> advertencias = new List<string>();

            if (string.IsNullOrEmpty(dtpMesPago.Text) || !dtpMesPago.Value.HasValue)
                advertencias.Add("Debe seleccionar mes de pago.");

            if (string.IsNullOrEmpty(dtpFechaIngreso.Text) || !dtpFechaIngreso.Value.HasValue)
                advertencias.Add("Debe seleccionar fecha de ingreso.");
            
            if (listMontos.SelectedItems.Count == 0)
                advertencias.Add("Debe seleccionar al menos una disciplina.");

            // Mensaje de Alto
            if (advertencias.Count > 0)
            {
                System.Windows.MessageBox.Show(string.Join("\n",advertencias), "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }


            advertencias = new List<string>();

            // No se selecciono alguna de las disciplinas
            if (listMontos.SelectedItems.Count < listMontos.Items.Count)
                advertencias.Add("No se han seleccionado todas las disciplinas.");
            // Uno de los montos a pagar es igual a 0
            foreach (Monto mon in listMontos.SelectedItems)
            {
                if (mon.MontoParcial == 0)
                {
                    advertencias.Add("El valor de uno de los montos es $U 0,00.");
                    break;
                }
            }
            // No se registraron pagos en el mes anterior para alguna de las disciplinas
            if (ultimoPago != null) // Solo chequeo si es un socio con pagos ingresados
            {
                String dsc = SociosHandler.Instancia.SocioNoPagoDscMesAnterior(socio.Id, dtpMesPago.Value.Value);
                if(dsc != null)
                   advertencias.Add("No se registran pagos en el mes pasado para la disciplina " + dsc + ".");
            }


            MessageBoxResult response = MessageBoxResult.Yes;
            // Mensaje de Confirmación
            if (advertencias.Count > 0)
            {
                response = MessageBox.Show(string.Join("\n", advertencias) + "\n\n" + "Esta seguro que desea ingresar pago?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question,MessageBoxResult.No);
            }

            if (response == MessageBoxResult.Yes) // Agrego el pago a la base
            {
                try
                {
                    Pago nuevoPago = new Pago { 
                        SocioId = socio.Id,
                        MesPago = dtpMesPago.Value.Value, 
                        MontoTotal = montoTotal, 
                        Fecha = dtpFechaIngreso.Value.Value
                    };

                    PagosHandler.Instancia.AgregarPago(nuevoPago);

                    foreach (Monto mon in listMontos.SelectedItems)
                    {
                        mon.PagoId = nuevoPago.Id;
                        mon.Disciplina = null;
                        PagosHandler.Instancia.AgregarMonto(mon);
                    }

                    // Una vez agregado el pago se actualiza la lista de pagos de la ficha del socio
                    MessageBox.Show("El pago ha sido ingresado exitosamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al ingresar pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }

                this.Close();
            }  
        }

        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show("Esta seguro que desea eliminar pago?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (response == MessageBoxResult.Yes) // Elimino el pago de la base
            {
                try
                {
                    PagosHandler.Instancia.EliminarPago(pago);

                    // Una vez eliminado el pago se actualiza la lista de pagos de la ficha del socio
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha ocurrido un error al eliminar pago.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(ex.ToString());
                }
            }
        }
    }
}
