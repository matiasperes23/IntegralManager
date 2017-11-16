using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral
{
    public partial class Pago
    {
        public string Disciplinas
        {
            get
            {
                return string.Join(", ", from monto in Montos.OrderBy(m => m.Disciplina.Nombre) select monto.Disciplina.Nombre);
            }
        }
    }
}
