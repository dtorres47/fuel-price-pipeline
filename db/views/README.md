# Views

Reporting and analytics views over the `fuel_price` schema.

- `v_latest_fuel_price.sql` → most recent price per product/area, via
  `DISTINCT ON`. Depends on `fuel_price.diesel_fuel_price`, so apply it after the
  schema.
