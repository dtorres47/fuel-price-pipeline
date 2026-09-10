namespace FuelPricePipeline.Infra.Postgres;

using FuelPricePipeline.Domain;

public interface IFuelRepository
{
    Task UpsertAsync(DieselFuelPrice fuelRate);
}
