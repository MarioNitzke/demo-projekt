# Claude Code Session

Session: `543f4a82-b756-4375-b56c-060af2e13f67`

---

## Uživatel

Vytvoř GitHub Actions CI/CD pipeline pro tento projekt. Po každém pushnutí nebo pull requestu na main se má automaticky spustit build a testy pro backend i frontend. Backend testy potřebují PostgreSQL databázi. Výsledky buildu ulož jako artifact.

## Claude Code

> **Agent**: Explore project structure

Mám přehled o projektu. Je to full-stack aplikace **PhysioBook** — .NET 8 backend s xUnit testy, React + TypeScript frontend s Vite, PostgreSQL databáze.

Vytvořím CI/CD pipeline.

> **Bash**: Check if workflows directory exists
> ```
> ls -la /Users/marioboss/demo-projekt/.github/workflows/ 2>/dev/null || echo "Directory does not exist"
> ```

> **Bash**: Create workflows directory
> ```
> mkdir -p /Users/marioboss/demo-projekt/.github/workflows
> ```

> **Write**: /Users/marioboss/demo-projekt/.github/workflows/ci-cd.yml (96 řádků)

Pipeline je vytvořen v `.github/workflows/ci-cd.yml`. Zde je shrnutí:

**Trigger:** Push a pull request na `main`

**Backend job:**
- PostgreSQL 15 service container (s health checkem)
- .NET 8 SDK — restore, build, test (`dotnet test` s xUnit)
- Connection string předán přes env proměnnou
- Výsledky testů uploadnuty jako artifact (`backend-test-results`)
- Publikovaný build uploadnut jako artifact (`backend-build`)

**Frontend job (paralelně):**
- Node.js 20 s npm cache
- `npm ci` → TypeScript type check → `npm run build`
- Výstupní `/dist` složka uploadnuta jako artifact (`frontend-build`)
