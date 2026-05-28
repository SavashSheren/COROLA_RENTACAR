using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty.")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("First name can be at most 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Last name can be at most 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(100).WithMessage("Email can be at most 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone cannot be empty.")
                .MaximumLength(30).WithMessage("Phone can be at most 30 characters.");

            RuleFor(x => x.DriverLicenseNumber)
                .NotEmpty().WithMessage("Driver license number cannot be empty.")
                .MaximumLength(50).WithMessage("Driver license number can be at most 50 characters.");

            RuleFor(x => x.DriverLicenseImageUrl)
                .NotEmpty().WithMessage("Driver license image URL cannot be empty.")
                .MaximumLength(700).WithMessage("Driver license image URL can be at most 700 characters.")
                .Must(BeAValidUrl).WithMessage("Driver license image URL must be a valid http or https address.");

            RuleFor(x => x.DriverLicenseVerificationStatus)
                .IsInEnum().WithMessage("Driver license verification status is invalid.");

            RuleFor(x => x.DriverLicenseRejectionReason)
                .MaximumLength(500).WithMessage("Rejection reason can be at most 500 characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date cannot be empty.");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}