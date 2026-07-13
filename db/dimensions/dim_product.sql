-- Dimension: dim_product
-- Stores metadata about fuel products.

CREATE TABLE IF NOT EXISTS fuel_price.dim_product (
    product_code TEXT PRIMARY KEY,
    product_name TEXT NOT NULL,
    description  TEXT
);

-- Example seed rows (optional)
INSERT INTO fuel_price.dim_product (product_code, product_name, description) VALUES
    ('EPD2D', 'No. 2 Diesel', 'No. 2 Diesel Fuel Price (Retail)'),
    ('EPGSL', 'Gasoline', 'Conventional Gasoline Price')
ON CONFLICT (product_code) DO NOTHING;
