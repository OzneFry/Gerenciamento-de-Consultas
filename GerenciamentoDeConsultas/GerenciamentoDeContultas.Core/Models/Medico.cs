using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models
{
    public class Medico
    {
         public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string CRM { get; set; }
    }
}