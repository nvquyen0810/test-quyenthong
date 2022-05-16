using FluentValidation;
using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Validators
{
    public class CustomerLoginValidator : AbstractValidator<CutomerLoginDTO>
    {
        public CustomerLoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
