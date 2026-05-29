using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class ContactMessageValidator : AbstractValidator<ContactMessage>
    {
        public ContactMessageValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("Phone cannot exceed 30 characters.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(150).WithMessage("Subject cannot exceed 150 characters.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");
        }
    }
}