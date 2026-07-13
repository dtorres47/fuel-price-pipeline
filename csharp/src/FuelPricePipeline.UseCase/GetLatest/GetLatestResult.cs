using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelPricePipeline.Domain;

namespace FuelPricePipeline.UseCase.GetLatest
{
    public class GetLatestResult
    {
        public bool IsSuccess { get; init; }
        public DieselFuelPrice? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static GetLatestResult Success(DieselFuelPrice data) =>
            new() { IsSuccess = true, Data = data };

        public static GetLatestResult Failure(string error) =>
            new() { IsSuccess = false, ErrorMessage = error };
    }
}
