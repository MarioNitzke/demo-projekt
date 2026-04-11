# PhysioBook Demo Scaffold

Demo scaffold pro rezervační systém fyzioterapeutů (zatím jen auth + `articles` CRUD).

## Stack
- Backend: ASP.NET Core 8 Minimal API, EF Core 8 (Npgsql), ASP.NET Identity + JWT, Cortex.Mediator, FluentValidation
- Frontend: React 18, Vite, TypeScript, TailwindCSS
- DB: PostgreSQL 15
- Testy: xUnit, FluentAssertions, Moq
- Kontejnery: Docker + docker-compose

## Struktura
- `PhysioBook/` backend
- `physiobook-frontend/` frontend
- `tests/PhysioBook.Tests/` unit testy validatorů

## Spuštění přes Docker
```bash
docker compose up --build
```

- Frontend: `http://localhost:5173`
- Backend Swagger: `http://localhost:5050/swagger`

## Lokální běh bez Dockeru
```bash
cd PhysioBook
dotnet restore
dotnet run
```

```bash
cd physiobook-frontend
npm install
npm run dev
```

## Testy
```bash
dotnet test tests/PhysioBook.Tests/PhysioBook.Tests.csproj
```

## Smoke test
```bash
python3 scripts/smoke_test.py
```

## Seed admin účet
- Email: `admin@physiobook.local`
- Heslo: `Admin123!`

## EF migrace
První migrace je vytvořená ve `PhysioBook/Data/Migrations/`.

Pro další migrace použij:
```bash
dotnet ef migrations add <NazevMigrace> --project PhysioBook/PhysioBook.csproj --startup-project PhysioBook/PhysioBook.csproj --output-dir Data/Migrations
```

