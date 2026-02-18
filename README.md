# Medicine Finder

A backend API for a Medicine Finder system implemented with .NET 8, EF Core and ASP.NET Identity. This README documents how to run the project, seeded credentials, migrations, tests and tradeoffs for local development.

Prerequisites
-------------
- .NET 8 SDK
- SQL Server LocalDB (installed with Visual Studio) or a SQL Server instance (can run in Docker)
- Optional: `dotnet-ef` tool (`dotnet tool install --global dotnet-ef`)

Run the app (recommended - LocalDB)
---------------------------------
1. From repository root restore and build:

```bash
dotnet restore
dotnet build
```

2. Update configuration if needed:
- `API/appsettings.json` contains `ConnectionStrings:DefaultConnection` (defaults to LocalDB) and `Jwt` settings. Change the connection string or JWT secret as required.

3. Run the API (migrations are applied automatically at startup):

```bash
cd API
dotnet run
```

4. Open Swagger (in Development): `http://localhost:5018/swagger/` (port may vary).

Run with Docker (SQL Server)
---------------------------
1. Start SQL Server container (example):

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

2. Update `API/appsettings.json` with the container connection string, e.g.:

```
Server=localhost;Database=MedicineFinderDb;User ID=sa;Password=YourStrong!Passw0rd;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

3. Build & run as above.

Seeded credentials (default)
---------------------------
The application seeds roles, one admin, one users, pharmacies, medicines and stocks at startup. Default accounts:

- Admin
  - Email: `admin@mf.local`
  - Password: `Admin123!`
  - Role: `Admin`

- User
  - Email: `user@mf.local`
  - Password: `User123!`

You can change these values in `Infrastructure/Seeders/DatabaseSeeder.cs`.

Migrations
----------
- The initial migration is included under `Infrastructure/Data/Migrations`.
- Typical migration workflow from the solution root:

```bash
dotnet ef migrations add MyMigrationName --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

- The application calls `Database.MigrateAsync()` on startup (in `DatabaseSeeder.SeedAsync`), so migrations will be applied automatically when the app starts. For production, prefer manual migration application in controlled deployment pipelines.

Running tests
-------------
- Run all tests from repository root:

```bash
dotnet test
```

- Tests live in the `Tests` project. Some tests use a test database or helpers. If tests require a real DB, make sure a test DB is configured or run tests after updating connection strings for test environment.

API endpoints & SignalR
-----------------------
- Common endpoints (versioning may be present in controllers):
  - `POST /api/v1/auth/register`
  - `POST /api/v1/auth/login`
  - `GET /api/v1/medicines/search`
  - `GET /api/v1/medicines/{id}/alternatives`
  - `POST /api/v1/reservations`
  - `POST /api/v1/reservations/{id}/approve` (Admin)
  - `POST /api/v1/reservations/{id}/reject` (Admin)
  - `PUT /api/v1/admin/stock` (Admin)
  - `GET /api/v1/pharmacies/`
  - `GET /api/v1/pharmacies/{id}`

- SignalR hub for realtime stock updates: `/hubs/stock`. The hub notifiers are implemented in `API/Hubs`.

Tradeoffs and notes
-------------------
- Architecture: The project uses a layered (clean) architecture (Domain, Application, Infrastructure, API). This improves separation of concerns and testability at the cost of extra indirection for small changes.

- Identity: ASP.NET Identity with GUID keys provides a full-featured auth system (roles, tokens), but is heavier than a custom lightweight JWT solution.

- Migrations at startup: Applying migrations automatically on app start is convenient in development but may be risky in production where DB changes require review/approval.

- Seeding & secrets: The seeder embeds sample credentials; the JWT secret in `appsettings.json` is a placeholder. For production use environment variables or secret stores and avoid committing secrets.

- Concurrency: `Stock` uses a `rowversion` column to support optimistic concurrency. Handle `DbUpdateConcurrencyException` when updating stocks in high-concurrency scenarios.

- Tests: Unit tests exist for services and controllers. Consider adding integration tests (using a disposable SQL Server container) and CI pipeline steps for better coverage.

Helpful commands summary
-----------------------
- Restore & build: `dotnet restore && dotnet build`
- Run API: `cd API && dotnet run`
- Add migration: `dotnet ef migrations add Name --project Infrastructure --startup-project API`
- Apply migrations manually: `dotnet ef database update --project Infrastructure --startup-project API`
- Run tests: `dotnet test`

Docker Compose example
----------------------
The following `docker-compose.yml` and `Dockerfile` examples show a simple local setup with SQL Server and the API. They use environment variables (from a `.env` file) to pass secrets and connection strings.

`docker-compose.yml` (example):

```yaml
version: '3.8'
services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${SA_PASSWORD}
    ports:
      - "1433:1433"
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $${SA_PASSWORD} -Q \"SELECT 1\" || exit 1"]
      interval: 10s
      timeout: 5s
      retries: 10
    volumes:
      - mssql-data:/var/opt/mssql

  api:
    build:
      context: ./API
      dockerfile: Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
      - Jwt__Issuer=${JWT_ISSUER}
      - Jwt__Audience=${JWT_AUDIENCE}
      - Jwt__SecretKey=${JWT_SECRET}
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      db:
        condition: service_healthy
    ports:
      - "5000:80"

volumes:
  mssql-data:
```

`API/Dockerfile` (example, multi-stage):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore "API/MedicineFinder.API.csproj"
RUN dotnet publish "API/MedicineFinder.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "MedicineFinder.API.dll"]
```

Bring up the stack:

```bash
# create a .env file (see example below)
docker compose up --build
```

Because the application runs `Database.MigrateAsync()` at startup, migrations will be applied automatically when the API container starts (once the DB is ready). If you prefer to run migrations manually, run the EF tools from a build container or run a one-off container that executes `dotnet ef database update`.

Environment variable examples
-----------------------------
Create a `.env` file (DO NOT commit to source control) with values used by `docker-compose.yml`:

```
# SQL Server strong SA password (must satisfy complexity rules)
SA_PASSWORD=YourStrong!Passw0rd

# Connection string used by the app (example for SQL Server container)
CONNECTION_STRING=Server=db;Database=MedicineFinderDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true;

# JWT settings
JWT_SECRET=ReplaceThisWithA32CharSecretKey!
JWT_ISSUER=MedicineFinder
JWT_AUDIENCE=MedicineFinderUsers

# Optional: override ASP.NET Core URLs
ASPNETCORE_URLS=http://+:80
```

Notes on environment variables and configuration binding
- To override `appsettings.json` nested values use double underscore in env variable names, e.g. `ConnectionStrings__DefaultConnection` or `Jwt__SecretKey`.
- For production deployments use a secret store or orchestration platform secrets; do not store credentials in `.env` files or commit them.

Contact / next steps
---------------------
If you want this README expanded (CI configuration, deployment guides, or more environment examples), tell me which section to expand.

