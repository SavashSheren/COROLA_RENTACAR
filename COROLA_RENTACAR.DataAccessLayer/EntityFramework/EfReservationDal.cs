using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.Repository;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace COROLA_RENTACAR.DataAccessLayer.EntityFramework
{
    public class EfReservationDal : GenericRepository<Reservation>, IReservationDal
    {
        private readonly CorolaContext _context;

        public EfReservationDal(CorolaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetAllReservationsWithDetailsAsync()
        {
            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(x => x.Brand)
                .Include(x => x.Car)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Customer)
                .Include(x => x.PickupLocation)
                .Include(x => x.ReturnLocation)
                .OrderByDescending(x => x.ReservationId)
                .ToListAsync();
        }

        public async Task<Reservation> GetReservationWithDetailsByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(x => x.Brand)
                .Include(x => x.Car)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Customer)
                .Include(x => x.PickupLocation)
                .Include(x => x.ReturnLocation)
                .FirstOrDefaultAsync(x => x.ReservationId == id);
        }

        public async Task<Reservation?> GetReservationByCodeAndEmailAsync(string reservationCode, string email)
        {
            var normalizedCode = reservationCode.Trim().ToUpper();
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Reservations
                .Include(x => x.Car)
                    .ThenInclude(x => x.Brand)
                .Include(x => x.Car)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Customer)
                .Include(x => x.PickupLocation)
                .Include(x => x.ReturnLocation)
                .FirstOrDefaultAsync(x =>
                    x.ReservationCode != null &&
                    x.ReservationCode.ToUpper() == normalizedCode &&
                    x.Customer.Email.ToLower() == normalizedEmail);
        }

        public async Task<bool> ReservationCodeExistsAsync(string reservationCode)
        {
            var normalizedCode = reservationCode.Trim().ToUpper();

            return await _context.Reservations
                .AnyAsync(x =>
                    x.ReservationCode != null &&
                    x.ReservationCode.ToUpper() == normalizedCode);
        }

        public async Task UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if (reservation == null)
            {
                return;
            }

            reservation.ReservationStatus = status;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasReservationConflictAsync(
            int carId,
            DateTime pickupDate,
            DateTime returnDate,
            int? ignoredReservationId = null,
            bool includePending = true)
        {
            var pickup = pickupDate.Date;
            var returnDay = returnDate.Date;

            var query = _context.Reservations
                .Where(x => x.CarId == carId);

            if (includePending)
            {
                query = query.Where(x =>
                    x.ReservationStatus == ReservationStatus.Pending ||
                    x.ReservationStatus == ReservationStatus.Approved);
            }
            else
            {
                query = query.Where(x =>
                    x.ReservationStatus == ReservationStatus.Approved);
            }

            if (ignoredReservationId.HasValue)
            {
                query = query.Where(x => x.ReservationId != ignoredReservationId.Value);
            }

            return await query.AnyAsync(x =>
                pickup <= x.ReturnDate.Date &&
                returnDay >= x.PickupDate.Date);
        }
    }
}