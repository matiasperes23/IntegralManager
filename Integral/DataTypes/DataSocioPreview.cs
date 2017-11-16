using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral.DataTypes
{
    public class DataSocioPreview
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Disciplinas { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public DateTime? FechaUltimoPago { get; set; }
        public bool Atrasado { get; set; }
    }
}
