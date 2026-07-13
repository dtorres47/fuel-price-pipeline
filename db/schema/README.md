# Schema

Base schema and table definitions.

- `diesel_fuel_price.sql` → creates the `fuel_price` schema and the
  `fuel_price.diesel_fuel_price` table, plus its indexes and an `updated_at`
  trigger.

Primary key is `(product_code, area_code, period)`, so re-importing the same
period is an idempotent upsert rather than a duplicate.
