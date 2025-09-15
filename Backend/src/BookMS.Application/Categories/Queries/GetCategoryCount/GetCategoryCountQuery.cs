using MediatR;

namespace BookMS.Application.Categories.Queries.GetCategoryCount
{
    public record GetCategoryCountQuery() : IRequest<int>;
}
