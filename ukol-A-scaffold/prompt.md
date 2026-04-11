# Úkol A: Scaffold — Prompt (identický pro všechny nástroje)

Vytvoř kompletní scaffold webové aplikace PhysioBook, rezervační systém pro fyzioterapeuty s online platbami. Jedná se o demo projekt. Aplikace umožňuje klientům rezervovat si termíny u fyzioterapeutů a platit online přes Stripe, avšak teď bude za úkol vytvořit pouze scaffold, article entitu a základní CRUD na articles.

### Tech Stack

- Backend: ASP.NET Core 8.0 (Minimal API)
- Frontend: React 18 + Vite + TypeScript + TailwindCSS
- Databáze: PostgreSQL 15 + Entity Framework Core 8 (Npgsql)
- Autentizace: ASP.NET Identity + JWT Bearer tokeny
- Mediator/CQRS: Cortex.Mediator (NuGet `Cortex.Mediator`) — verze 1.6.1+
- Validace: FluentValidation + pipeline behavior
- Testy: xUnit + FluentAssertions + Moq
- Kontejnerizace: Docker + docker-compose (vše v kontejnerech — backend, frontend, PostgreSQL)

### Architektura: Vertical Slice + CQRS

Projekt používá Vertical Slice Architecture s CQRS patternem. Každá operace (command/query) žije ve vlastním adresáři se 7 soubory.

#### Adresářová struktura backendu

PhysioBook/
├── Auth/
│   ├── AppPolicies.cs              # Authorization policies (Visitor, Admin (Therapist))
│   ├── AppRoles.cs                 # Role constants (Admin (Therapist))
│   └── JwtAuthSettings.cs          # JWT + Identity konfigurace
├── Configurations/
│   ├── ServiceRegistrations.cs     # DI registrace všech služeb
│   ├── AppConfiguration.cs         # Middleware pipeline
│   ├── EndpointRegistration.cs     # Centrální registrace VŠECH endpointů
│   └── DatabaseConfiguration.cs    # EF migrace + seed
├── Data/
│   ├── PhysioBookContext.cs        # DbContext (IDbContextFactory pattern)
│   ├── Entities/                   # Doménové entity + IEntityTypeConfiguration v jednom souboru
│   ├── Enums/                      # Business enumerace
│   ├── Migrations/
│   └── Shared/
│       ├── PagedList.cs            # PagedList<T> pro stránkované responses
│       ├── PaginationParameters.cs # Sdílené parametry stránkování
│       └── QueryableExtensions.cs  # IQueryable<T>.ToPagedListAsync() extension
├── Exceptions/
│   ├── GlobalExceptionHandlerMiddleware.cs
│   └── ApiResultHelper.cs          # Obaluje handler volání, mapuje výjimky na HTTP responses
├── Extensions/
│   └── ValidationBehavior.cs       # FluentValidation pipeline behavior
├── Features/
│   └── Articles/                   # Ukázková feature
│       ├── Commands/
│       │   ├── CreateArticle/      # 7 souborů
│       │   ├── UpdateArticle/
│       │   └── DeleteArticle/
│       └── Queries/
│           ├── GetArticles/
│           └── GetArticleById/
├── Middleware/
│   └── CorrelationIdMiddleware.cs
├── Program.cs                      # Minimální entry point
├── GlobalUsings.cs
├── appsettings.json
└── appsettings.Development.json

#### 7-file struktura per operace

Každý command/query má přesně tyto soubory:

Features/{Feature}/Commands/{Operation}/
├── {Operation}Command.cs           # CQRS command — implements IQuery<TResponse>
├── {Operation}CommandHandler.cs    # Handler — implements IQueryHandler<TCommand, TResponse>, injectuje IDbContextFactory<PhysioBookContext>
├── {Operation}Request.cs           # HTTP request DTO (record)
├── {Operation}Response.cs          # HTTP response DTO (record, PascalCase properties)
├── {Operation}Validator.cs         # FluentValidation (AbstractValidator<TCommand>)
├── {Operation}Endpoint.cs          # MinimalAPI endpoint (static extension method)
└── {Operation}Mappings.cs          # Ruční mapping extension metody (Request→Command, Command→Entity, Entity→Response)

Pro queries je struktura analogická (Query místo Command).

### CQRS Pattern — KRITICKÉ PRAVIDLO

Cortex.Mediator má POUZE `IQueryHandler<TRequest, TResponse>`. Neexistuje žádné `ICommandHandler`!

// SPRÁVNĚ — pro commands I queries:
public class CreateArticleCommandHandler : IQueryHandler<CreateArticleCommand, CreateArticleResponse>

