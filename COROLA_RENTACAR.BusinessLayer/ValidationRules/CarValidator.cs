using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class CarValidator :AbstractValidator<Car>
    {
        public CarValidator()
        {
            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand field cannot be empty.")
                .MinimumLength(2).WithMessage("Brand must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Brand can be at most 50 characters.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model field cannot be empty.")
                .MinimumLength(2).WithMessage("Model must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Model can be at most 50 characters.");

            RuleFor(x => x.ModelYear)
                .NotEmpty().WithMessage("Model year cannot be empty.")
                .InclusiveBetween(1990, DateTime.Now.Year)
                .WithMessage($"Model year must be between 1990 and {DateTime.Now.Year}.");

            RuleFor(x => x.PlateNumber)
                .NotEmpty().WithMessage("Plate number cannot be empty.")
                .MinimumLength(5).WithMessage("Plate number must be at least 5 characters long.")
                .MaximumLength(15).WithMessage("Plate number can be at most 15 characters.");

            RuleFor(x => x.DailyPrice)
                .NotEmpty().WithMessage("Daily price cannot be empty.")
                .GreaterThan(0).WithMessage("Daily price must be greater than 0.");

            RuleFor(x => x.SeatCount)
                .NotEmpty().WithMessage("Seat count cannot be empty.")
                .InclusiveBetween(1, 12).WithMessage("Seat count must be between 1 and 12.");

            RuleFor(x => x.LuggageCapacity)
                .NotEmpty().WithMessage("Luggage capacity cannot be empty.")
                .InclusiveBetween(0, 10).WithMessage("Luggage capacity must be between 0 and 10.");

            RuleFor(x => x.Mileage)
                .NotEmpty().WithMessage("Mileage cannot be empty.")
                .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");

            RuleFor(x => x.FuelType)
                .NotEmpty().WithMessage("Fuel type cannot be empty.")
                .MaximumLength(30).WithMessage("Fuel type can be at most 30 characters.");

            RuleFor(x => x.TransmissionType)
                .NotEmpty().WithMessage("Transmission type cannot be empty.")
                .MaximumLength(30).WithMessage("Transmission type can be at most 30 characters.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category must be selected.")
                .GreaterThan(0).WithMessage("Please select a valid category.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Car image cannot be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Please enter a valid image URL.");
        }

    }
}
