using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Books.Commands.CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Isbn).MaximumLength(32).When(x => !string.IsNullOrWhiteSpace(x.Isbn));
        }
    }
}
