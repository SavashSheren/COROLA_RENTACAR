using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
 

public class BrandValidator : AbstractValidator<Brand>
    {
        public BrandValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Brand name cannot be empty.")
                .Length(2, 40).WithMessage("Brand name must be between 2 and 40 characters.")
                .Matches(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$")
                .WithMessage("Brand name must contain letters only.")
                .NotEqual("Test").WithMessage("Invalid brand name.");

            RuleFor(x => x.BrandName)
                .Must(BeAValidBrandName)
                .WithMessage("Brand name cannot contain special characters.");

            RuleFor(x => x.LogoUrl)
                .NotEmpty().WithMessage("Logo URL cannot be empty.")
                .Must(BeAValidImageUrl)
                .WithMessage("Logo URL must be a valid image address.");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status must be specified.");

            RuleFor(x => x.BrandId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("BrandId cannot be negative.");

            When(x => x.Status == true, () =>
            {
                RuleFor(x => x.LogoUrl)
                    .NotEmpty().WithMessage("Logo is required for active brands.");
            });
        }

        private bool BeAValidBrandName(string brandName)
        {
            return !brandName.Contains("@") && !brandName.Contains("#");
        }

        private bool BeAValidImageUrl(string url)
        {
            return url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".jpeg");
        }
    }


}
