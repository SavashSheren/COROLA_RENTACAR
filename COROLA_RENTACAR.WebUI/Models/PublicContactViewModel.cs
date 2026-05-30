using System.ComponentModel.DataAnnotations;

namespace COROLA_RENTACAR.WebUI.Models
{
    public class PublicContactViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30, ErrorMessage = "Phone cannot exceed 30 characters.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(150, ErrorMessage = "Subject cannot exceed 150 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}