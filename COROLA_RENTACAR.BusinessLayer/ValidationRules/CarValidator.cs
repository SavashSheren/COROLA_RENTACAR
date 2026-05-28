using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class CarValidator : AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Please select a valid brand.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Please select a valid category.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model cannot be empty.")
                .MinimumLength(2).WithMessage("Model must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Model can be at most 50 characters.");

            RuleFor(x => x.ModelYear)
                .InclusiveBetween(1990, DateTime.Now.Year + 1)
                .WithMessage($"Model year must be between 1990 and {DateTime.Now.Year + 1}.");

            RuleFor(x => x.PlateNumber)
                .NotEmpty().WithMessage("Plate number cannot be empty.")
                .MinimumLength(5).WithMessage("Plate number must be at least 5 characters long.")
                .MaximumLength(20).WithMessage("Plate number can be at most 20 characters.");

            RuleFor(x => x.DailyPrice)
                .GreaterThan(0).WithMessage("Daily price must be greater than 0.");

            RuleFor(x => x.SeatCount)
                .InclusiveBetween(1, 12).WithMessage("Seat count must be between 1 and 12.");

            RuleFor(x => x.LuggageCapacity)
                .InclusiveBetween(0, 10).WithMessage("Luggage capacity must be between 0 and 10.");

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");

            RuleFor(x => x.FuelType)
                .NotEmpty().WithMessage("Fuel type cannot be empty.")
                .MaximumLength(30).WithMessage("Fuel type can be at most 30 characters.");

            RuleFor(x => x.TransmissionType)
                .NotEmpty().WithMessage("Transmission type cannot be empty.")
                .MaximumLength(30).WithMessage("Transmission type can be at most 30 characters.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL cannot be empty.")
                .MaximumLength(500).WithMessage("Image URL can be at most 500 characters.");
        }
    }
}