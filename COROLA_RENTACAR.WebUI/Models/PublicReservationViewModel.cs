using Microsoft.AspNetCore.Http;

namespace COROLA_RENTACAR.WebUI.Models
{
    public class PublicReservationViewModel
    {
        public int CarId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string DriverLicenseNumber { get; set; } = string.Empty;

        public IFormFile? DriverLicenseImage { get; set; }
        public string? DriverLicenseImageUrl { get; set; }

        public DateTime? DriverLicenseIssueDate { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public int PickupLocationId { get; set; }
        public int ReturnLocationId { get; set; }

        public string? Description { get; set; }
    }
}