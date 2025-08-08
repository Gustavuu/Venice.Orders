# Venice Orders - API de Pedidos

API / Microserviço para gerenciamento de pedidos, construído com foco em boas práticas de arquitetura de software, escalabilidade e manutenibilidade.

## Desenvolvido por
Gustavo Irentti

## 💻 Tecnologias Utilizadas

-   .NET 8 (LTS)
-   Entity Framework Core
-   Docker / Docker Compose
-   **Bancos de Dados:** SQL Server & MongoDB
-   **Cache:** Redis
-   **Mensageria:** RabbitMQ
-   **Padrões e Bibliotecas:** MediatR, xUnit, Moq, Shouldly
-   **Segurança:** JSON Web Tokens (JWT)

.
## ✅ Requisitos Atendidos

- **Endpoint POST para criação de pedido:** `POST /api/Pedidos`
- **Endpoint GET para consulta de pedido:** `GET /api/Pedidos/{id}`
- **Armazenamento híbrido:** Dados principais no SQL Server e a lista de itens no MongoDB.
- **Mensageria / fila:** Publica um evento `PedidoCriado` no RabbitMQ após a criação.
- **Cache com Redis:** A resposta do endpoint GET é cacheada por 2 minutos.
- **Testes unitários:** Dois testes foram criados com base nos fluxos dos endpoints.
- **Boas práticas e Padrões:** Clean Architecture, DDD, SOLID, Injeção de Dependência e CQRS.
- **Autenticação:** Os endpoints de `Pedidos` são protegidos via token JWT.
- **Docker:** Um arquivo `docker-compose.yml` orquestra toda a aplicação e seus serviços.

.
## 🏛️ Decisões de Arquitetura e Design

### Clean Architecture

A solução foi dividida seguindo os princípios da Clean Architecture:

-   **API:** A camada de apresentação, responsável por expor os casos de uso como endpoints REST e Swagger. Apenas delega o trabalho para a camada de `Application`.
-   **Application:** Orquestra as entidades de domínio para executar os casos de uso. Define as interfaces (contratos) para a camada de infraestrutura, mas não conhece suas implementações.
-   **Domain:** Contém as entidades de negócio e as regras mais puras do sistema. Não possui dependências externas.
-   **Infrastructure:** É aqui que implementa o código que lida com tecnologias externas, como Entity Framework, MongoDB, Redis e RabbitMQ.
-   **Tests:** Responsável pelos testes unitários com xUnit.

### CQRS (Command Query Responsibility Segregation)

Este padrão foi escolhido por se alinhar aos requisitos. Separa as operações de escrita (Commands) das de leitura (Queries):

-   **O fluxo de escrita (`CriarPedidoCommand`)** é otimizado para transações, persistência híbrida e publicação de eventos em fila.
-   **O fluxo de leitura (`ObterPedidoPorIdQuery`)** é otimizado para performance, utilizando cache (Redis) para evitar acessos desnecessários ao banco de dados relacional.

O framework **MediatR** foi utilizado como uma implementação do padrão Mediator para despachar os `Commands` e `Queries` para seus respectivos `Handlers`.

### Persistência Híbrida

-   **SQL Server - Relacional:** Armazena os dados principais e transacionais do pedido (cabeçalho), que possuem uma estrutura bem definida.
-   **MongoDB - Não relacional:** Armazena a lista de itens do pedido em um banco de dados orientado a documentos.

.
## 🚀 Como Executar o Projeto

**Pré-requisitos:**
* **Git** instalado.
* **Docker Desktop** instalado e em execução.

**Passos:**
1.  **Clone o repositório:** Abra um terminal ou prompt de comando no seu diretório local e execute o seguinte comando para clonar o projeto para sua máquina:
    ```bash
    git clone https://github.com/Gustavuu/Venice.Orders.git
    ```
    *Como alternativa, utilize a função de clone da sua IDE preferida ou do GitHub Desktop.*

2.  Abra um terminal na pasta raiz do projeto (onde o `docker-compose.yml` está localizado).
3.  **Execute o Docker Compose:** Este comando irá construir a imagem da API e subir todos os serviços necessários.
    ```bash
    docker compose up --build
    ```
4.  Aguarde todos os serviços subirem. Isso pode levar alguns minutos na primeira vez.
5.  Quando os logs se estabilizarem, a aplicação estará pronta para uso.

**URLs Úteis:**
-   **Swagger UI (para testar a API):** `http://localhost:8080/swagger`
-   **RabbitMQ Management UI (para visualizar a fila):** `http://localhost:15672` (**login:** `guest` / `guest`)

.
## 🔑 Como Usar a API

A API é protegida por JWT. Para usar os endpoints de `Pedidos`, siga os passos:

1.  Acesse o endpoint `POST /api/auth/login`.
2.  Envie **exatamente** o request json abaixo para obter um token:
    ```json
    {
      "username": "test_user",
      "password": "password123"
    }
    ```
3.  Copie o token JWT retornado na resposta.
4.  No topo da página do Swagger, clique no botão **"Authorize"**.
5.  Na janela que abrir, digite **`Bearer `** (a palavra Bearer seguida de um espaço) e cole o seu token.
6.  Agora você está autenticado e pode testar os endpoints `GET` e `POST` de `/api/Pedidos`.
\
**Exemplo de request json para `POST /api/Pedidos`:**
```json
{
  "clienteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "itens": [
    {
      "produtoId": "c2b5c1b8-0b5c-4b1a-9b1a-1a2b3c4d5e6f",
      "descricaoProduto": "Teclado Mecânico",
      "quantidade": 1,
      "precoUnitario": 350.50
    },
    {
      "produtoId": "f6e5d4c3-2b1a-4b9c-8c7d-6f5e4d3c2b1a",
      "descricaoProduto": "Mouse Gamer",
      "quantidade": 2,
      "precoUnitario": 120.00
    }
  ]
}
```
**Exemplo de request json para `GET /api/Pedidos`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

.
## 🧪 Testes Unitários

**Para executar os testes, você tem duas opções:**

#### Via Visual Studio:
1.  Abra a solução do projeto no Visual Studio.
2.  Abra o "Gerenciador de Testes" no menu `Testar > Gerenciador de Testes` (Test > Test Explorer).
3.  Clique no botão "Executar Todos os Testes na Exibição".

#### Via Linha de Comando:
1.  Abra um terminal na pasta raiz da solução (onde o arquivo `.sln` está localizado).
2.  Execute o seguinte comando:
    ```bash
    dotnet test
    ```

\
****Nota:** O HTTPS foi desabilitado na configuração da API para simplificar a execução em ambiente Docker, que não possui os certificados de desenvolvimento locais.*

****Nota2:** Para aumentar a coesão e o baixo acoplamento entre as funcionalidades, a solução adota uma abordagem de "Slices Verticais" (Vertical Slices). Por isso, DTOs de entrada e classes auxiliares, que são específicas de um único caso de uso (**CriarPedidoCommand** e **PedidoWriteRepository**), são mantidas como privadas dentro de seus respectivos arquivos. Já os DTOs de resposta de queries, que representam um contrato de dados público, são mantidos em arquivos separados.*