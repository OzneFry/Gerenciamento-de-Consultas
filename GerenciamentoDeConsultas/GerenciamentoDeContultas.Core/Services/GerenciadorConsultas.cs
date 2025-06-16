using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;

namespace GerenciamentoDeConsultas.GerenciamentoDeConsultas.Core.Services
{
    public class GerenciadorConsultas
    {
        private readonly IServicoPaciente _servicoPaciente;
        private readonly IServicoConsulta _servicoConsulta;

        public GerenciadorConsultas(
            IServicoPaciente servicoPaciente,
            IServicoConsulta servicoConsulta
        )
        {
            _servicoPaciente = servicoPaciente;
            _servicoConsulta = servicoConsulta;
        }

        public bool AgendarNovaConsulta(
            Paciente paciente,
            Medico medico,
            DateTime data,
            TimeSpan hora
        )
        {
            var consulta = new Consulta
            {
                Paciente = paciente,
                Medico = medico,
                DataConsulta = data.Date,
                HoraConsulta = hora,
                Realizada = false,
            };

            return _servicoConsulta.AgendarConsulta(consulta);
        }

        public List<Consulta> ListarConsultas()
        {
            return _servicoConsulta.ListarConsultas();
        }

        public List<Consulta> ObterConsultasPorData(DateTime data)
        {
            return _servicoConsulta.ObterConsultasPorData(data);
        }

        public List<Consulta> ObterConsultasPorMedico(int medicoId, DateTime data)
        {
            return _servicoConsulta.ObterConsultasPorMedico(medicoId, data);
        }

        public void AdicionarPacienteNaFila(Paciente paciente)
        {
            _servicoPaciente.AdicionarPacienteNaFila(paciente);
        }

        public Paciente ObterProximoPaciente()
        {
            return _servicoPaciente.ObterProximoPaciente();
        }

        public List<Paciente> ListarPacientesOrdemAlfabetica()
        {
            return _servicoPaciente.ListarPacientesOrdemAlfabetica();
        }

        public string[,] ObterMatrizConsultas()
        {
            return _servicoConsulta.ObterMatrizConsultas();
        }

        public bool CancelarConsulta(int consultaId)
        {
            return _servicoConsulta.CancelarConsulta(consultaId);
        }
    }
}
