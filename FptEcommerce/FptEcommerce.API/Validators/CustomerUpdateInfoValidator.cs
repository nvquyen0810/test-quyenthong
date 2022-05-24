using FluentValidation;
using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Validators
{
    public class CustomerUpdateInfoValidator : AbstractValidator<CustomerInfoUpdateDTO>
    {
        public CustomerUpdateInfoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Fullname is required");
        }
    }
}
