# Dimensions

Lookup tables that add descriptive metadata for analytics and joins.

- `dim_product.sql` → product codes (e.g. `EPD2D` → No. 2 Diesel)
- `dim_area.sql` → area codes (e.g. `NUS` → U.S.)

Both live in the `fuel_price` schema and use `ON CONFLICT DO NOTHING`, so they're
safe to re-run.
