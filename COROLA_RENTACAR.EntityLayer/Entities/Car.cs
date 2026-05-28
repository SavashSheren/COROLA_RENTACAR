using System.Collections.Generic;

namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class Car
    {
        public int CarId { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string PlateNumber { get; set; }

        public decimal DailyPrice { get; set; }

        public int SeatCount { get; set; }
        public int LuggageCapacity { get; set; }
        public int Mileage { get; set; }

        public string FuelType { get; set; }
        public string TransmissionType { get; set; }

        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; }

        public List<CarImage> CarImages { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}