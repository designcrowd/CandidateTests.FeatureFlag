
# Feature Flags Admin

The **Feature Flags Admin** is a small full-stack application for managing feature flags. It lets you list, create, edit, and delete flags, and evaluate a flag for a given user against a rollout percentage — the same shape of tool teams use in production to control feature rollouts.

### Features
- **Flag Management**: Create, read, update, and delete feature flags.
- **Rollout Control**: Each flag has an on/off switch and a 0–100 rollout percentage.
- **Deterministic Evaluation**: A given `(userId, flagKey)` pair always returns the same result.
- **Test-as-user Panel**: Evaluate any flag for any user from the UI.
- **Versioned Migrations**: Database schema and seed data are managed by DbUp on startup.

### Key Components

- **`Flag`** (`Data/Flag.cs`): The entity stored in the database (`key`, `name`, `enabled`, `rolloutPercent`, timestamps).
- **`IFlagDao` / `FlagDao`** (`Data/`): Dapper-backed data access. Each method opens its own `SqliteConnection` from the connection string and runs the SQL in `FlagSqlQueries`.
- **`FlagSqlQueries`** (`Data/FlagSqlQueries.cs`): SQL constants kept in one place — `SelectAll`, `SelectByKey`, `Insert`, `UpdateByKey`, `DeleteByKey`.
- **`FlagEvaluator`** (`Evaluation/FlagEvaluator.cs`): Decides whether a flag is enabled for a given user. Uses an FNV-1a hash of `userId + flagKey` to bucket users 0–99 and compares against the rollout percentage.
- **`FlagsController`** (`Controllers/FlagsController.cs`): ASP.NET Core controller exposing the REST surface.
- **`DbMigratorService`** (`Migrations/DbMigratorService.cs`): Wraps DbUp. On startup, runs any pending SQL scripts embedded in the assembly under `Migrations/Scripts/`.
- **`useFlags`** (`web/composables/useFlags.ts`): Vue composable holding the list state and wrapping the API client.

### Installation

Prerequisites:
- .NET 8 SDK
- Node 20+

Clone the repo and install frontend dependencies:

```bash
cd web
npm install
```

The .NET project restores on first build.

### Running

**Backend** — from `src/CandidateTests.FeatureFlags.Api`:

```bash
dotnet run
```

The API listens on `http://localhost:5080`. Swagger UI is at `/swagger` in Development. On first run a SQLite database (`flags.db`) is created next to the project; DbUp then applies the two migration scripts (`Script0001_CreateFlagsTable.sql` and `Script0002_SeedFlags.sql`) and records them in a `SchemaVersions` journal table so subsequent starts skip them.

**Frontend** — from `web/`:

```bash
npm run dev
```

Nuxt serves on `http://localhost:4000`.

### Example Usage

Create a flag:

```bash
curl -X POST http://localhost:5080/flags \
  -H "Content-Type: application/json" \
  -d '{"key":"new-onboarding","name":"New onboarding flow","enabled":true,"rolloutPercent":25}'
```

Evaluate it for a user:

```bash
curl "http://localhost:5080/evaluate?userId=user-123&flagKey=new-onboarding"
```

Example response:

```json
{ "enabled": true }
```

### API Surface

| Method   | Path                            | Description |
|----------|---------------------------------|---|
| `GET`    | `/flags`                        | List all flags |
| `GET`    | `/flags/{key}`                  | Fetch a single flag |
| `POST`   | `/flags`                        | Create a flag |
| `PUT`    | `/flags/{key}`                  | Update name / enabled / rollout |
| `DELETE` | `/flags/{key}`                  | Delete a flag |
| `GET`    | `/evaluate?userId=&flagKey=`    | Returns `{ enabled: bool }` |

### How It Works

1. **Migrations on startup**: `DbMigratorService` runs DbUp against the configured connection string. Any script in `Migrations/Scripts/` not yet recorded in `SchemaVersions` is executed in transactional order.
2. **Stored configuration**: Each flag is persisted with a key, a name, an `enabled` switch, and a rollout percentage (0–100).
3. **Deterministic bucketing**: When `/evaluate` is called, `FlagEvaluator` computes a stable FNV-1a hash of `userId:flagKey` and reduces it to a bucket 0–99.
4. **Decision**: The user is "in" if the bucket is below the flag's rollout percentage. A disabled flag is always off; a 100% flag is always on; an unknown flag returns `{ enabled: false }` (fail-closed).

### Testing

The solution includes an xUnit test project (`tests/CandidateTests.FeatureFlags.Tests`) using FluentAssertions, AutoFixture, and NSubstitute for the test stack. It exercises:

- **`FlagEvaluator`** (unit tests in `Evaluation/`): returns true for an enabled flag at 100% rollout; returns false when a flag is disabled.
- **`FlagsController`** (integration tests in `Controllers/` via `WebApplicationFactory<Program>`): list returns seeded flags; `GET /flags/{key}` returns a flag or 404; `POST` creates; `PUT` updates; `DELETE` removes and the follow-up `GET` returns 404; `/evaluate` returns the expected state.

Run the tests with:

```bash
dotnet test
```

Integration tests use an in-memory SQLite connection (`Mode=Memory;Cache=Shared`) so the suite is self-contained and never touches the on-disk dev DB.

### Repository Layout

```
src/CandidateTests.FeatureFlags.Api/
  Controllers/FlagsController.cs        REST surface
  Data/                                  Flag entity, IFlagDao, FlagDao, FlagSqlQueries
  Evaluation/FlagEvaluator.cs            Bucketing + rollout decision
  Migrations/
    DbMigratorService.cs                 DbUp wrapper
    IDbMigratorService.cs
    Scripts/
      Script0001_CreateFlagsTable.sql    Schema
      Script0002_SeedFlags.sql           Seed data
  Program.cs                             Composition root

web/
  pages/index.vue                        Main page — list, drawer form, test-user panel
  components/                            FlagList, FlagForm (drawer), TestUserPanel
  composables/useFlags.ts                State + API client
  types/flag.ts                          Shared TypeScript types

tests/CandidateTests.FeatureFlags.Tests/
  ApiFactory.cs                          Shared WebApplicationFactory infrastructure
  Controllers/FlagsControllerTests.cs    Integration tests for the controller
  Evaluation/FlagEvaluatorTests.cs       Unit tests for the evaluator
```

### Notes for Candidates

You'll have this codebase ahead of your live coding interview. Read it end to end, run it, hit the endpoints, look at the tests. You'll be asked how it works and where you'd improve it, then given one feature to extend or rework using your AI tool of choice.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
