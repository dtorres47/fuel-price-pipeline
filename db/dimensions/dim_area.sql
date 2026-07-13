-- Dimension: dim_area
-- Stores metadata about areas/regions.

CREATE TABLE IF NOT EXISTS fuel_price.dim_area (
    area_code   TEXT PRIMARY KEY,
    area_name   TEXT NOT NULL,
    description TEXT
);

-- Example seed rows (optional)
INSERT INTO fuel_price.dim_area (area_code, area_name, description) VALUES
    ('NUS', 'U.S.', 'United States total'),
    ('PET', 'Petroleum Admin District', 'Regional grouping for petroleum data')
ON CONFLICT (area_code) DO NOTHING;
