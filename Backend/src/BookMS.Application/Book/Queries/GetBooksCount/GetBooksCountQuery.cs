using MediatR;

namespace BookMS.Application.Books.Queries.GetBooksCount
{
    public record GetBooksCountQuery() : IRequest<int>;
}
