using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.CarId)
                .GreaterThan(0).WithMessage("Please select a valid car.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Please select a valid customer.");

            RuleFor(x => x.PickupDate)
                .NotEmpty().WithMessage("Pickup date cannot be empty.");

            RuleFor(x => x.ReturnDate)
                .NotEmpty().WithMessage("Return date cannot be empty.")
                .Must((reservation, returnDate) => returnDate.Date >= reservation.PickupDate.Date)
                .WithMessage("Return date cannot be earlier than pickup date.");

            RuleFor(x => x.PickupLocationId)
                .GreaterThan(0).WithMessage("Please select a valid pickup location.");

            RuleFor(x => x.ReturnLocationId)
                .GreaterThan(0).WithMessage("Please select a valid return location.");

            RuleFor(x => x.ReservationStatus)
                .IsInEnum().WithMessage("Please select a valid reservation status.");

            RuleFor(x => x.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Total price cannot be negative.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description can be at most 500 characters.");
        }
    }
}