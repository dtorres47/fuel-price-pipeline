# fuel-price-pipeline — Go

The Go implementation and primary focus of the project. A translation of the C# reference into idiomatic Go, keeping the same clean-architecture boundaries.

See the [root README](../README.md) for the project overview and ELT design.

## Architecture

```
go/
├── domain/     # DieselFuelPrice model, repository interface
├── service/    # EIA fetch + business logic
├── adapters/   # PostgreSQL (pgx) + CSV export
├── ports/      # Chi HTTP handlers
└── main.go     # composition root
```

Dependencies point inward: `ports` and `adapters` depend on `domain`, never the reverse. The repository is defined as an interface in `domain` and implemented in `adapters`, so the service layer stays free of database concerns.

## Run it

```bash
export EIA_API_KEY="your-key"        # free from https://www.eia.gov/opendata/
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel_price"

go run ./cmd/migrate    # apply schema
go run .                # fetch → store → export
```

## Endpoints

| Method | Route          | Purpose                          |
|--------|----------------|----------------------------------|
| GET    | `/getEIAData`  | Fetch fresh prices from EIA      |
| GET    | `/getAll`      | Return stored prices from the DB |
| POST   | `/save`        | Persist prices to the DB         |

## Stack

Go 1.26.5 · [Chi](https://github.com/go-chi/chi) router · [pgx/v5](https://github.com/jackc/pgx) · PostgreSQL · standard-library `net/http`, `encoding/csv`, `log`

## C# → Go translation

| Concern        | C#                        | Go                          |
|----------------|---------------------------|-----------------------------|
| HTTP client    | `HttpClient`              | `net/http`                  |
| Database       | Npgsql + Dapper           | pgx/v5                      |
| Dependency wiring | MS.Extensions.DI       | constructor injection       |
| Error handling | exceptions                | error values + `%w` wrapping|
| Config         | env vars                  | env vars                    |

Same architecture and business logic, expressed in each language's idioms.
