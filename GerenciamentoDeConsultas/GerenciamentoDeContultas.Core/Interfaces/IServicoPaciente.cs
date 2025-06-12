using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces
{
    public interface IServicoPaciente
    {
         void AdicionarPacienteNaFila(Paciente paciente);
        Paciente ObterProximoPaciente();
        List<Paciente> ListarPacientesOrdemAlfabetica();
        Paciente[] ObterVetorPacientes();
    }
}