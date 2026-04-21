# Minimal API com Entity Framework (DIO)

Este repositório contém o desenvolvimento de uma **Minimal API** utilizando **.NET** e **Entity Framework Core**. O projeto é baseado no desafio de código da Digital Innovation One (DIO), focado em criar serviços leves, performáticos e com acesso a banco de dados de forma simplificada.

## 🚀 Tecnologias e Ferramentas

- **Linguagem:** C#
- **Framework:** .NET 8/9+ (Minimal APIs)
- **ORM:** Entity Framework Core
- **Banco de Dados:** MySQL
- **Testes:** xUnit e pacotes de teste para integração
- **Documentação:** Swagger (OpenAPI)
- **Gerenciamento de Projeto:** Arquivo `.slnx` (formato moderno de solução do Visual Studio)

## 📁 Estrutura do Repositório

- **`/Api`**: Implementação principal contendo os modelos, contextos (DbContext), serviços e os endpoints da API definidos no `Program.cs`.
- **`/Test`**: Suite de testes automatizados para validar a lógica de negócio e os endpoints.
- **`dio-minimal-api.slnx`**: Arquivo de solução para abertura rápida no Visual Studio ou VS Code (com extensões compatíveis).

## 🛠️ Como Executar

### Pré-requisitos
- [.NET SDK](https://dotnet.microsoft.com/download) instalado.
- Ferramentas de linha de comando ou uma IDE (Visual Studio 2022 / VS Code).

### Execução local
1. Clone o seu repositório:
   ```bash
   git clone https://github.com/andrexm/dio-minimal-api.git
   ```
2. Entre na pasta:
```Bash
cd dio-minimal-api
```

3. Restaure as dependências:
```Bash
dotnet restore
```

4.Execute o projeto da API:
```Bash
dotnet run --project Api
```

#### Acessando a API
Após iniciar, a API estará disponível (geralmente em http://localhost:5131). Você pode acessar a interface do Swagger para testar os endpoints através do endereço:
http://localhost:5131/swagger.

## 🧪 Testes
Para garantir que tudo está funcionando corretamente, execute os testes unitários:
```Bash
dotnet test
```

## 📝 Aprendizados
Este projeto demonstra:

- Configuração de rotas diretamente no Program.cs (sem Controllers tradicionais).

- Injeção de dependência para Contexts do Entity Framework.

- Migrations de banco de dados e persistência.

- Estruturação de testes para APIs.

## ✒️ Autor
Desenvolvido por André Nascimento.