using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReadTogether.Domain.Contexts;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public abstract class BookshelfRepositoryTestsBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected DbContextOptions<BooksDbContext> DbOptions { get; }

        protected const string OwnerId = "owner";
        protected const string OtherUserId = "user";

        protected BookshelfRepositoryTestsBase()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            DbOptions = new DbContextOptionsBuilder<BooksDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = CreateContext();
            context.Database.EnsureCreated();
        }

        protected BooksDbContext CreateContext() => new(DbOptions);

        protected static ApplicationUser CreateUser(
            string id = OwnerId,
            string userName = "Jane",
            string email = "jane@example.test")
            => new() { Id = id, UserName = userName, Email = email };

        protected static Bookshelf CreateBookshelf(
            string userId = OwnerId,
            string name = "Fantasy",
            bool isDefaultShelf = false)
            => new() { UserId = userId, Name = name, IsDefaultShelf = isDefaultShelf };

        protected static Book CreateBook(
            string id = "book-1",
            string title = "Test book",
            string? coverImageUrl = "https://example.test/book.jpg")
            => new()
            {
                Id = id,
                Title = title,
                CoverImageUrl = coverImageUrl
            };

        protected static BookshelfBook CreateBookshelfBook(
            int bookshelfId,
            string bookId = "book-1")
            => new()
            {
                BookshelfId = bookshelfId,
                BookId = bookId
            };

        protected static BookMetadataDto CreateMetadata(
            string id = "book-1",
            string title = "Test book")
            => new()
            {
                Id = id,
                Title = title,
                AuthorName = "Test Author",
                CoverImageUrl = "https://example.test/book.jpg"
            };

        protected static async Task SeedAsync(
            BooksDbContext context,
            params object[] entities)
        {
            await context.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}