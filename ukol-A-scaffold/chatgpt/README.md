# PhysioBook demo scaffold

Demo scaffold rezervačního systému pro fyzioterapeuty se zaměřením na Vertical Slice + CQRS, ASP.NET Core Minimal API a React frontend.

## Co je součástí
- ASP.NET Core 8 Minimal API backend
- PostgreSQL + EF Core + ASP.NET Identity + JWT
- Cortex.Mediator wiring pro CQRS slice operace
- Articles CRUD jako ukázková feature
- React 18 + Vite + TypeScript + Tailwind frontend
- Docker + docker-compose
- xUnit validační testy pro Articles

## Spuštění
```bash
docker-compose up --build
```

## URL
- Frontend: `http://localhost:5173`
- Backend API: `http://localhost:5050`
- Swagger: `http://localhost:5050/swagger`

## Seedované demo přihlašovací údaje
- Email: `admin@physiobook.demo`
- Heslo: `Admin123!`

## Poznámky
- Stripe je zatím připraven pouze jako budoucí integrační bod v konfiguraci, protože v tomto kroku je cílem scaffold + Articles CRUD.
- `DatabaseConfiguration` používá `MigrateAsync()` když jsou k dispozici migrace; pro čistý demo scaffold zároveň fallbackuje na `EnsureCreatedAsync()`, aby šel projekt jednoduše spustit i bez předgenerovaných EF migrací.
