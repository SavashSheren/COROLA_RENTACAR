using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.WebUI.Services
{
    public interface IEmailNotificationService
    {
        Task SendReservationApprovedEmailAsync(Reservation reservation);
    }
}