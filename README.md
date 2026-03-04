# Invensa API 🚀

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Invensa API** is a robust, scalable, and high-performance backend solution built with **ASP.NET Core 9.0**. It follows the **Clean Architecture** (Onion Architecture) principles to ensure maintainability, testability, and a clear separation of concerns.

This API serves as the backbone for the Invensa ecosystem, managing everything from inventory and products to real-time sales dashboards via WebSockets.

---

## 🏗 Architecture Overview

The project is structured into four main layers:

- **Invensa.Api**: The entry point of the application. Handles HTTP requests, SignalR hubs, Authentication/Authorization, and Swagger documentation.
- **Invensa.Application**: Contains the business logic, CQRS patterns (MediatR), DTOs, and mapping profiles.
- **Invensa.Infrastructure**: Implements data persistence using EF Core, repository patterns, email services, and security providers.
- **Invensa.Domain**: The core of the system. Contains enterprise entities, exceptions, and core interfaces.

---

## 🛠 Key Technologies

- **Framework**: [.NET 9.0](https://dotnet.microsoft.com/)
- **Real-Time**: [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction) for live dashboard updates.
- **Security**: JWT (JSON Web Token) with policy-based authorization.
- **API Documentation**: [Swagger/OpenAPI](https://swagger.io/) with support for API Versioning.
- **Patterns**: CQRS, Repository Pattern, Dependency Injection.
- **Database**: SQL Server with Entity Framework Core.
- **Hosting**: Support for Windows Services.

---

## 📦 Modules & Features

- **🔐 Auth & Users**: Secure authentication flow and user management.
- **📊 Dashboard**: Real-time sales and inventory tracking via Hubs.
- **🛒 Sales**: Management of transactions, orders, and sales history.
- **📦 Inventory**: Complete inventory lifecycle management.
- **🍎 Products**: Detailed catalog management including units and categories.
- **👥 Clients**: CRM modules to manage customer data.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/ReyDavidAG/Invensa_api.git
   cd Invensa_api
   ```

2. **Configure Database**:
   Update the connection string in `Invensa.Api/appsettings.json`.
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=InvensaDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Run Migrations (if applicable)**:
   ```bash
   dotnet ef database update --project Invensa.Infrastructure --startup-project Invensa.Api
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project Invensa.Api
   ```

---

## 📖 API Documentation

Once the API is running, you can explore the endpoints via Swagger UI:
- **Development**: `https://localhost:7197/index.html` (or your configured port)

The API supports versioning (e.g., `/api/v1/...`).

---

## 🤝 Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---
Developed with ❤️ by [ReyDavidAG](https://github.com/ReyDavidAG)
