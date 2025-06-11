using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("\t--- SISTEMA DE GERENCIAMENTO DE CONSULTAS MÉDICAS (SGCM) ---");
        Console.WriteLine("\nIniciando o sistema...\n");

        // Configuração da injeção de dependência
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
                    case 1: // Agendar nova consulta
                        Console.WriteLine("Digite o nome do paciente e os detalhes da consulta:");

                        try
                        {
                            // Cadastro rápido do paciente (em um sistema real, teríamos um cadastro completo)
                            Console.Write("Nome do paciente: ");
                            string nomePaciente = Console.ReadLine();

                            var paciente = new Paciente
                            {
                                Id = 1,
                                Nome = nomePaciente,
                                Email = "temp@email.com",
                                Telefone = "000000000",
                            };
                            // Simulação de busca de médicos disponíveis
                            Console.WriteLine("\nSelecione um médico para a consulta:");
                            var medicos = new List<Medico> // Lista de médicos fictícia
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
                                    Nome = "Dr. Marcos Lima",
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

                            DateTime dataConsulta = LerData("Data da consulta (dd/MM/yyyy): ");

                            Console.Write("Hora da consulta (HH:mm): ");
                            TimeSpan horaConsulta;
                            while (!TimeSpan.TryParse(Console.ReadLine(), out horaConsulta))
                            {
                                Console.WriteLine("Formato inválido! Digite no formato HH:mm");
                                Console.Write("Hora da consulta (HH:mm): ");
                            }

                            bool sucesso = gerenciador.AgendarNovaConsulta(
                                paciente,
                                medicoSelecionado,
                                dataConsulta,
                                horaConsulta
                            );

                            if (sucesso)
                            {
                                Console.WriteLine("\nConsulta agendada com sucesso!");
                                Console.WriteLine($"Paciente: {paciente.Nome}");
                                Console.WriteLine($"Médico: {medicoSelecionado.Nome}");
                                Console.WriteLine(
                                    $"Data: {dataConsulta:dd/MM/yyyy} às {horaConsulta:hh\\:mm}"
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nErro ao agendar consulta: {ex.Message}");
                        }
                        break;

                    case 2: // Listar todas as consultas
                        Console.WriteLine("--- TODAS AS CONSULTAS AGENDADAS ---");
                        // Implementar listagem
                        break;

                    case 3: // Consultar por data
                        Console.WriteLine("--- CONSULTAS POR DATA ---");
                        Console.Write("Digite a data (dd/MM/yyyy): ");
                        // Implementar busca por data
                        break;

                    case 4: // Consultar por médico
                        Console.WriteLine("--- CONSULTAS POR MÉDICO ---");
                        // Implementar busca por médico
                        break;

                    case 5: // Adicionar paciente à fila
                        Console.WriteLine("--- ADICIONAR PACIENTE À FILA ---");
                        Console.Write("Nome do paciente: ");
                        string nome = Console.ReadLine();
                        // Implementar adição à fila
                        break;

                    case 6: // Chamar próximo paciente
                        Console.WriteLine("--- CHAMAR PRÓXIMO PACIENTE ---");
                        // Implementar chamada do próximo
                        break;

                    case 7: // Listar pacientes em ordem alfabética
                        Console.WriteLine("--- PACIENTES (ORDEM ALFABÉTICA) ---");
                        // Implementar listagem ordenada
                        break;

                    case 8: // Visualizar matriz de consultas
                        Console.WriteLine("--- MATRIZ DE CONSULTAS ---");
                        // Implementar exibição da matriz
                        break;

                    case 9: // Cancelar consulta
                        Console.WriteLine("--- CANCELAR CONSULTA ---");
                        Console.Write("ID da consulta a cancelar: ");
                        // Implementar cancelamento
                        break;

                    case 0: // Sair do sistema
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
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                if (!Console.IsInputRedirected)
                {
                    Console.Clear();
                }
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
            if (TimeSpan.TryParse(Console.ReadLine(), out hora))
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
            if (DateTime.TryParse(Console.ReadLine(), out data))
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
