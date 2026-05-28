using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class BrandValidator : AbstractValidator<Brand>
    {
        public BrandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Brand name cannot be empty.")
                .MinimumLength(2).WithMessage("Brand name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Brand name can be at most 50 characters.");

            RuleFor(x => x.LogoUrl)
                .MaximumLength(500).WithMessage("Logo URL can be at most 500 characters.")
                .Must(BeAValidImageUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl))
                .WithMessage("Logo URL must be a valid image address. Allowed formats: png, jpg, jpeg, svg, webp.");
        }

        private bool BeAValidImageUrl(string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl))
            {
                return true;
            }

            var lowerUrl = logoUrl.ToLower();

            return lowerUrl.EndsWith(".png")
                || lowerUrl.EndsWith(".jpg")
                || lowerUrl.EndsWith(".jpeg")
                || lowerUrl.EndsWith(".svg")
                || lowerUrl.EndsWith(".webp");
        }
    }
}