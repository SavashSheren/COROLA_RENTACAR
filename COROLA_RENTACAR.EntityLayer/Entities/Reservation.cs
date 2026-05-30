using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        public string? ReservationCode { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public DateTime PickupDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public int PickupLocationId { get; set; }

        public Location PickupLocation { get; set; } = null!;

        public int ReturnLocationId { get; set; }

        public Location ReturnLocation { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public ReservationStatus ReservationStatus { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}