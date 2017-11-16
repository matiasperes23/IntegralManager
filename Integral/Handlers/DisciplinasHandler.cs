using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Integral.Handlers
{
    public class DisciplinasHandler
    {
        private static readonly DisciplinasHandler instancia = new DisciplinasHandler();

        private DisciplinasHandler() { }

        public static DisciplinasHandler Instancia
        {
            get { return instancia; }
        }

        public IList<Disciplina> ObtenerDisciplinasHabilitadas()
        {
            using (var ctx = new integralDataEntities())
            {

                var qry = from dis in ctx.Disciplinas
                          where dis.Habilitada
                          orderby dis.Nombre
                          select dis;

                return new ObservableCollection<Disciplina>(qry.ToList());
            }
        }

        public IList<Disciplina> ObtenerDisciplinas()
        {
            using (var ctx = new integralDataEntities())
            {

                var qry = from dis in ctx.Disciplinas
                          orderby dis.Nombre
                          select dis;

                return new ObservableCollection<Disciplina>(qry.ToList());
            }
        }

        public void AgregarSocioDisciplina(Disciplina dsc, Socio socio)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Attach(dsc); 

                dsc.Socios.Add(ctx.Socios.Where(s => s.Id == socio.Id).FirstOrDefault());

                ctx.SaveChanges();
            }
        }

        public void QuitarSocioDisciplina(Disciplina dsc, Socio socio)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Attach(dsc); ctx.Attach(socio);

                dsc.Socios.Remove(socio);

                ctx.SaveChanges();
            }
        }

        public Disciplina ObtenerDisciplina(long dscId)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Disciplinas
                    .Where(dsc => dsc.Id == dscId)
                    .FirstOrDefault();
            }
        }

        public void ModificarDisciplina(Disciplina disciplina)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Disciplinas.Attach(disciplina);

                ctx.ObjectStateManager.ChangeObjectState(disciplina, System.Data.EntityState.Modified);

                ctx.SaveChanges();
            } 
        }

        public Disciplina AgregarDisciplina(Disciplina disciplina)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.AddToDisciplinas(disciplina);

                ctx.SaveChanges();

                return disciplina;
            }  
        }

        public int NumeroSociosDisciplina(long dscId)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Disciplinas
                    .Where(dsc => dsc.Id == dscId)
                    .FirstOrDefault().Socios.Count;
            }  
        }

        public void EliminarDisciplina(Disciplina disciplina)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Attach(disciplina);

                ctx.DeleteObject(disciplina);

                ctx.SaveChanges();
            }
        }

        public void InhabilitarDisciplina(Disciplina disciplina)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Attach(disciplina);
                disciplina.Socios.Clear();
                disciplina.Habilitada = false;

                ctx.SaveChanges();
            }
        }

        public void HabilitarDisciplina(Disciplina disciplina)
        {
            using (var ctx = new integralDataEntities())
            {
                ctx.Attach(disciplina);
                disciplina.Habilitada = true;

                ctx.SaveChanges();
            }
        }


        public int NumeroMontosDisciplina(long dscId)
        {
            using (var ctx = new integralDataEntities())
            {
                return ctx.Disciplinas
                    .Where(dsc => dsc.Id == dscId)
                    .FirstOrDefault().Montos.Count;
            } 
        }
    }
}
