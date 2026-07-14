using Npgsql;
using Microsoft.Extensions.Logging;

namespace FuelPricePipeline.Infra.Postgres
{
    public static class DbInitializer
    {
        public static async Task ApplyMigrationsAsync(string connectionString, ILogger? logger = null)
        {
            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                logger?.LogInformation("Applying database migrations...");

                // Create schema if it doesn't exist
                await using (var cmd = new NpgsqlCommand(@"
                    CREATE SCHEMA IF NOT EXISTS fuel_price;
                ", conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                // Create table if it doesn't exist
                await using (var cmd = new NpgsqlCommand(@"
                    CREATE TABLE IF NOT EXISTS fuel_price.diesel_fuel_price (
                        id BIGSERIAL PRIMARY KEY,
                        product_code TEXT NOT NULL,
                        area_code TEXT NOT NULL,
                        period DATE NOT NULL,
                        value NUMERIC(10, 4) NOT NULL,
                        unit TEXT NOT NULL,
                        product_name TEXT,
                        area_name TEXT,
                        raw JSONB DEFAULT '{}'::jsonb,
                        created_at TIMESTAMPTZ DEFAULT NOW(),
                        updated_at TIMESTAMPTZ DEFAULT NOW(),
                        CONSTRAINT uq_diesel_fuel_price_product_area_period 
                            UNIQUE (product_code, area_code, period)
                    );
                ", conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                // Create indexes
                await using (var cmd = new NpgsqlCommand(@"
                    CREATE INDEX IF NOT EXISTS idx_diesel_fuel_price_lookup 
                    ON fuel_price.diesel_fuel_price(product_code, area_code, period DESC);

                    CREATE INDEX IF NOT EXISTS idx_diesel_fuel_price_period 
                    ON fuel_price.diesel_fuel_price(period DESC);
                ", conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                logger?.LogInformation("Database migrations completed successfully");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error applying database migrations");
                throw;
            }
        }
    }
}