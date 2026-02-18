# Medicine Finder Backend (ASP.NET Core 8)

## Overview
Production-style Clean Architecture backend for a Medicine Finder system.

## Prerequisites
- .NET SDK 8.x
- SQL Server LocalDB or SQL Server instance

## Setup
1. Restore packages:

```bash
dotnet restore
```

2. Apply migrations:

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

3. Run the API:

```bash
dotnet run --project API
```

Swagger UI is available at `https://localhost:5001/swagger` (or the URL shown in output).

## Configuration
`API/appsettings.json`:
- `ConnectionStrings:DefaultConnection`
- `Jwt` settings

## Seeded Credentials
- Admin: `admin@mf.local` / `Admin123!`
- User: `user@mf.local` / `User123!`

## API Routes
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/medicines/search`
- `GET /api/v1/medicines/{id}/alternatives`
- `POST /api/v1/reservations`
- `POST /api/v1/reservations/{id}/approve` (Admin)
- `POST /api/v1/reservations/{id}/reject` (Admin)
- `PUT /api/v1/admin/stock` (Admin)
- `GET /api/v1/pharmacies/{id}`

## Example Search
```
GET /api/v1/medicines/search?keyword=relief&inStockOnly=true&sortBy=Nearest&lat=30.04&lng=31.23
```

## SignalR
- Hub: `/hubs/stock`
- Event: `StockUpdated(pharmacyId, medicineId, quantity)`

## Tests
```
dotnet test
```
