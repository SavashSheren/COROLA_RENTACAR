using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.DataAccessLayer.Abstract
{
    public interface IReservationDal : IGenericDal<Reservation>
    {
        Task<List<Reservation>> GetAllReservationsWithDetailsAsync();

        Task<Reservation> GetReservationWithDetailsByIdAsync(int id);

        Task<Reservation?> GetReservationByCodeAndEmailAsync(string reservationCode, string email);

        Task<bool> ReservationCodeExistsAsync(string reservationCode);

        Task UpdateReservationStatusAsync(int reservationId, ReservationStatus status);

        Task<bool> HasReservationConflictAsync(
            int carId,
            DateTime pickupDate,
            DateTime returnDate,
            int? ignoredReservationId = null,
            bool includePending = true);
    }
}