using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class CarImageValidator : AbstractValidator<CarImage>
    {
        public CarImageValidator()
        {
            RuleFor(x => x.CarId)
                .GreaterThan(0).WithMessage("Please select a valid car.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL cannot be empty.")
                .MaximumLength(700).WithMessage("Image URL can be at most 700 characters.")
                .Must(BeAValidUrl).WithMessage("Image URL must be a valid http or https address.");
        }

        private bool BeAValidUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return false;
            }

            return Uri.TryCreate(imageUrl, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}