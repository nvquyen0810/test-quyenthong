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
                //.Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is required");
            //.Must(IsValidName).WithMessage("{PropertyName} should be all letters.");
            //.Length(2, 255).WithMessage("{PropertyName} length < 2");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("{PropertyName} is required");
        }

        private bool IsValidName(string name)
        {
            return name.All(Char.IsLetter);
        }
    }
}
