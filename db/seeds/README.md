# Seeds

Seed data to populate dimension tables or static lookups.

- `seed_dimensions.sql` → preloads baseline dimension rows. Run it after the
  `dimensions/` tables exist. Idempotent (`ON CONFLICT DO NOTHING`).
