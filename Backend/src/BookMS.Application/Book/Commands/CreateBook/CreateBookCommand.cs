using BookMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Books.Commands.CreateBook
{
    public record CreateBookCommand(
        string Title,
        string? Isbn,
        int? PublishedYear,
        List<Guid>? AuthorIds,
        List<Guid>? CategoryIds
    ) : IRequest<BookDto>;
}
