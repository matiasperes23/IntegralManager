using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Integral.DataTypes;

namespace Integral.Handlers
{
    public class SociosHandler
    {
        private static readonly SociosHandler instancia = new SociosHandler();

        private SociosHandler() { }

        public static SociosHandler Instancia
        {
            get { return instancia; }
        }

        private DataSocioPreview dataSocioPreview(Socio socio)
        {
            return socio.ObtenerDataSocioPreview();
        }

        public IList<DataSocioPreview> ObtenerDSP()
        {
            ObservableCollection<DataSocioPreview> dsp = new ObservableCollection<DataSocioPreview>();
            using (var ctx = new integralDataEntities())
            {

                var qry = from soc in ctx.Socios
                          orderby soc.Nombre
                          select soc;

                foreach (var socio in qry)
                {
                    dsp.Add(socio.ObtenerDataSocioPreview());
                }

                return dsp;
            }
        }

        public IList<DataSocioPreview> ObtenerDSPActivos()
        {
            ObservableCollection<DataSocioPreview> dspActivos = new ObservableCollection<DataSocioPreview>();
            using (var ctx = new integralDataEntities())
            {

                var qry = from soc in ctx.Socios
                          where soc.Disciplinas.Count > 0
                          orderby soc.Nombre
                          select soc;

                foreach (var socio in qry)
                {
                    dspActivos.Add(socio.ObtenerDataSocioPreview());
                }

                return dspActivos;
            }
        }

        public IList<DataSocioPreview> ObtenerDSPInactivos()
        {
            ObservableCollection<DataSocioPreview> dspInactivos = new ObservableCollection<DataSocioPreview>();
            using (var ctx = new integralDataEntities())
            {

                var qry = from soc in ctx.Socios
                          where soc.Disciplinas.Count == 0
                          orderby soc.Nombre
                          select soc;

                foreach (var socio in qry)
                {
                    dspInactivos.Add(socio.ObtenerDataSocioPreview());
                }

                return dspInactivos;
            }
        }

        // Procedimiento no utilizado
        public IList<DataSocioPreview> ObtenerDSPFiltrado(long disId, string nombre)
        {
            ObservableCollection<DataSocioPreview> dspActivos = new ObservableCollection<DataSocioPreview>();
            using (var ctx = new integralDataEntities())
            {
                if (disId > 0)
                {
                    var qry = from soc in ctx.Socios
                              where soc.Disciplinas.Any(dis => dis.Id == disId) && (String.IsNullOrEmpty(nombre) || soc.Nombre.ToLower().Contains(nombre.ToLower()))
                              orderby soc.Nombre
                              select soc;

                    foreach (var socio in qry)
                    {
                        dspActivos.Add(socio.ObtenerDataSocioPreview());
                    }
                }
                else
                {
                    var qry = from soc in ctx.Socios
                              where (String.IsNullOrEmpty(nombre) || soc.Nombre.ToLower().Contains(nombre.ToLower()))
                              orderby soc.Nombre
                              select soc;

                    foreach (var socio in qry)
                    {
                        dspActivos.Add(socio.ObtenerDataSocioPreview());
                    }
                }

                return dspActivos;
            }
        }

        public IList<Socio> ObtenerSociosActivos()
        {
            using (var ctx = new integralDataEntities())
            {

                var qry = from soc in ctx.Socios
                          where soc.Disciplinas.Count > 0
                          orderby soc.Nombre
                          select soc;

                return new ObservableCollection<Socio>(qry.ToList());
            }
        }

        public IList<DataSocioPreview> ObtenerDSPAtrasados()
        {
            List<DataSocioPreview> sociosAtrasados = new List<DataSocioPreview>();
            using (var ctx = new integralDataEntities())
            {
                var qry = from soc in ctx.Socios
                          where soc.Disciplinas.Count > 0
                          select soc;

                foreach (var socio in qry)
                {
                    DataSocioPreview dsp = socio.ObtenerDataSocioPreview();
                    if(dsp.Atrasado)
                        sociosAtrasados.Add(dsp);
                }

                return sociosAtrasados.OrderBy(sa => sa.FechaUltimoPago).ToList();
            }
        }

        public Socio AgregarSocio(Socio nuevoSocio)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.AddToSocios(nuevoSocio);

                ctx.SaveChanges();

