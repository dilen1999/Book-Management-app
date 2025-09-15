using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Commands.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Request.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Request.Name)
                .NotEmpty();

            RuleFor(x => x.Request.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Request.Role)
                .NotEmpty()
                .Must(role => role == "User" || role == "Admin" || role == "SuperAdmin")
                .WithMessage("Role must be User, Admin, or SuperAdmin.");
        }
    }
}
