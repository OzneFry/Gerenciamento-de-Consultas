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

            // Ajuste para garantir que a hora esteja no formato correto
            // Não é necessário checar null para TimeSpan, pois não é nullable
            // Garantir que a hora esteja no formato HH:mm (por exemplo, 14:00)
            TimeSpan hora;
            string horaString = consulta.HoraConsulta.ToString();
            if (!TimeSpan.TryParse(horaString, out hora))
            {
                throw new ArgumentException(
                    "Hora da consulta está em formato inválido. Utilize o formato HH:mm."
                );
            }
            consulta.HoraConsulta = hora;

            // Adicionar consulta à lista de consultas
            _listaConsultas.Add(consulta);

            try
            {
                // Salvar no XML
                XmlStorageHelper.SalvarLista(_listaConsultas, ArquivoConsultas);
            }
            catch (Exception ex)
            {
                // Tratar erro ao salvar o arquivo XML
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

            // Se não houver consultas, apenas retorne uma lista vazia
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
                // Adicionar apenas as consultas válidas (com dados completos)
                consultasValidas.Add(consulta);
            }

            // Exibir mensagem para consultas inválidas
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
            // Filtra as consultas pela data
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
            // Filtra as consultas pelo médico e pela data
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
            // Exemplo simples: preenche a matriz com nomes dos pacientes por horário e dia
            // 0 = 00:00, 1 = 01:00, ..., 23 = 23:00 | 0 = segunda, ..., 6 = domingo
            var matriz = new string[24, 7];
            var consultas = this.ListarConsultas();
            foreach (var consulta in consultas)
            {
                int hora = consulta.HoraConsulta.Hours;
                int dia = (int)consulta.DataConsulta.DayOfWeek - 1;
                if (dia < 0)
                    dia = 6; // Ajusta para segunda=0, domingo=6
                matriz[hora, dia] = consulta.Paciente?.Nome ?? "(Sem nome)";
            }
            return matriz;
        }
    }
}
