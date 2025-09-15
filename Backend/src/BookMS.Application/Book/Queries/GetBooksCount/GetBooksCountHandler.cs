using BookMS.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookMS.Application.Books.Queries.GetBooksCount
{
    public class GetBooksCountHandler : IRequestHandler<GetBooksCountQuery, int>
    {
        private readonly IAppDbContext _db;

        public GetBooksCountHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(GetBooksCountQuery request, CancellationToken ct)
        {
            return await _db.Books.CountAsync(cancellationToken: ct);
        }
    }
}
