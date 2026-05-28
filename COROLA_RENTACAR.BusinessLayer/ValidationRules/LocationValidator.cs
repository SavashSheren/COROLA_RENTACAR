using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class LocationValidator : AbstractValidator<Location>
    {
        public LocationValidator()
        {
            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("Location name cannot be empty.")
                .MinimumLength(2).WithMessage("Location name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Location name can be at most 100 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City cannot be empty.")
                .MinimumLength(2).WithMessage("City must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("City can be at most 50 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address cannot be empty.")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
                .MaximumLength(250).WithMessage("Address can be at most 250 characters.");

            RuleFor(x => x.AuthorizedPerson)
                .NotEmpty().WithMessage("Authorized person cannot be empty.")
                .MinimumLength(2).WithMessage("Authorized person must be at least 2 characters long.")
                .MaximumLength(80).WithMessage("Authorized person can be at most 80 characters.");
        }
    }
}