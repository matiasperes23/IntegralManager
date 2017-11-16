using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Integral.Handlers;

namespace Integral
{
    public partial class Disciplina
    {
        public int NumeroSocios
        {
            get
            {
                try
                {
                    return DisciplinasHandler.Instancia.NumeroSociosDisciplina(Id);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.ToString());
                    return 0;
                }
            }
        }

        public int NumeroMontos 
        {
            get
            {
                try
                {
                    return DisciplinasHandler.Instancia.NumeroMontosDisciplina(Id);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.ToString());
                    return 0;
                }
            }
        }
    }
}
