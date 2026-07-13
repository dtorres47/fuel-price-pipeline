-- Schema: fuel_price
-- The fuel_price schema is the general container for fuel price data.
-- diesel_fuel_price is the first table; other fuels can be added as sibling
-- tables (e.g. gasoline_fuel_price) under the same schema.

CREATE SCHEMA IF NOT EXISTS fuel_price;

CREATE TABLE IF NOT EXISTS fuel_price.diesel_fuel_price (
    product_code  TEXT           NOT NULL,
    area_code     TEXT           NOT NULL,
    period        DATE           NOT NULL,
    value         NUMERIC(12,4)  NOT NULL,
    unit          TEXT,
    product_name  TEXT,
    area_name     TEXT,
    raw           JSONB,
    created_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    PRIMARY KEY (product_code, area_code, period)
);

CREATE INDEX IF NOT EXISTS idx_diesel_fuel_price_period  ON fuel_price.diesel_fuel_price (period);
CREATE INDEX IF NOT EXISTS idx_diesel_fuel_price_product ON fuel_price.diesel_fuel_price (product_code);
CREATE INDEX IF NOT EXISTS idx_diesel_fuel_price_area    ON fuel_price.diesel_fuel_price (area_code);

CREATE OR REPLACE FUNCTION fuel_price.set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_diesel_fuel_price_updated_at ON fuel_price.diesel_fuel_price;

CREATE TRIGGER trg_diesel_fuel_price_updated_at
BEFORE UPDATE ON fuel_price.diesel_fuel_price
FOR EACH ROW
EXECUTE FUNCTION fuel_price.set_updated_at();
