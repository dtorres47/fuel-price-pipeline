namespace FuelPricePipeline.Infra.Eia;

using FuelPricePipeline.Domain;

public interface IEiaClient
{
    Task<DieselFuelPrice?> FetchLatestDieselAsync(string area = "NUS");
}
