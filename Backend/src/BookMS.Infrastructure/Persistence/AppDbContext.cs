using BookMS.Application.Abstractions;
using BookMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Authors> Authors => Set<Authors>();
        public DbSet<Books> Books => Set<Books>();
        public DbSet<Categories> Categories => Set<Categories>();
        public DbSet<BookAuthors> BookAuthors => Set<BookAuthors>();
        public DbSet<BookCategories> BookCategories => Set<BookCategories>();
        public DbSet<Users> Users => Set<Users>();
        public DbSet<Roles> Roles => Set<Roles>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Books
            modelBuilder.Entity<Books>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.AuthorName).HasMaxLength(150);
                b.Property(x => x.Isbn).HasMaxLength(32);
            });

            // Authors
            modelBuilder.Entity<Authors>(a =>
            {
                a.HasKey(x => x.Id);
                a.Property(x => x.FullName).IsRequired().HasMaxLength(150);
                a.Property(x => x.Bio).HasMaxLength(2000);
            });

            // Categories 
            modelBuilder.Entity<Categories>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.Name).IsRequired().HasMaxLength(120);
                c.Property(x => x.Description).HasMaxLength(1000);
            });

            // Roles
            modelBuilder.Entity<Roles>(r =>
            {
                r.HasKey(x => x.Id);
                r.Property(x => x.Name).IsRequired().HasMaxLength(50);
            });

            // Users (1 Role : many Users) 
            modelBuilder.Entity<Users>(u =>
            {
                u.HasKey(x => x.Id);
                u.Property(x => x.Email).IsRequired().HasMaxLength(200);
                u.Property(x => x.Name).IsRequired().HasMaxLength(150);
                u.Property(x => x.ProviderUserId).IsRequired();

                u.HasOne(x => x.Roles)
                 .WithMany(r => r.Users)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //  BookAuthors (M : M via join entity)
            modelBuilder.Entity<BookAuthors>(ba =>
            {
                ba.HasKey(x => new { x.BookId, x.AuthorId });

                ba.HasOne(x => x.Books)
                  .WithMany(b => b.Authors)
                  .HasForeignKey(x => x.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

                ba.HasOne(x => x.Authors)
                  .WithMany(a => a.Books)
                  .HasForeignKey(x => x.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            //BookCategories (M : M via join entity
            modelBuilder.Entity<BookCategories>(bc =>
            {
                bc.HasKey(x => new { x.BookId, x.CategoryId });

                bc.HasOne(x => x.Books)
                  .WithMany(b => b.Categories)
                  .HasForeignKey(x => x.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

                bc.HasOne(x => x.Categories)
                  .WithMany(c => c.Books)
                  .HasForeignKey(x => x.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            // Refresh Token 
            modelBuilder.Entity<RefreshToken>(rt =>
            {
                rt.HasKey(x => x.Id);
                rt.HasIndex(x => x.Token).IsUnique();
                rt.Property(x => x.Token).IsRequired().HasMaxLength(512);
                rt.Property(x => x.ExpiresAt).IsRequired();

                rt.HasOne(x => x.User)
                  .WithMany()        
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
