using COROLA_RENTACAR.WebUI.Models;

namespace COROLA_RENTACAR.WebUI.Services
{
    public interface IAiDriverLicenseVerificationService
    {
        Task<DriverLicenseAiVerificationResult> VerifyAsync(
            IFormFile driverLicenseImage,
            PublicReservationViewModel reservationModel);
    }
}