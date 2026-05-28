using COROLA_RENTACAR.EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.BusinessLayer.ValidationRules
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Category Name Cannot Be Empty");
            RuleFor(x => x.CategoryName).MinimumLength(2).WithMessage("Please Min. 2 Character ").MaximumLength(20).WithMessage("Max. 20 Character");
        }
    }
}
