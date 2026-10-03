# User Service (`bbu-user-service`)

Microsserviço de gestão de usuários construído em **C# / ASP.NET Core** seguindo os princípios de **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS (MediatR)**, **FluentValidation**, **Entity Framework Core**, **RabbitMQ** e **xUnit**.

---

## 🏗️ Estrutura do Projeto

```text
bbu-user-service/
│
├── src/
│   ├── Api/
│   │   ├── Controllers/UsersController.cs
│   │   ├── Middlewares/ExceptionMiddleware.cs
│   │   ├── Extensions/DependencyInjection.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── Application/
│   │   ├── Users/
│   │   │   ├── Commands/ (CreateUser, UpdateUser, DeleteUser)
│   │   │   └── Queries/ (GetUser, GetUserByPersonaName)
│   │   ├── DTOs/ (UserDto.cs, ApiResponse.cs)
│   │   ├── Interfaces/ (IUserRepository.cs)
│   │   └── DependencyInjection.cs
│   ├── Domain/
│   │   ├── Users/ (User.cs, UserId.cs, ValueObjects, Enums, Events, IUserRepository.cs)
│   │   └── Common/ (Entity.cs, AggregateRoot.cs, IDomainEvent.cs)
│   └── Infrastructure/
│       ├── Persistence/ (UserDbContext.cs, UserConfiguration.cs, UserRepository.cs)
│       ├── Messaging/ (RabbitMqPublisher.cs, UserCreatedConsumer.cs)
│       └── DependencyInjection.cs
├── tests/
│   ├── Domain.Tests/
│   ├── Application.Tests/
│   └── Integration.Tests/
├── Dockerfile
├── docker-compose.yml
├── .gitignore
├── README.md
└── UserService.sln
```

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker & Docker Compose](https://www.docker.com/) (opcional para execução conteinerizada)

### Execução Local (.NET CLI)

1. Restaurar pacotes e compilar a solução:
   ```bash
   dotnet build UserService.sln
   ```

2. Executar os testes de unidade e integração:
   ```bash
   dotnet test UserService.sln
   ```

3. Rodar a API Web:
   ```bash
   dotnet run --project src/Api/Api.csproj
   ```

Acesse a documentação interativa da API no navegador via Swagger:
- `http://localhost:5000/swagger` ou `https://localhost:5001/swagger`

---

## 🐳 Execução via Docker Compose

Para subir a API junto ao **PostgreSQL** e **RabbitMQ**:

```bash
docker-compose up -d --build
```

Serviços disponíveis:
- **API**: `http://localhost:5000`
- **PostgreSQL**: `localhost:5432`
- **RabbitMQ Management**: `http://localhost:15672` (Usuário: `guest`, Senha: `guest`)

---

## 📌 Endpoints da API

- `POST /api/users` - Criar novo usuário
- `GET /api/users/{id}` - Buscar usuário por Id
- `GET /api/users/by-PersonaName/{PersonaName}` - Buscar usuário por PersonaName
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Remover usuário
