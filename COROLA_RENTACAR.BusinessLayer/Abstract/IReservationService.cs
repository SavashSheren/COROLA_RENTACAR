using COROLA_RENTACAR.EntityLayer.Entities;

namespace COROLA_RENTACAR.BusinessLayer.Abstract
{
    public interface IReservationService : IGenericService<Reservation>
    {
        Task<List<Reservation>> TGetAllReservationsWithDetailsAsync();
        Task<Reservation> TGetReservationWithDetailsByIdAsync(int id);

        Task<bool> THasReservationConflictAsync(
            int carId,
            DateTime pickupDate,
            DateTime returnDate,
            int? ignoredReservationId = null,
            bool includePending = true);

        Task TCreatePublicReservationRequestAsync(Reservation reservation);

        Task TApproveReservationAsync(int reservationId);
        Task TRejectReservationAsync(int reservationId);
        Task TCancelReservationAsync(int reservationId);
    }
}