// ŠPATNĚ — toto rozhraní NEEXISTUJE:
public class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand, CreateArticleResponse>

Command/Query třída implementuje `IQuery<TResponse>`:

public class CreateArticleCommand : IQuery<CreateArticleResponse>
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? UserId { get; set; }
}

GlobalUsings:

global using Cortex.Mediator;
global using Cortex.Mediator.Queries;
global using Cortex.Mediator.DependencyInjection;
global using FluentValidation;
global using Microsoft.EntityFrameworkCore;

Registrace mediatoru:

builder.Services.AddCortexMediator(builder.Configuration, new[] { typeof(Program) });
builder.Services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(ValidationBehavior<,>));

Volání z endpointu:

var response = await mediator.SendAsync<CreateArticleCommand, CreateArticleResponse>(command, ct);

### JSON Serializace

Backend automaticky konvertuje PascalCase → camelCase:

| Vrstva | Konvence | Příklad |
|--------|----------|---------|
| C# DTOs | PascalCase | Title, CreatedAt |
| JSON API | camelCase | title, createdAt |
| TypeScript | camelCase | response.title |

### Autentizace — ASP.NET Identity + JWT

- ASP.NET Identity pro správu uživatelů (registrace, login, role)
- JWT Bearer tokeny pro API autorizaci
- Identity tabulky ve stejné PostgreSQL databázi
- Žádná externí závislost (žádný Keycloak, Auth0 apod.)

Role: Admin
Policies: Visitor (kdokoliv), Admin (Therapist)
Auth endpointy: /api/auth/register, /api/auth/login, /api/auth/refresh-token

### Databáze — Entity pattern

Entity + EF konfigurace v jednom souboru. Handlery přistupují k datům přímo přes IDbContextFactory<PhysioBookContext> — žádný Repository pattern, žádný RepositoryBase<T>. EF Core DbContext je repository i Unit of Work — nepřidávej další abstrakční vrstvu.

### Endpoint pattern

Endpointy se registrují centrálně v EndpointRegistration.cs (NIKDY v Program.cs).
Endpoint je static extension method na IEndpointRouteBuilder.
Každý endpoint volá ApiResultHelper.ExecuteWithErrorHandling pro konzistentní error handling.

### Error handling

ApiResultHelper mapuje výjimky:
- ValidationException → 400
- KeyNotFoundException → 404
- UnauthorizedAccessException → 401
- InvalidOperationException → 400
- Exception → 500

Formát: ProblemDetails (RFC 7807).

### Mapping pattern

Ruční statické extension metody — žádný AutoMapper:
- Request→Command (přidá server-side data jako UserId z JWT)
- Command→Entity
- Entity→Response

### Frontend struktura

physiobook-frontend/
├── src/
│   ├── App.tsx                     # Routing (react-router-dom v6, lazy loading)
│   ├── main.tsx
│   ├── contexts/ (AuthContext.tsx, ToastContext.tsx)
│   ├── features/ (per feature: service, hooks, types)
│   ├── shared/ (BaseApiService.ts, ApiResponse.ts)
│   ├── components/ (Header, Footer, ProtectedRoute)
│   └── pages/
├── vite.config.ts, tailwind.config.js, tsconfig.json, package.json, Dockerfile

BaseApiService automaticky přidává JWT Bearer token do Authorization headeru.

### Docker — docker-compose.yml

Všechny služby v kontejnerech, jeden příkaz spustí vše:
- postgres (PostgreSQL 15, port 5432)
- backend (ASP.NET Core 8, port 5050)
- frontend (React + Vite + Nginx, port 5173)

### Testy

xUnit + FluentAssertions (NIKDY Verify snapshot testing) + Moq pro mocking.
Test pattern: {Feature}ValidatorTests, {Feature}HandlerTests.

### Program.cs — minimální

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();   // → ServiceRegistrations.cs
var app = builder.Build();
app.ConfigureApp();            // → AppConfiguration.cs → EndpointRegistration
await DatabaseConfiguration.InitializeAsync(app.Services);
app.Run();

### Požadovaný výstup

Vygeneruj kompletní scaffold se všemi soubory. Zahrň:
1. Celou backend strukturu včetně kompletní Articles CRUD feature (všech 5 operací × 7 souborů)
2. Auth endpointy (register, login, refresh)
3. Frontend s Articles stránkami (CRUD) a auth flow
4. Docker setup (Dockerfile pro backend, Dockerfile pro frontend, docker-compose.yml)
5. Testovací projekt s testy pro Articles validátory
6. Všechny konfigurační soubory (appsettings, .csproj, package.json, vite.config, tailwind.config)

Kód musí být funkční a připravený ke spuštění přes docker-compose up.
