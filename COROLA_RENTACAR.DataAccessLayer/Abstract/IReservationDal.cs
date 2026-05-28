using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;

namespace COROLA_RENTACAR.DataAccessLayer.Abstract
{
    public interface IReservationDal : IGenericDal<Reservation>
    {
        Task<List<Reservation>> GetAllReservationsWithDetailsAsync();
        Task<Reservation> GetReservationWithDetailsByIdAsync(int id);
        Task UpdateReservationStatusAsync(int reservationId, ReservationStatus status);
    }
}