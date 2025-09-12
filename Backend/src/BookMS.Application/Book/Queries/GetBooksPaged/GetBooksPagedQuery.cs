using BookMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Book.Queries.GetBooksPaged
{
    public record GetBooksPagedQuery(int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<BookDto>>;
}
