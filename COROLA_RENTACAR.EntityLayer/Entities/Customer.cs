using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.EntityLayer.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseImageUrl { get; set; }

        public DateTime? DriverLicenseIssueDate { get; set; }

        public bool IsDriverLicenseSystemVerified { get; set; }
        public string DriverLicenseSystemMessage { get; set; }

        public DriverLicenseVerificationStatus DriverLicenseVerificationStatus { get; set; }

        public DateTime? DriverLicenseVerifiedDate { get; set; }
        public string DriverLicenseRejectionReason { get; set; }

        public DateTime BirthDate { get; set; }

        public List<Reservation> Reservations { get; set; }
    }
}