using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.WebUI.Models
{
    public class PublicCarFilterViewModel
    {
        public string? SearchTerm { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }

        public string? FuelType { get; set; }

        public string? TransmissionType { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int? SeatCount { get; set; }

        public bool OnlyAvailable { get; set; } = true;

        public List<Car> Cars { get; set; } = new();

        public List<Brand> Brands { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public List<string> FuelTypes { get; set; } = new();

        public List<string> TransmissionTypes { get; set; } = new();

        public int TotalCarCount { get; set; }

        public int FilteredCarCount { get; set; }
    }
}