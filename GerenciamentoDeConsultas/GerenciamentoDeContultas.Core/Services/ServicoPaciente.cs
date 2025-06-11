using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services
{
      public class ServicoPaciente : IServicoPaciente
    {
        private readonly Queue<Paciente> _filaPacientes;
        private readonly List<Paciente> _listaPacientes;
        private readonly Paciente[] _vetorPacientes;

        public ServicoPaciente()
        {
            _filaPacientes = new Queue<Paciente>();
            _listaPacientes = new List<Paciente>();
            _vetorPacientes = new Paciente[100]; // Capacidade inicial
        }

        public void AdicionarPacienteNaFila(Paciente paciente)
        {
            // Implementação aqui
        }

        public Paciente ObterProximoPaciente()
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }

        public List<Paciente> ListarPacientesOrdemAlfabetica()
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }

        public Paciente[] ObterVetorPacientes()
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }
    }
}