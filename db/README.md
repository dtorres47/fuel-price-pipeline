# Database

SQL definitions for the pipeline's PostgreSQL layer. Everything lives under the
`fuel_price` schema — the general container for fuel price data — with
`diesel_fuel_price` as the first table. Other fuels can be added later as sibling
tables (e.g. `gasoline_fuel_price`) under the same schema.

## Layout

| Folder        | Purpose                                                        |
|---------------|----------------------------------------------------------------|
| `schema/`     | Core table, indexes, and the `updated_at` trigger              |
| `dimensions/` | Lookup tables (`dim_product`, `dim_area`) for analytics        |
| `seeds/`      | Seed data to preload dimensions and static lookups             |
| `views/`      | Reporting views (e.g. latest price per product/area)           |

## Apply order

1. `schema/diesel_fuel_price.sql` — creates the schema, table, indexes, trigger
2. `dimensions/dim_product.sql`, `dimensions/dim_area.sql` — lookup tables
3. `seeds/seed_dimensions.sql` — preload dimension rows
4. `views/v_latest_fuel_price.sql` — reporting view (depends on the table)
