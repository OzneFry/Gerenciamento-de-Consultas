using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services
{
    public class ServicoConsulta : IServicoConsulta
    {
        private readonly List<Consulta> _listaConsultas;
        private readonly string[,] _matrizConsultas;
        private const string ArquivoConsultas = "../../../Data/consultas.xml";

        public ServicoConsulta()
        {
            _listaConsultas = XmlStorageHelper.CarregarLista<Consulta>(ArquivoConsultas);
            _matrizConsultas = new string[24, 7]; // 24 horas, 7 dias
        }

        public bool AgendarConsulta(Consulta consulta)
        {
            if (consulta == null)
                throw new ArgumentNullException(nameof(consulta), "Consulta não pode ser nula");

            if (consulta.Paciente == null || consulta.Medico == null)
                throw new ArgumentException("Paciente e médico são obrigatórios");

            if (consulta.DataConsulta < DateTime.Today)
                throw new ArgumentException("Não é possível agendar consultas para datas passadas");

            // Verificar conflito de horário
            if (
                _listaConsultas.Any(c =>
                    c.Medico.Id == consulta.Medico.Id
                    && c.DataConsulta == consulta.DataConsulta
                    && c.HoraConsulta == consulta.HoraConsulta
                )
            )
            {
                throw new InvalidOperationException(
                    "Médico já possui consulta agendada neste horário"
                );
            }

            // Atribuir ID (simulação - em produção usar banco de dados)
            consulta.Id = _listaConsultas.Count > 0 ? _listaConsultas.Max(c => c.Id) + 1 : 1;

            // Adicionar na lista
            _listaConsultas.Add(consulta);

            // Atualizar matriz de consultas
            int hora = consulta.HoraConsulta.Hours;
            int diaSemana = (int)consulta.DataConsulta.DayOfWeek;

            if (hora >= 0 && hora < 24 && diaSemana >= 0 && diaSemana < 7)
            {
                _matrizConsultas[hora, diaSemana] =
                    $"Pac: {consulta.Paciente.Nome.Substring(0, Math.Min(10, consulta.Paciente.Nome.Length))} | "
                    + $"Méd: {consulta.Medico.Nome.Substring(0, Math.Min(10, consulta.Medico.Nome.Length))}";
            }

            // Salvar no XML
            XmlStorageHelper.SalvarLista(_listaConsultas, ArquivoConsultas);

            return true;
        }

        public bool CancelarConsulta(int consultaId)
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }

        public List<Consulta> ObterConsultasPorData(DateTime data)
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }

        public List<Consulta> ObterConsultasPorMedico(int medicoId, DateTime data)
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }

        public string[,] ObterMatrizConsultas()
        {
            // Implementação aqui
            throw new NotImplementedException("Método ainda não implementado");
        }
    }
}
