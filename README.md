# Saga Pattern com .NET Core 8

Este projeto demonstra a implementação do padrão Saga utilizando Apache Kafka, FastEndpoints, Refit, MongoDB e microsserviços com .NET Core 8. O padrão Saga é uma abordagem para gerenciar transações distribuídas garantindo consistência e confiabilidade em sistemas de microsserviços.

## Tecnologias Utilizadas

- **.NET Core 8**: Framework para desenvolvimento de aplicações modernas, escaláveis e de alta performance.
- **Apache Kafka**: Plataforma de streaming distribuída utilizada para construir pipelines de dados em tempo real.
- **FastEndpoints**: Biblioteca para criação de endpoints de API de forma rápida e fácil.
- **Refit**: Biblioteca de REST client para .NET, que facilita a comunicação entre microsserviços.
- **MongoDB**: Banco de dados NoSQL orientado a documentos, utilizado para armazenamento de dados.

## Estrutura do Projeto

O projeto é composto por vários microsserviços que se comunicam entre si utilizando o Apache Kafka como sistema de mensageria. Cada microsserviço é responsável por uma parte específica do fluxo de pedidos e utiliza o padrão Saga para garantir que todas as operações sejam concluídas com sucesso ou revertidas em caso de falha.

### Microsserviços

1. **Order Service**: Responsável pela criação e gerenciamento de pedidos.
2. **Payment Service**: Gerencia o processamento de pagamentos.
3. **Inventory Service**: Cuida da gestão de estoque e reserva de produtos.

### Fluxo do Pedido

1. O `Order Service` recebe um novo pedido e inicia uma nova Saga.
2. Uma mensagem é enviada para o `Payment Service` para processar o pagamento.
3. Após a confirmação do pagamento, uma mensagem é enviada para o `Inventory Service` para reservar os produtos.
4. Cada serviço confirma a conclusão da sua etapa e a Saga é finalizada com sucesso.

## Como Executar

### Pré-requisitos

- .NET Core 8
- Apache Kafka
- MongoDB

### Passos para Execução

1. Clone o repositório:
   ```bash
   git clone https://github.com/marvinho-tn/saga-pattern-dotnet.git

2. Navegue até o diretório do projeto:

   ```bash
   cd saga-pattern-dotnet
   
3. Configure as conexões com Apache Kafka e MongoDB nos arquivos de configuração dos microsserviços.

Execute os microsserviços:

   ```bash
   dotnet run --project Order
   dotnet run --project Payment
   dotnet run --project Inventory
   dotnet run --project Worker
   dotnet run --project Orchestrator
