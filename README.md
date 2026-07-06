# 🔐 AuthAPI

A production-ready Authentication REST API built with ASP.NET Core 10, featuring JWT authentication with refresh token rotation.

## Features

- User registration and login
- JWT Access Token (15 min expiry)
- Refresh Token rotation (7 days expiry)
- Role-based authorization (Admin/User)
- Password hashing with BCrypt
- Token revocation (logout)
- Unit tested with xUnit

## Tech Stack

- **Framework:** ASP.NET Core 10
- **Database:** SQL Server (Docker)
- **ORM:** Entity Framework Core 10
- **Authentication:** JWT Bearer
- **Testing:** xUnit v3
- **Documentation:** Swagger UI

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### Run the project

1. Clone the repository
```bash
   git clone https://github.com/your-username/AuthAPI.git
   cd AuthAPI
```

2. Start SQL Server
```bash
   docker-compose up -d
```

3. Configure settings
```bash
   cp AuthAPI/appsettings.example.json AuthAPI/appsettings.json
```
   Fill in your values in `appsettings.json`

4. Apply migrations
```bash
   dotnet ef database update --project AuthAPI
```

5. Run the API
```bash
   dotnet run --project AuthAPI
```

6. Open Swagger UI