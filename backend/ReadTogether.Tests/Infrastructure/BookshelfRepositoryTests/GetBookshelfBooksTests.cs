using Microsoft.IdentityModel.Tokens.Experimental;
using ReadTogether.Infrastructure.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class GetBookshelfBooksTests : BookshelfRepositoryTestsBase
    {
        [Fact]
        public async Task GetBookshelfBooks_ReturnsBook()
        {
            // Arrange
            var bookshelf = CreateBookshelf();
            var user = CreateUser();
            var context = CreateContext();
            await SeedAsync(context, user, bookshelf);

            var book = CreateBookshelfBook(bookshelf.Id, "Book 1");
            await SeedAsync(context, book);

            // Act
            var books = await new BookshelfRepository(context).GetBookshelfBooks(bookshelf.Id, 1, 10, CancellationToken.None);

            // Assert
            Assert.NotNull(books);
            Assert.Single(books.Results);
            Assert.Equal(books.Results.Single().Id, book.VolumeId);
            Assert.Equal(1, books.NumPages);
        }

        [Fact]
        public async Task GetBookshelfBooks_ReturnsMultipleBooks()
        {
            // Arrange
            var bookshelf = CreateBookshelf();
            var user = CreateUser();
            var context = CreateContext();
            await SeedAsync(context, user, bookshelf);

            for (int i = 1; i <= 5; i++)
            {
                var book = CreateBookshelfBook(bookshelf.Id, $"Book {i}");
                await SeedAsync(context, book);
            }

            // Act
            var books = await new BookshelfRepository(context).GetBookshelfBooks(bookshelf.Id, 1, 10, CancellationToken.None);

            // Assert
            Assert.NotNull(books);
            Assert.Equal(5, books.Results.Count());
            Assert.Equal(1, books.NumPages);
        }

        [Theory]
        [InlineData(1, 10, 2)]
        [InlineData(2, 10, 2)]
        [InlineData(3, 5, 3)]
        public async Task GetBookshelfBooks_ReturnsCorrectPaging(int pageNumber, int pageSize, int expectedNumPages)
        {
            // Arrange
            var bookshelf = CreateBookshelf();
            var user = CreateUser();
            var context = CreateContext();
            await SeedAsync(context, user, bookshelf);
            for (int i = 1; i <= 15; i++)
            {
                var book = CreateBookshelfBook(bookshelf.Id, $"Book {i}");
                await SeedAsync(context, book);
            }

            // Act
            var books = await new BookshelfRepository(context).GetBookshelfBooks(bookshelf.Id, pageNumber, pageSize, CancellationToken.None);

            // Assert
            Assert.NotNull(books);
            Assert.NotEmpty(books.Results);
            Assert.Equal(expectedNumPages, books.NumPages);
            Assert.Equal(pageNumber, books.Page);
            Assert.Equal(pageSize, books.PageSize);
        }

    }
}
