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
            _matrizConsultas = new string[24, 7];
        }

        public bool AgendarConsulta(Consulta consulta)
        {
            if (consulta == null)
                throw new ArgumentNullException(nameof(consulta), "Consulta não pode ser nula");

            if (consulta.Paciente == null || consulta.Medico == null)
                throw new ArgumentException("Paciente e médico são obrigatórios");

            if (consulta.DataConsulta < DateTime.Today)
                throw new ArgumentException("Não é possível agendar consultas para datas passadas");

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

            consulta.Id = _listaConsultas.Count > 0 ? _listaConsultas.Max(c => c.Id) + 1 : 1;

            TimeSpan hora;
            string horaString = consulta.HoraConsulta.ToString();
            if (!TimeSpan.TryParse(horaString, out hora))
            {
                throw new ArgumentException(
                    "Hora da consulta está em formato inválido. Utilize o formato HH:mm."
                );
            }
            consulta.HoraConsulta = hora;

            _listaConsultas.Add(consulta);

            try
            {
                XmlStorageHelper.SalvarLista(_listaConsultas, ArquivoConsultas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar consultas: {ex.Message}");
                return false;
            }

            return true;
        }

        public List<Consulta> ListarConsultas()
        {
            if (_listaConsultas == null)
            {
                throw new ArgumentNullException(
                    nameof(_listaConsultas),
                    "A lista de consultas não foi inicializada."
                );
            }

            if (_listaConsultas.Count == 0)
            {
                return new List<Consulta>();
            }

            List<Consulta> consultasValidas = new List<Consulta>();

            // Ordena por data e hora
            var consultasOrdenadas = _listaConsultas
                .Where(c => c.Paciente != null && c.Medico != null)
                .OrderBy(c => c.DataConsulta)
                .ThenBy(c => c.HoraConsulta)
                .ToList();

            foreach (var consulta in consultasOrdenadas)
            {
                consultasValidas.Add(consulta);
            }

            var consultasInvalidas = _listaConsultas.Where(c =>
                c.Paciente == null || c.Medico == null
            );
            foreach (var consulta in consultasInvalidas)
            {
                Console.WriteLine("Consulta com dados inválidos ignorada.");
            }

            return consultasValidas;
        }

        public List<Consulta> ObterConsultasPorData(DateTime data)
        {
            var consultasDoDia = _listaConsultas
                .Where(c => c.DataConsulta.Date == data.Date)
                .ToList();

            if (consultasDoDia.Count == 0)
            {
                Console.WriteLine("\nNão há consultas agendadas para esta data.");
            }
            else
            {
                Console.WriteLine("\nConsultas agendadas para a data informada:");

                foreach (var consulta in consultasDoDia)
                {
                    Console.WriteLine($"Paciente: {consulta.Paciente.Nome}");
                    Console.WriteLine($"Médico: {consulta.Medico.Nome}");
                    Console.WriteLine($"Data: {consulta.DataConsulta:dd/MM/yyyy}");
                    Console.WriteLine($"Hora: {consulta.HoraConsulta:hh\\:mm}");
                    Console.WriteLine("---------------------------------");
                }
            }

            return consultasDoDia;
        }

        public bool CancelarConsulta(int consultaId)
        {
            var consulta = _listaConsultas.FirstOrDefault(c => c.Id == consultaId);
            if (consulta == null)
                return false;
            _listaConsultas.Remove(consulta);
            try
            {
                XmlStorageHelper.SalvarLista(_listaConsultas, ArquivoConsultas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar consultas após cancelamento: {ex.Message}");
                return false;
            }
            return true;
        }

        public List<Consulta> ObterConsultasPorMedico(int medicoId, DateTime data)
        {
            var consultas = _listaConsultas
                .Where(c =>
                    c.Medico != null && c.Medico.Id == medicoId && c.DataConsulta.Date == data.Date
                )
                .OrderBy(c => c.HoraConsulta)
                .ToList();
            return consultas;
        }

        public string[,] ObterMatrizConsultas()
        {
            var matriz = new string[24, 7];
            var consultas = this.ListarConsultas();
            foreach (var consulta in consultas)
            {
                int hora = consulta.HoraConsulta.Hours;
                int dia = (int)consulta.DataConsulta.DayOfWeek - 1;
                if (dia < 0)
                    dia = 6;
                matriz[hora, dia] = consulta.Paciente?.Nome ?? "(Sem nome)";
            }
            return matriz;
        }
    }
}
