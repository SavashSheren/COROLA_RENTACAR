using COROLA_RENTACAR.EntityLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace COROLA_RENTACAR.WebUI.Models
{
    public class TrackReservationViewModel
    {
        [Required(ErrorMessage = "Reservation code is required.")]
        public string ReservationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public bool HasSearched { get; set; }

        public string? ErrorMessage { get; set; }

        public Reservation? Reservation { get; set; }
    }
}