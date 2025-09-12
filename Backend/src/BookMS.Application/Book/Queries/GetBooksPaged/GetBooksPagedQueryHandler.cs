using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookMS.Application.Abstractions;
using BookMS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Book.Queries.GetBooksPaged
{
    public class GetBooksPagedQueryHandler : IRequestHandler<GetBooksPagedQuery, IReadOnlyList<BookDto>>
    {
        private readonly IAppDbContext _db;
        private readonly IMapper _mapper;
        public GetBooksPagedQueryHandler(IAppDbContext db, IMapper mapper)
        { _db = db; _mapper = mapper; }

        public async Task<IReadOnlyList<BookDto>> Handle(GetBooksPagedQuery request, CancellationToken ct)
        {
            var skip = (request.Page - 1) * request.PageSize;

            var items = await _db.Books.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip).Take(request.PageSize)
                .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return items;
        }

    }
}
