using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.Handlers
{
    public class PagosHandler
    {
        private static readonly PagosHandler instancia = new PagosHandler();

        private PagosHandler() { }

        public static PagosHandler Instancia
        {
            get { return instancia; }
        }

        public Pago AgregarPago(Pago nuevoPago)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.AddToPagos(nuevoPago);

                ctx.SaveChanges();

                return nuevoPago;
            }    
        }

        public Monto AgregarMonto(Monto nuevoMonto)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.AddToMontos(nuevoMonto);

                ctx.SaveChanges();

                return nuevoMonto;
            }   
        }

        public void EliminarPago(Pago pagoAEliminar)
        {
            using (var ctx = new integralDataEntities())
            {
                foreach (Monto mon in pagoAEliminar.Montos.ToList())
                {
                    ctx.Attach(mon);
                    ctx.DeleteObject(mon);
                }

                ctx.Attach(pagoAEliminar);
                ctx.DeleteObject(pagoAEliminar);

                ctx.SaveChanges();
            }
        }

        public IList<Monto> ObtenerMontosFiltrados(long? socioId, long? disciplinaId, DateTime fechaDe, DateTime fechaA)
        {
            using (var ctx = new integralDataEntities())
            {
                if (socioId.HasValue && disciplinaId.HasValue)
                {
                    var qry = from m in ctx.Montos.Include("Disciplina").Include("Pago.Socio")
                              where m.Pago.SocioId == socioId && m.DisciplinaId == disciplinaId
                              && m.Pago.MesPago >= fechaDe && m.Pago.MesPago <= fechaA
                              select m;
                    return qry.ToList();
                }
                else if (socioId.HasValue)
                {
                    var qry = from m in ctx.Montos.Include("Disciplina").Include("Pago.Socio")
                              where m.Pago.SocioId == socioId
                              && m.Pago.MesPago >= fechaDe && m.Pago.MesPago <= fechaA
                              select m;
                    return qry.ToList();
                }
                else if (disciplinaId.HasValue)
                {
                    var qry = from m in ctx.Montos.Include("Disciplina").Include("Pago.Socio")
                              where m.DisciplinaId == disciplinaId
                              && m.Pago.MesPago >= fechaDe && m.Pago.MesPago <= fechaA
                              select m;
                    return qry.ToList();
                }
                else
                {
                    var qry = from m in ctx.Montos.Include("Disciplina").Include("Pago.Socio")
                              where m.Pago.MesPago >= fechaDe && m.Pago.MesPago <= fechaA
                              select m;
                    return qry.ToList();
                }

            }


        }
    }
}
