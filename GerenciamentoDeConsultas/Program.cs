using System.Collections.Concurrent;
using System.Globalization;
using GerenciamentoDeConsultas.GerenciamentoDeConsultas.Core.Services;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static List<Consulta> listaConsultas = new List<Consulta>();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("\t--- SISTEMA DE GERENCIAMENTO DE CONSULTAS MÉDICAS (SGCM) ---");
        Console.WriteLine("\nIniciando o sistema...\n");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IServicoPaciente, ServicoPaciente>()
            .AddSingleton<IServicoConsulta, ServicoConsulta>()
            .AddSingleton<GerenciadorConsultas>()
            .BuildServiceProvider();

        var gerenciador = serviceProvider.GetRequiredService<GerenciadorConsultas>();

        bool executando = true;
        while (executando)
        {
            Console.WriteLine("Digite o número da operação que deseja realizar:");
            Console.WriteLine("==============================================");
            Console.WriteLine("1 - Agendar nova consulta");
            Console.WriteLine("2 - Listar todas as consultas agendadas");
            Console.WriteLine("3 - Consultar agendamentos por data");
            Console.WriteLine("4 - Consultar agendamentos por médico");
            Console.WriteLine("5 - Adicionar paciente à fila de espera");
            Console.WriteLine("6 - Chamar próximo paciente da fila");
            Console.WriteLine("7 - Listar pacientes em ordem alfabética");
            Console.WriteLine("8 - Visualizar matriz de consultas");
            Console.WriteLine("9 - Cancelar consulta agendada");
            Console.WriteLine("0 - Sair do sistema");
            Console.WriteLine("==============================================");
            Console.Write("\nOpção: ");

            if (!int.TryParse(Console.ReadLine(), out int opcao))
            {
                Console.WriteLine("\nOpção inválida! Por favor, digite um número.");
                continue;
            }

            Console.WriteLine();

            try
            {
                switch (opcao)
                {
                    case 1: // AGENDAR NOVA CONSULTA
                        Console.WriteLine("--- AGENDE A CONSULTA ---\n");
                        Console.WriteLine("Digite o nome do paciente e os detalhes da consulta:");

                        try
                        {
                            Console.Write("Nome completo do paciente: ");
                            string? nomePaciente = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(nomePaciente))
                            {
                                Console.WriteLine("Nome do paciente não pode ser vazio!");
                                break;
                            }

                            var paciente = new Paciente
                            {
                                Id = 1,
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
                                break;
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
                                        Console.WriteLine(
                                            "Formato inválido! Digite no formato HH:mm"
                                        );
                                        Console.Write("Hora da consulta (HH:mm): ");
                                    }

                                    var consultasNoMesmoHorario = gerenciador
                                        .ListarConsultas()
                                        .Where(c =>
                                            c.Medico != null
                                            && c.DataConsulta.Date == dataConsulta1.Date
                                        )
                                        .Any(c =>
                                            c.Medico.Id == medicoSelecionado.Id
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
                                    bool sucesso = gerenciador.AgendarNovaConsulta(
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
                                catch (ArgumentException ex)
                                    when (ex.Message.Contains("datas passadas"))
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
                        break;

                    case 2: // LISTAR TODAS AS CONSULTAS
                        Console.WriteLine("--- TODAS AS CONSULTAS AGENDADAS ---");
                        gerenciador.ListarConsultas();
                        break;

                    case 3: // CONSULTAS POR DATA
                        Console.WriteLine("--- CONSULTAS POR DATA ---");
                        DateTime dataConsulta2 = LerData(
                            "Digite a data da consulta (dd/MM/yyyy): "
                        );

                        gerenciador.ObterConsultasPorData(dataConsulta2);
                        break;

                    case 4: // CONSULTAS POR MÉDICO
                        Console.WriteLine("--- CONSULTAS POR MÉDICO ---");
                        int medicoId = LerInteiro("Digite o ID do médico: ");
                        DateTime dataConsultaMedico = LerData(
                            "Digite a data da consulta (dd/MM/yyyy): "
                        );
                        var consultasPorMedico = gerenciador.ObterConsultasPorMedico(
                            medicoId,
                            dataConsultaMedico
                        );
                        if (consultasPorMedico == null || consultasPorMedico.Count == 0)
                        {
                            Console.WriteLine(
                                "Nenhuma consulta encontrada para este médico nesta data."
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Consultas do médico {medicoId} em {dataConsultaMedico:dd/MM/yyyy}:"
                            );
                            foreach (var consulta in consultasPorMedico)
                            {
                                Console.WriteLine(
                                    $"- Paciente: {consulta.Paciente?.Nome ?? "Desconhecido"} | Horário: {consulta.HoraConsulta.ToString(@"hh\:mm")}"
                                );
                            }
                        }
                        break;

                    case 5: // ADICIONAR PACIENTE À FILA
                        Console.WriteLine("--- ADICIONAR PACIENTE À FILA ---");
                        Console.Write("Nome do paciente: ");
                        string? nome = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nome))
                        {
                            Console.WriteLine("Nome do paciente não pode ser vazio!");
                            break;
                        }
                        var pacienteFila = new Paciente
                        {
                            Nome = nome,
                            Email = "temp@email.com",
                            Telefone = "000000000",
                        };
                        gerenciador.AdicionarPacienteNaFila(pacienteFila);
                        Console.WriteLine(
                            $"Paciente '{nome}' adicionado à fila de espera com sucesso!"
                        );
                        break;

                    case 6: // CHAMAR PRÓXIMO PACIENTE
                        Console.WriteLine("--- CHAMAR PRÓXIMO PACIENTE ---");
                        var proximoPaciente = gerenciador.ObterProximoPaciente();
                        if (proximoPaciente == null)
                        {
                            Console.WriteLine("Nenhum paciente na fila de espera.");
                        }
                        else
                        {
                            Console.WriteLine($"Próximo paciente chamado: {proximoPaciente.Nome}");
                        }
                        break;

                    case 7: // LISTAR PACIENTES EM ORDEM ALFABÉTICA
                        Console.WriteLine("--- PACIENTES (ORDEM ALFABÉTICA) ---");
                        var pacientesOrdenados = gerenciador.ListarPacientesOrdemAlfabetica();
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
                        break;

                    case 8: //VISUALIZAR MATRIZ DE CONSULTAS
                        Console.WriteLine("--- MATRIZ DE CONSULTAS ---");
                        var matriz = gerenciador.ObterMatrizConsultas();
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
                        break;

                    case 9: // CANCELAR CONSULTA
                        Console.WriteLine("--- CANCELAR CONSULTA ---");
                        int idCancelar = LerInteiro("ID da consulta a cancelar: ");
                        bool cancelada = gerenciador.CancelarConsulta(idCancelar);
                        if (cancelada)
                        {
                            Console.WriteLine($"Consulta {idCancelar} cancelada com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Consulta {idCancelar} não encontrada ou já foi cancelada."
                            );
                        }
                        break;

                    case 0: // ENCERRAR SISTEMA
                        executando = false;
                        Console.WriteLine("Encerrando o sistema...");
                        break;

                    default:
                        Console.WriteLine(
                            "Opção inválida! Por favor, digite um número entre 0 e 9."
                        );
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nOcorreu um erro: {ex.Message}");
                Console.WriteLine("Por favor, tente novamente.");
            }

            if (opcao != 0)
            {
                Console.WriteLine("\nPressione qualquer tecla para retornar ao menu principal...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("\nSistema encerrado com sucesso!");
    }

    private static TimeSpan LerTimeSpan(string prompt)
    {
        TimeSpan hora;
        while (true)
        {
            Console.Write(prompt);
            if (
                TimeSpan.TryParseExact(
                    Console.ReadLine(),
                    "hh\\:mm",
                    CultureInfo.InvariantCulture,
                    TimeSpanStyles.None,
                    out hora
                )
            )
                return hora;
            Console.WriteLine("Formato inválido! Use HH:mm");
        }
    }

    private static DateTime LerData(string prompt)
    {
        DateTime data;
        while (true)
        {
            Console.Write(prompt);
            string? entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada))
            {
                Console.WriteLine("Data não pode ser vazia!");
                continue;
            }
            if (
                DateTime.TryParseExact(
                    entrada,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out data
                )
            )
                return data;
            Console.WriteLine("Data inválida! Formato esperado: dd/MM/yyyy");
        }
    }

    private static void MostrarErro(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nERRO: {mensagem}");
        Console.ResetColor();
    }

    private static int LerInteiro(string prompt)
    {
        int valor;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out valor))
                return valor;
            Console.WriteLine("Valor inválido! Digite um número inteiro.");
        }
    }
}
