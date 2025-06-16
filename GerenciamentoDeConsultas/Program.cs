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
                        gerenciador.ExecutarAgendamentoConsulta();
                        break;

                    case 2: // LISTAR TODAS AS CONSULTAS
                        gerenciador.ExibirTodasConsultasAgendadas();
                        break;

                    case 3: // CONSULTAS POR DATA
                        DateTime dataConsulta2 = LerData("Digite a data da consulta (dd/MM/yyyy): ");
                        gerenciador.ExibirConsultasPorData(dataConsulta2);
                        break;

                    case 4: // CONSULTAS POR MÉDICO
                        int medicoId = LerInteiro("Digite o ID do médico: ");
                        DateTime dataConsultaMedico = LerData(
                            "Digite a data da consulta (dd/MM/yyyy): "
                        );
                        gerenciador.ExibirConsultasPorMedico(medicoId, dataConsultaMedico);
                        break;

                    case 5: // ADICIONAR PACIENTE À FILA
                        Console.Write("Nome do paciente: ");
                        string? nome = Console.ReadLine();
                        gerenciador.AdicionarPacienteFila(nome);
                        break;

                    case 6: // CHAMAR PRÓXIMO PACIENTE
                        gerenciador.ChamarProximoPaciente();
                        break;

                    case 7: // LISTAR PACIENTES EM ORDEM ALFABÉTICA
                        gerenciador.ExibirPacientesOrdemAlfabetica();
                        break;

                    case 8: //VISUALIZAR MATRIZ DE CONSULTAS
                        gerenciador.ExibirMatrizConsultas();
                        break;

                    case 9: // CANCELAR CONSULTA
                        int idCancelar = LerInteiro("ID da consulta a cancelar: ");
                        gerenciador.CancelarConsultaAgendada(idCancelar);
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
