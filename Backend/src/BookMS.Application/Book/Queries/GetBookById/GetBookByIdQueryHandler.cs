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

namespace BookMS.Application.Book.Queries.GetBookById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
    {
        private readonly IAppDbContext _db;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandler(IAppDbContext db, IMapper mapper)
        { _db = db; _mapper = mapper; }

        public Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken ct)
        {
            return _db.Books
                .AsNoTracking()
                .Where(b => b.Id == request.Id)
                .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }
    }
}
