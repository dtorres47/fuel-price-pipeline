using System.ComponentModel.DataAnnotations;

namespace FuelPricePipeline.Domain
{
    public class DieselFuelPrice
    {
        [Required, StringLength(10)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string AreaCode { get; set; } = string.Empty;

        [Required]
        public string AreaName { get; set; } = string.Empty;

        [Required]
        public DateTime Period { get; set; }

        [Range(0, 1000)]
        public decimal Value { get; set; }

        [Required]
        public string Unit { get; set; } = string.Empty;
    }
}