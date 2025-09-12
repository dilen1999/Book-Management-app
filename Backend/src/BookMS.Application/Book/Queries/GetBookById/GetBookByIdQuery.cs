using BookMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Book.Queries.GetBookById
{
    public record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;
}
