# fuel-price-pipeline

**A PostgreSQL ELT pipeline that tracks EIA diesel prices to solve a real invoicing workflow. Chi-based REST API with CSV export. Demonstrates C#→Go architectural translation.**

I create freight invoices for my company. This project automates downloading from [EIA](https://www.eia.gov/petroleum): **Extract** from the EIA API, **Load** into PostgreSQL, **Transform** into query-ready views, then serve the data over REST and export a CSV for invoicing.

Built primarily in **Go** (the focus), with a **C# reference implementation** and a lean **Angular** UI. All three follow the same clean-architecture principles, idiomatic to each stack.

---

## Architecture (ELT)

```
EIA API  ──Extract──▶  PostgreSQL (raw)  ──Transform──▶  Views  ──▶  REST API + CSV
```

- **Extract** — HTTP client pulls diesel prices from the EIA v2 API.
- **Load** — raw rows land in `fuel_price.diesel_fuel_price` (idempotent upserts).
- **Transform** — SQL views derive the latest price per product/area for querying.

The schema `fuel_price` is the general container; `diesel_fuel_price` is the first table under it, leaving room for other fuels as sibling tables.

---

## Go implementation (focus)

Clean architecture with clear layer boundaries:

```
go/
├── domain/     # DieselFuelPrice model, repository interface
├── service/    # EIA fetch + business logic
├── adapters/   # PostgreSQL (pgx) + CSV export
├── ports/      # Chi HTTP handlers
└── main.go     # composition root
```

**Stack:** Go 1.26.5 · [Chi](https://github.com/go-chi/chi) router · [pgx](https://github.com/jackc/pgx) · PostgreSQL

**Endpoints:**

| Method | Route          | Purpose                          |
|--------|----------------|----------------------------------|
| GET    | `/getEIAData`  | Fetch fresh prices from EIA      |
| GET    | `/getAll`      | Return stored prices from the DB |
| POST   | `/save`        | Persist prices to the DB         |

---

## Run it

```bash
# 1. Start PostgreSQL
docker compose up -d

# 2. Set environment
export EIA_API_KEY="your-key"        # free from https://www.eia.gov/opendata/
export FUEL_DSN="postgres://postgres:postgres@localhost:5432/fuel_price"

# 3. Migrate + run
cd go
go run ./cmd/migrate
go run .
```

Output CSV:

```
product_code,product_name,area_code,area_name,period,value,unit,generated_utc
EPD2D,No 2 Diesel,NUS,U.S.,2025-08,3.744,$/GAL,2025-09-20T06:39:46Z
```

---

## C# reference · Angular UI

- **C#/.NET** — the stack I have production experience in. Implemented first, then translated to Go. Uses DI, `IHttpClientFactory`, Polly retries, Dapper, and the Result pattern. See `csharp/`.
- **Angular** — a deliberately thin UI layer to demonstrate full-stack reach, not a frontend showcase. See `angular/`.

---

## Tech stack

**Go** · **C#/.NET** · **Angular** · **PostgreSQL** · **Chi** · **pgx** · **Docker**
