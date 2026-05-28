using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public int PickupLocationId { get; set; }
        public Location PickupLocation { get; set; }

        public int ReturnLocationId { get; set; }
        public Location ReturnLocation { get; set; }

        public decimal TotalPrice { get; set; }

        public ReservationStatus ReservationStatus { get; set; }

        public string Description { get; set; }
    }
}
