using BookMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<BookEntity> Books { get; }
        DbSet<AuthorEntity> Authors { get; }
        DbSet<CategoryEntity> Categories { get; }
        DbSet<BookAuthorEntity> BookAuthors { get; }
        DbSet<BookCategoryEntity> BookCategories { get; }
        DbSet<UserEntity> Users { get; }
        DbSet<RoleEntity> Roles { get; }
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
