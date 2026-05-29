namespace COROLA_RENTACAR.WebUI.Areas.Admin.Models
{
    public class AiCarDescriptionViewModel
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string FuelType { get; set; } = string.Empty;

        public string Transmission { get; set; } = string.Empty;

        public int SeatCount { get; set; } = 5;

        public decimal DailyPrice { get; set; }

        public string Features { get; set; } = string.Empty;

        public string? GeneratedDescription { get; set; }
    }
}