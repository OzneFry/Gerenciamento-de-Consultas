using System;
using System.Collections.Generic;
using System.Globalization;
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

        public void ExecutarAgendamentoConsulta()
        {
            Console.WriteLine("--- AGENDE A CONSULTA ---\n");
            Console.WriteLine("Digite o nome do paciente e os detalhes da consulta:");

            try
            {
                Console.Write("Nome completo do paciente: ");
                string? nomePaciente = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nomePaciente))
                {
                    Console.WriteLine("Nome do paciente não pode ser vazio!");
                    return;
                }

                var paciente = new Paciente
                {
                    Id = 1, // Id fictício, pois não há persistência de pacientes
                    Nome = nomePaciente,
                    Email = "temp@email.com",
                    Telefone = "000000000",
                };
                Console.WriteLine("\nSelecione um médico para a consulta:");
                var medicos = new List<Medico>
                {
                    new Medico
                    {
                        Id = 1,
                        Nome = "Dr. Carlos Silva",
                        Especialidade = "Cardiologia",
                        CRM = "12345-SP",
                    },
                    new Medico
                    {
                        Id = 2,
                        Nome = "Dra. Ana Souza",
                        Especialidade = "Dermatologia",
                        CRM = "54321-SP",
                    },
                    new Medico
                    {
                        Id = 3,
                        Nome = "Dr. Marcos Oliveira",
                        Especialidade = "Ortopedia",
                        CRM = "98765-SP",
                    },
                };

                Console.WriteLine("\nMédicos disponíveis:");
                foreach (var med in medicos)
                {
                    Console.WriteLine(
                        $"{med.Id} - {med.Nome} ({med.Especialidade}) - CRM: {med.CRM}"
                    );
                }

                int idMedico = LerInteiro("\nID do médico: ");
                var medicoSelecionado = medicos.FirstOrDefault(m => m.Id == idMedico);

                if (medicoSelecionado == null)
                {
                    Console.WriteLine("Médico não encontrado!");
                    return;
                }

                DateTime dataConsulta1;
                TimeSpan horaConsulta;
                bool agendamentoConcluido = false;
                while (!agendamentoConcluido)
                {
                    dataConsulta1 = LerData("Data da consulta (dd/MM/yyyy): ");

                    bool horarioDisponivel = false;
                    do
                    {
                        Console.Write("Hora da consulta (HH:mm): ");
                        while (!TimeSpan.TryParse(Console.ReadLine(), out horaConsulta))
                        {
                            Console.WriteLine("Formato inválido! Digite no formato HH:mm");
                            Console.Write("Hora da consulta (HH:mm): ");
                        }

                        var consultasNoMesmoHorario = ListarConsultas()
                            .Where(c =>
                                c.Medico != null && c.DataConsulta.Date == dataConsulta1.Date
                            )
                            .Any(c =>
                                c.Medico != null
                                && c.Medico.Id == medicoSelecionado.Id
                                && c.HoraConsulta == horaConsulta
                            );

                        if (consultasNoMesmoHorario)
                        {
                            Console.WriteLine(
                                "Este médico já possui uma consulta nesse horário. Escolha outro horário!"
                            );
                        }
                        else
                        {
                            horarioDisponivel = true;
                        }
                    } while (!horarioDisponivel);

                    try
                    {
                        bool sucesso = AgendarNovaConsulta(
                            paciente,
                            medicoSelecionado,
                            dataConsulta1,
                            horaConsulta
                        );
                        if (sucesso)
                        {
                            Console.WriteLine("\nConsulta agendada com sucesso!");
                            Console.WriteLine($"Paciente: {paciente.Nome}");
                            Console.WriteLine($"Médico: {medicoSelecionado.Nome}");
                            Console.WriteLine(
                                $"Data: {dataConsulta1:dd/MM/yyyy} às {horaConsulta:hh\\:mm}"
                            );
                            agendamentoConcluido = true;
                        }
                    }
                    catch (ArgumentException ex) when (ex.Message.Contains("datas passadas"))
                    {
                        Console.WriteLine(
                            "Não é possível agendar consultas para datas passadas. Escolha uma nova data!"
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nErro ao agendar consulta: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao agendar consulta: {ex.Message}");
            }
        }

        public void ExibirTodasConsultasAgendadas()
        {
            Console.WriteLine("--- TODAS AS CONSULTAS AGENDADAS ---");
            var consultas = ListarConsultas();
            if (consultas == null || consultas.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta agendada encontrada.");
            }
            else
            {
                foreach (var consulta in consultas)
                {
                    Console.WriteLine("ID: " + consulta.Id);
                    Console.WriteLine(
                        "Paciente: "
                            + (consulta.Paciente != null ? consulta.Paciente.Nome : "(sem nome)")
                    );
                    Console.WriteLine(
                        "Médico: " + (consulta.Medico != null ? consulta.Medico.Nome : "(sem nome)")
                    );
                    Console.WriteLine(
                        "Especialidade: "
                            + (
                                consulta.Medico != null
                                    ? consulta.Medico.Especialidade
                                    : "(sem especialidade)"
                            )
                    );
                    Console.WriteLine("Data: " + consulta.DataConsulta.ToString("dd/MM/yyyy"));
                    Console.WriteLine("Hora: " + consulta.HoraConsulta.ToString(@"hh\:mm"));
                    Console.WriteLine("Realizada: " + (consulta.Realizada ? "Sim" : "Não"));
                    Console.WriteLine("----------------------------------------");
                }
            }
        }

        public void ExibirConsultasPorData(DateTime dataConsulta)
        {
            Console.WriteLine("--- CONSULTAS POR DATA ---");
            var consultas = ObterConsultasPorData(dataConsulta);
            if (consultas == null || consultas.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta agendada para esta data.");
            }
            else
            {
                foreach (var consulta in consultas)
                {
                    Console.WriteLine("ID: " + consulta.Id);
                    Console.WriteLine(
                        "Paciente: "
                            + (consulta.Paciente != null ? consulta.Paciente.Nome : "(sem nome)")
                    );
                    Console.WriteLine(
                        "Médico: " + (consulta.Medico != null ? consulta.Medico.Nome : "(sem nome)")
                    );
                    Console.WriteLine(
                        "Especialidade: "
                            + (
                                consulta.Medico != null
                                    ? consulta.Medico.Especialidade
                                    : "(sem especialidade)"
                            )
                    );
                    Console.WriteLine("Data: " + consulta.DataConsulta.ToString("dd/MM/yyyy"));
                    Console.WriteLine("Hora: " + consulta.HoraConsulta.ToString(@"hh\:mm"));
                    Console.WriteLine("Realizada: " + (consulta.Realizada ? "Sim" : "Não"));
                    Console.WriteLine("----------------------------------------");
                }
            }
        }

        public void ExibirConsultasPorMedico(int medicoId, DateTime dataConsulta)
        {
            Console.WriteLine("--- CONSULTAS POR MÉDICO ---");
            var consultasPorMedico = ObterConsultasPorMedico(medicoId, dataConsulta);
            if (consultasPorMedico == null || consultasPorMedico.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta encontrada para este médico nesta data.");
            }
            else
            {
                Console.WriteLine($"Consultas do médico {medicoId} em {dataConsulta:dd/MM/yyyy}:");
                foreach (var consulta in consultasPorMedico)
                {
                    Console.WriteLine(
                        $"- Paciente: {consulta.Paciente?.Nome ?? "Desconhecido"} | Horário: {consulta.HoraConsulta:hh\\:mm}"
                    );
                }
            }
        }

        public void AdicionarPacienteFila(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome do paciente não pode ser vazio!");
                return;
            }
            var pacienteFila = new Paciente
            {
                Nome = nome,
                Email = "temp@email.com",
                Telefone = "000000000",
            };
            AdicionarPacienteNaFila(pacienteFila);
            Console.WriteLine($"Paciente '{nome}' adicionado à fila de espera com sucesso!");
        }

        public void ChamarProximoPaciente()
        {
            var proximoPaciente = ObterProximoPaciente();
            if (proximoPaciente == null)
            {
                Console.WriteLine("Nenhum paciente na fila de espera.");
            }
            else
            {
                Console.WriteLine($"Próximo paciente chamado: {proximoPaciente.Nome}");
            }
        }

        public void ExibirPacientesOrdemAlfabetica()
        {
            Console.WriteLine("--- PACIENTES (ORDEM ALFABÉTICA) ---");
            var pacientesOrdenados = ListarPacientesOrdemAlfabetica();
            if (pacientesOrdenados == null || pacientesOrdenados.Count == 0)
            {
                Console.WriteLine("Nenhum paciente cadastrado.");
            }
            else
            {
                foreach (var paciente in pacientesOrdenados)
                {
                    Console.WriteLine($"- {paciente.Nome}");
                }
            }
        }

        public void ExibirMatrizConsultas()
        {
            Console.WriteLine("--- MATRIZ DE CONSULTAS ---");
            var matriz = ObterMatrizConsultas();
            if (matriz == null)
            {
                Console.WriteLine("Matriz de consultas não disponível.");
            }
            else
            {
                string[] dias =
                {
                    "Segunda",
                    "Terça",
                    "Quarta",
                    "Quinta",
                    "Sexta",
                    "Sábado",
                    "Domingo",
                };
                int larguraColuna = 18;
                Console.Write("Horário/Dia ");
                foreach (var dia in dias)
                    Console.Write($"| {dia.PadRight(larguraColuna - 2)}");
                Console.WriteLine("|");
                Console.WriteLine(new string('-', 10 + (larguraColuna + 2) * 7));
                for (int h = 0; h < matriz.GetLength(0); h++)
                {
                    Console.Write($"{h:00}:00      ");
                    for (int d = 0; d < matriz.GetLength(1); d++)
                    {
                        string valor = matriz[h, d] ?? "-";
                        Console.Write($"| {valor.PadRight(larguraColuna - 2)}");
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine(new string('-', 10 + (larguraColuna + 2) * 7));
            }
        }

        public void CancelarConsultaAgendada(int idCancelar)
        {
            bool cancelada = CancelarConsulta(idCancelar);
            if (cancelada)
            {
                Console.WriteLine($"Consulta {idCancelar} cancelada com sucesso!");
            }
            else
            {
                Console.WriteLine($"Consulta {idCancelar} não encontrada ou já foi cancelada.");
            }
        }

        private int LerInteiro(string prompt)
        {
            int valor;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out valor))
            {
                Console.WriteLine("Valor inválido! Digite um número inteiro.");
                Console.Write(prompt);
            }
            return valor;
        }

        private DateTime LerData(string prompt)
        {
            DateTime data;
            Console.Write(prompt);
            string? entrada = Console.ReadLine();
            while (
                !DateTime.TryParseExact(
                    entrada ?? string.Empty,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out data
                )
            )
            {
                Console.WriteLine("Data inválida! Digite no formato dd/MM/yyyy.");
                Console.Write(prompt);
                entrada = Console.ReadLine();
            }
            return data;
        }
    }
}
