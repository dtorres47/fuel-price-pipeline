CREATE SCHEMA IF NOT EXISTS fuel_price;

CREATE TABLE IF NOT EXISTS fuel_price.diesel_fuel_price (
    product_code  TEXT           NOT NULL,
    area_code     TEXT           NOT NULL,
    period        DATE           NOT NULL,
    value         NUMERIC(12, 4) NOT NULL,
    unit          TEXT,
    product_name  TEXT,
    area_name     TEXT,
    raw           JSONB DEFAULT '{}'::jsonb,
    created_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
    PRIMARY KEY (product_code, area_code, period)
);
