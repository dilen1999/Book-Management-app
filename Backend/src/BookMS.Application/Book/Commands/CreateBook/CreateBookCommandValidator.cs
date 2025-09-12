using AutoMapper;
using BookMS.Application.Abstractions;
using BookMS.Application.DTOs;
using BookMS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly IAppDbContext _db;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(IAppDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken ct)
        {
            var book = new BookEntity
            {
                Title = request.Title,
                Isbn = request.Isbn,
                PublishedYear = request.PublishedYear
            };

            if (request.AuthorIds is { Count: > 0 })
            {
                var authors = await _db.Authors
                    .Where(a => request.AuthorIds.Contains(a.Id))
                    .ToListAsync(ct);

                foreach (var a in authors)
                    book.Authors.Add(new BookAuthors { BookId = book.Id, AuthorId = a.Id, Books = book, Authors = a });
            }

            if (request.CategoryIds is { Count: > 0 })
            {
                var cats = await _db.Categories
                    .Where(c => request.CategoryIds.Contains(c.Id))
                    .ToListAsync(ct);

                foreach (var c in cats)
                    book.Categories.Add(new BookCategories { BookId = book.Id, CategoryId = c.Id, Books = book, Categories = c });
            }

            await _db.Books.AddAsync(book, ct);
            await _db.SaveChangesAsync(ct);

            return _mapper.Map<BookDto>(book);
        }
    }
}
