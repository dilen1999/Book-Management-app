using BookMS.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookMS.Application.Categories.Queries.GetCategoryCount
{
    public class GetCategoryCountHandler : IRequestHandler<GetCategoryCountQuery, int>
    {
        private readonly IAppDbContext _db;

        public GetCategoryCountHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(GetCategoryCountQuery request, CancellationToken cancellationToken)
        {
            return await _db.Categories.CountAsync(cancellationToken);
        }
    }
}
