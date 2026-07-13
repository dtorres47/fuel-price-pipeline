-- View: v_latest_fuel_price
-- Shows the most recent price for each product/area combination.

CREATE OR REPLACE VIEW fuel_price.v_latest_fuel_price AS
SELECT DISTINCT ON (product_code, area_code)
    product_code,
    product_name,
    area_code,
    area_name,
    period,
    value,
    unit,
    updated_at
FROM fuel_price.diesel_fuel_price
ORDER BY product_code, area_code, period DESC;
