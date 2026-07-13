# fuel-price-pipeline — C#

The C#/.NET reference implementation — the stack I have production experience in. Implemented first, then translated to Go.

See the [root README](../README.md) for the project overview and ELT design.

## Architecture

```
csharp/src/
├── FuelPricePipeline.Domain/       # DieselFuelPrice model
├── FuelPricePipeline.Infra/        # EIA client, Postgres repo, CSV export
├── FuelPricePipeline.UseCase/      # GetLatest business logic
├── FuelPricePipeline.Cmd.FuelLatest/  # CLI entry point
├── FuelPricePipeline.Cmd.Migrate/     # DB migration
└── FuelPricePipeline.Tests/           # Unit tests
```

The use case depends on `Domain` and infrastructure abstractions, wired together with dependency injection at the command layer.

## Run it

```bash
export EIA_API_KEY="your-key"        # free from https://www.eia.gov/opendata/
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel_price"

dotnet run --project src/FuelPricePipeline.Cmd.Migrate       # apply schema
dotnet run --project src/FuelPricePipeline.Cmd.FuelLatest    # fetch → store → export
dotnet test                                               # run tests
```

## Stack

.NET 10.0.9 · Npgsql + Dapper · Polly (retry) · MSTest + Moq

## Design patterns

- **Dependency injection** — `IHttpClientFactory` and services wired via `Microsoft.Extensions.DependencyInjection`.
- **Repository pattern** — data access behind an abstraction.
- **Result pattern** — explicit success/failure return instead of exceptions for control flow.
- **Retry** — Polly handles transient database failures with exponential backoff.
