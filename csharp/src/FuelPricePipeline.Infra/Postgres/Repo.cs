namespace FuelPricePipeline.Infra.Postgres;

using FuelPricePipeline.Domain;
using Npgsql;
using Polly;
using Dapper;

public class Repo
{
    private readonly string _connectionString;
    private readonly IAsyncPolicy _retryPolicy;

    public Repo(string connectionString)
    {
        _connectionString = connectionString;
        _retryPolicy = Policy
            .Handle<NpgsqlException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task UpsertAsync(DieselFuelPrice fuelRate)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO eia.fuel_price
                    (product_code, area_code, period, value, unit, product_name, area_name, raw)
                VALUES
                    (@ProductCode, @AreaCode, @Period, @Value, @Unit, @ProductName, @AreaName, '{}'::jsonb)
                ON CONFLICT (product_code, area_code, period)
                DO UPDATE SET
                    value = EXCLUDED.value,
                    unit = EXCLUDED.unit,
                    updated_at = NOW(),
                    raw = EXCLUDED.raw;
            ";

            await conn.ExecuteAsync(sql, fuelRate);
        });
    }

    private NpgsqlConnection CreateConnection()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            MaxPoolSize = 20,
            MinPoolSize = 5,
            ConnectionIdleLifetime = 300,
            Timeout = 30
        };
        return new NpgsqlConnection(connStringBuilder.ConnectionString);
    }
}