                return nuevoSocio;
            }     
        }

        public void ModificarSocio(Socio socioModificado)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Socios.Attach(socioModificado);

                ctx.ObjectStateManager.ChangeObjectState(socioModificado, System.Data.EntityState.Modified);

                ctx.SaveChanges();
            }        
        }

        public void EliminarSocio(Socio socioEliminado)
        {
            using (var ctx = new integralDataEntities())
            {
                foreach (Pago pago in socioEliminado.Pagos.ToList())
                {
                    
                    foreach (Monto mon in pago.Montos.ToList())
                    {
                        ctx.Attach(mon);
                        ctx.DeleteObject(mon);
                    }

                    ctx.Attach(pago);
                    ctx.DeleteObject(pago);
                }

                ctx.Attach(socioEliminado);
                ctx.DeleteObject(socioEliminado);

                ctx.SaveChanges();
            }

            FotosHandler.Instancia.DeleteImageData(socioEliminado.Id);
        }

        public void Seed()
        {
            using (var ctx = new integralDataEntities())
            {
                var d1 = new Disciplina { Nombre = "Aparatos", Habilitada = true };
                var d2 = new Disciplina { Nombre = "Karate", Habilitada = true };
                var d3 = new Disciplina { Nombre = "Danza", Habilitada = true };
                
                ctx.AddToDisciplinas(d1);
                ctx.AddToDisciplinas(d2);
                ctx.AddToDisciplinas(d3);
                ctx.SaveChanges();
               
                var s1 = new Socio {    Nombre = "Matias Peres", FechaInscripcion = new DateTime(2013,1,1), Edad = 23,
                                        Direccion = "Segarra 4570bis", TelCel = "094303043", EmergenciaMedica = "Casmu" };
                var s2 = new Socio {    Nombre = "Alberto Peres", FechaInscripcion = new DateTime(2013,2,1), Edad = 40,
                                        Direccion = "Segarra 4570bis", TelCel = "098303043", EmergenciaMedica = "UCM" };
                
                ctx.AddToSocios(s1);
                ctx.AddToSocios(s2);
                ctx.SaveChanges();

                // Matias - Aparatos y Karate, Alberto - Karate, Jorge - Danza
                s1.Disciplinas.Add(d1);
                s1.Disciplinas.Add(d2);
                s2.Disciplinas.Add(d2);
                ctx.SaveChanges();


                for (int i = 0; i < 1; i++)
                {
                    var s3 = new Socio
                    {
                        Nombre = "Jorge Peres",
                        FechaInscripcion = new DateTime(2012, 5, 1),
                        Edad = 50,
                        Direccion = "Segarra 4570bis",
                        TelCel = "098303043",
                        EmergenciaMedica = "UCM"
                    };
                    ctx.AddToSocios(s3);
                    s3.Disciplinas.Add(d3);
                    ctx.SaveChanges();
                    var p3 = new Pago { MesPago = new DateTime(2012, 5, 1), MontoTotal = 460, Fecha = new DateTime(2012, 5, 1) };
                    s3.Pagos.Add(p3);
                    var m31 = new Monto { MontoParcial = 460, DisciplinaId = d3.Id, PagoId = p3.Id };
                    ctx.AddToMontos(m31);
                    ctx.SaveChanges();
                }

                var p1 = new Pago { MesPago = new DateTime(2013,1,1), MontoTotal = 920, Fecha = new DateTime(2013,1,1) };
                var p2 = new Pago { MesPago = new DateTime(2013,2,1), MontoTotal = 460, Fecha = new DateTime(2013,2,1) };
                

                s1.Pagos.Add(p1);
                s2.Pagos.Add(p2);
                
                ctx.SaveChanges();

                var m11 = new Monto { MontoParcial = 460, DisciplinaId = d1.Id, PagoId = p1.Id };
                var m12 = new Monto { MontoParcial = 460, DisciplinaId = d2.Id, PagoId = p1.Id };
                var m21 = new Monto { MontoParcial = 460, DisciplinaId = d2.Id, PagoId = p2.Id };
                
                ctx.AddToMontos(m11);
                ctx.AddToMontos(m12);
                ctx.AddToMontos(m21);
                
                ctx.SaveChanges();
            };
        }



        public Socio ObtenerSocio(long socioId)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Socios
                    .Include("Disciplinas")
                    .Include("Pagos.Montos")
                    .Include("Pagos.Montos.Disciplina")
                    .Where(s => s.Id == socioId)
                    .FirstOrDefault();
            }
        }

        public string SocioNoPagoDscMesAnterior(long socioId, DateTime mesAPagar)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Socios
                    .Where(s => s.Id == socioId)
                    .FirstOrDefault()
                    .NoPagoDcsMesAnterior(mesAPagar);
            }
        }

        public int ObtenerCantSociosRegistrados(DateTime fechaDe, DateTime fechaA)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Socios
                    .Where(s => s.FechaInscripcion >= fechaDe && s.FechaInscripcion <= fechaA)
                    .Count();
            }
        }
    }
}
