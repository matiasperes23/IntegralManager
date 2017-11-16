using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Integral.DataTypes;

namespace Integral
{
    public partial class Socio
    {
        public DataSocioPreview ObtenerDataSocioPreview()
        {
            DateTime? fechaUltimoPago = null;
            if(Pagos.Count > 0){
                fechaUltimoPago = Pagos.OrderByDescending(p => p.Fecha).First().Fecha;
            }

            return new DataSocioPreview
            {
                Id = Id,
                Nombre = Nombre,
                Disciplinas = DisciplinasString,
                FechaInscripcion = FechaInscripcion,
                FechaUltimoPago = fechaUltimoPago,
                Atrasado = Atrasado
            };
        }

        public string DisciplinasString
        {
            get
            {
                if (Disciplinas.Count > 0)
                    return string.Join(", ", from dsc in Disciplinas.OrderBy(d => d.Nombre) select dsc.Nombre);
                else
                    return "Ninguna";
            }
        }

        public bool Atrasado
        {
            // Un Socio esta atrasado si esta atrasado en alguna de sus disciplinas
            // Esto es, si no se le registra un pago en el mes actual "o mayor" para la disciplina y 
            // ademas ya paso la fecha de pago (fecha de inscripcion)
            get
            {        
                DateTime hoy = DateTime.Today;
                DateTime mesAnterior = hoy.AddMonths(-1);

                IEnumerable<Pago> pagosMesActual = from p in Pagos
                                                   where (p.MesPago.Month == hoy.Month) && (p.MesPago.Year == hoy.Year)
                                                   select p;
                
                IEnumerable<Pago> pagosMesAnterior = null;
 
                foreach (Disciplina dsc in Disciplinas)
                {
                    if(!pagosMesActual.Any(p => p.Montos.Any(m => m.DisciplinaId == dsc.Id)))
                    {
                        if (FechaInscripcion.Day < hoy.Day)
                            return true;
                        else
                        {
                            if (pagosMesAnterior == null)
                            {
                                pagosMesAnterior = from p in Pagos
                                                   where (p.MesPago.Month == mesAnterior.Month) && (p.MesPago.Year == mesAnterior.Year)
                                                   select p;
                            }

                            if (!pagosMesAnterior.Any(p => p.Montos.Any(m => m.DisciplinaId == dsc.Id)))
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public string NoPagoDcsMesAnterior(DateTime mesAPagar)
        {
            DateTime mesAnterior = mesAPagar.AddMonths(-1);

            IEnumerable<Pago> pagosMesActual = from p in Pagos
                                               where (p.MesPago.Month == mesAPagar.Month) && (p.MesPago.Year == mesAPagar.Year)
                                                select p;

            if (pagosMesActual.Count() > 0)
                return null;


            IEnumerable<Pago> pagosMesAnterior = from p in Pagos
                                                    where (p.MesPago.Month == mesAnterior.Month) && (p.MesPago.Year == mesAnterior.Year)
                                                    select p;

            foreach (Disciplina dsc in Disciplinas.OrderBy(dsc => dsc.Nombre))
            {
                    if (!pagosMesAnterior.Any(p => p.Montos.Any(m => m.DisciplinaId == dsc.Id)))
                    return dsc.Nombre;
            }

            return null;
       }
    }
}
