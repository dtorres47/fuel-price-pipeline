-- Run this after creating dim_product and dim_area to preload data.
-- Adds additional seed rows or updates descriptions.

-- Example: ensure "NUS" (U.S. aggregate) always exists
INSERT INTO fuel_price.dim_area (area_code, area_name, description) VALUES
    ('NUS', 'U.S.', 'United States aggregate area')
ON CONFLICT (area_code) DO NOTHING;
