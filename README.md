# Gerenciamento-de-Consultas

## Apresentação do Projeto

Este projeto foi desenvolvido individualmente com o objetivo de criar um sistema para gerenciamento de consultas médicas, utilizando C# e persistência de dados em XML. O sistema permite agendar, listar, consultar, cancelar consultas, gerenciar fila de espera e visualizar uma matriz semanal de agendamentos.

---

## Detalhamento do Desenvolvimento

Todo o desenvolvimento, desde a concepção até a implementação final, foi realizado por mim. Abaixo, descrevo as principais etapas e decisões técnicas do projeto:

### Estrutura e Arquitetura

- **Organização do Projeto:** Estruturei o projeto em camadas, separando modelos, serviços, interfaces e a aplicação principal. Isso facilita a manutenção e futuras expansões.
- **Persistência de Dados:** Optei por utilizar arquivos XML para armazenar as informações de consultas, garantindo fácil leitura e portabilidade sem necessidade de banco de dados externo.
- **Modelos:** Criei classes para representar Paciente, Médico e Consulta, com propriedades essenciais para o funcionamento do sistema.

### Funcionalidades Implementadas

- **Agendamento de Consultas:**  
  Implementei validações para evitar conflitos de horários, garantir integridade dos dados e padronizar nomes. O agendamento persiste os dados imediatamente no XML.
- **Listagem e Consulta:**  
  Permite listar todas as consultas, filtrar por data ou por médico, sempre exibindo as informações de forma organizada.
- **Fila de Espera:**  
  Desenvolvi uma fila de pacientes para situações em que não há horários disponíveis, permitindo chamada sequencial.
- **Cancelamento de Consultas:**  
  O sistema permite cancelar consultas, removendo-as do XML e atualizando a visualização.
- **Matriz de Consultas:**  
  Implementei uma matriz semanal que mostra, por horário e dia da semana, quais pacientes estão agendados.
- **Listagem Alfabética de Pacientes:**  
  Permite visualizar todos os pacientes cadastrados em ordem alfabética.

### Decisões Técnicas

- **Validações:**  
  Todas as entradas do usuário são validadas para evitar dados inconsistentes ou operações inválidas.
- **Padronização de Nomes:**  
  Os nomes de pacientes e médicos são padronizados para facilitar buscas e exibições.
- **Separação de Responsabilidades:**  
  Cada serviço (consultas, pacientes) possui sua própria classe, seguindo boas práticas de SOLID.

---

## Possíveis Melhorias Futuras

Caso o projeto seja evoluído, sugiro as seguintes melhorias:

- **Cadastro e autenticação de usuários:** Permitir login individual para médicos e pacientes.
- **Persistência em banco de dados relacional:** Migrar do XML para um banco como SQL Server ou SQLite.
- **Interface gráfica (GUI):** Desenvolver uma interface visual (Windows Forms, WPF ou Web).
- **Notificações automáticas:** Enviar lembretes de consultas por e-mail ou SMS.
- **Relatórios e estatísticas:** Gerar relatórios de atendimentos e horários mais procurados.
- **Testes automatizados:** Ampliar a cobertura de testes unitários e de integração.
- **Internacionalização:** Suporte a múltiplos idiomas.

---

## Considerações Finais

O projeto foi desenvolvido do zero por mim, com foco em modularidade, clareza e boas práticas de programação. Estou aberto a sugestões e dúvidas sobre o funcionamento ou possíveis melhorias.