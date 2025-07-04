# PaymentApp

**Project Status:** In development

**Description**
PaymentApp is a lightweight ASP.NET Core platform for processing customer payments, leveraging Entity Framework Core, RabbitMQ, MongoDB, Autofac, and Docker for reliability and maintainability.

---

## Technologies Used

* C# / .NET 9 (ASP.NET Core)
* Entity Framework Core
* RabbitMQ
* MongoDB
* Autofac
* Docker & Docker Compose
* xUnit & Moq (unit testing)
* GitHub Actions (CI/CD)

---

## Getting Started

1. **Configuration**
   – Edit `appsettings.json` in `src/PaymentAPI`:
   • `ConnectionStrings:DefaultConnection` → your SQL Server
   • `RabbitMQ:HostName` → e.g. `rabbitmq`
   • `MongoDbSettings` → your MongoDB URI & database

2. **Run with Docker**

   ```bash
   docker-compose up --build
   ```

   * API: [http://localhost:6000](http://localhost:6000)
   * RabbitMQ UI: [http://localhost:15672](http://localhost:15672)

3. **Run Tests**

   ```bash
   dotnet test tests/PaymentApp.Tests
   ```

---

## Project Structure

```plain
PaymentApp/
├── src/
│   ├── PaymentApp.Api/
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   ├── Dockerfile
│   │   └── appsettings.json
│   ├── PaymentApp.Application/
│   │   ├── DTOs/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── PaymentApp.Domain/
│   ├── PaymentApp.Infrastructure/
│   │   ├── DataModule.cs
│   │   ├── ServiceModule.cs
│   │   ├── Repositories/
│   │   ├── EventPublisher/
│   │   ├── EventConsumer/
│   │   └── MongoDb/
│   └── PaymentApp.SharedKernel/
├── test/
│   └── PaymentApp.Test/
├── docker-compose.yml
├── .github/workflows/ci.yml
├── README.md
└── .gitignore
```

---

## Configuration Details

All runtime settings are in `src/PaymentAPI/appsettings.json`:

* **ConnectionStrings\:DefaultConnection** – SQL Server
* **RabbitMQ\:HostName** – message broker host
* **MongoDbSettings** – audit log database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PaymentLogs"
  }
}
```
