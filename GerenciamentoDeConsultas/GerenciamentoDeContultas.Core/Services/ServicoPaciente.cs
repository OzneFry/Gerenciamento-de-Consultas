using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services
{
    public class ServicoPaciente : IServicoPaciente
    {
        private readonly Queue<Paciente> _filaPacientes;
        private const string ArquivoConsultas = "../../../Data/consultas.xml";

        public ServicoPaciente()
        {
            _filaPacientes = new Queue<Paciente>();
        }

        public void AdicionarPacienteNaFila(Paciente paciente)
        {
            // Adiciona paciente na fila, mas não salva em XML (apenas consultas salvarão pacientes)
            _filaPacientes.Enqueue(paciente);
        }

        public Paciente ObterProximoPaciente()
        {
            if (_filaPacientes.Count > 0)
                return _filaPacientes.Dequeue();
            return null;
        }

        public List<Paciente> ListarPacientesOrdemAlfabetica()
        {
            // Lê todos os pacientes das consultas salvas
            var consultas = XmlStorageHelper.CarregarLista<Consulta>(ArquivoConsultas);
            return consultas
                .Where(c => c.Paciente != null)
                .Select(c => c.Paciente)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .OrderBy(p => p.Nome)
                .ToList();
        }

        public Paciente[] ObterVetorPacientes()
        {
            var consultas = XmlStorageHelper.CarregarLista<Consulta>(ArquivoConsultas);
            return consultas
                .Where(c => c.Paciente != null)
                .Select(c => c.Paciente)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToArray();
        }
    }
}
