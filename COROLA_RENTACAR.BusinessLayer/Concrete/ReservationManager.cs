using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.EntityLayer.Enums;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.Concrete
{
    public class ReservationManager : IReservationService
    {
        private readonly IReservationDal _reservationDal;
        private readonly ICarDal _carDal;
        private readonly ICustomerDal _customerDal;
        private readonly IValidator<Reservation> _validator;

        public ReservationManager(
            IReservationDal reservationDal,
            ICarDal carDal,
            ICustomerDal customerDal,
            IValidator<Reservation> validator)
        {
            _reservationDal = reservationDal;
            _carDal = carDal;
            _customerDal = customerDal;
            _validator = validator;
        }

        public async Task<List<Reservation>> TGetAllAsync()
        {
            return await _reservationDal.GetAllAsync();
        }

        public async Task<List<Reservation>> TGetAllReservationsWithDetailsAsync()
        {
            return await _reservationDal.GetAllReservationsWithDetailsAsync();
        }

        public async Task<Reservation> TGetReservationWithDetailsByIdAsync(int id)
        {
            return await _reservationDal.GetReservationWithDetailsByIdAsync(id);
        }

        public async Task<Reservation> TGetByIdAsync(int id)
        {
            return await _reservationDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Reservation entity)
        {
            entity.Description ??= string.Empty;

            await EnsureCustomerLicenseApprovedAsync(entity.CustomerId);
            await CalculateTotalPriceAsync(entity);

            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _reservationDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Reservation entity)
        {
            entity.Description ??= string.Empty;

            await EnsureCustomerLicenseApprovedAsync(entity.CustomerId);
            await CalculateTotalPriceAsync(entity);

            var result = await _validator.ValidateAsync(entity);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await _reservationDal.UpdateAsync(entity);
        }

        public async Task TDeleteAsync(int id)
        {
            await _reservationDal.DeleteAsync(id);
        }

        public async Task TApproveReservationAsync(int reservationId)
        {
            await _reservationDal.UpdateReservationStatusAsync(reservationId, ReservationStatus.Approved);
        }

        public async Task TRejectReservationAsync(int reservationId)
        {
            await _reservationDal.UpdateReservationStatusAsync(reservationId, ReservationStatus.Rejected);
        }

        public async Task TCancelReservationAsync(int reservationId)
        {
            await _reservationDal.UpdateReservationStatusAsync(reservationId, ReservationStatus.Cancelled);
        }

        private async Task EnsureCustomerLicenseApprovedAsync(int customerId)
        {
            var customer = await _customerDal.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new ValidationException("Customer could not be found.");
            }

            if (customer.DriverLicenseVerificationStatus != DriverLicenseVerificationStatus.Approved)
            {
                throw new ValidationException("Reservation cannot be created because the customer's driver license has not been approved.");
            }
        }

        private async Task CalculateTotalPriceAsync(Reservation reservation)
        {
            var car = await _carDal.GetByIdAsync(reservation.CarId);

            if (car == null)
            {
                reservation.TotalPrice = 0;
                return;
            }

            var totalDays = (reservation.ReturnDate.Date - reservation.PickupDate.Date).Days;

            if (totalDays < 1)
            {
                totalDays = 1;
            }

            reservation.TotalPrice = car.DailyPrice * totalDays;
        }
    }
}