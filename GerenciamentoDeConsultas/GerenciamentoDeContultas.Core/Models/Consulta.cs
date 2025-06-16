using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int MedicoId { get; set; }
        public Medico Medico { get; set; }
        public DateTime DataConsulta { get; set; }
        public TimeSpan HoraConsulta { get; set; }
        public bool Realizada { get; set; }
    }
}
