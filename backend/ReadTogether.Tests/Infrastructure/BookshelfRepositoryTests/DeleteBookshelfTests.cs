using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadTogether.Domain.Contexts;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Implementations;
using ReadTogether.Infrastructure.Interfaces;
using Xunit;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class DeleteBookshelfTests : BookshelfRepositoryTestsBase
    {
        [Fact]
        public async Task DeleteBookshelf_ReturnsTrue_OnSuccess()
        {
            // Arrange
            await using var context = CreateContext();
            var bookshelf = CreateBookshelf();
            var user = CreateUser();
            await SeedAsync(context, bookshelf, user);


            // Act
            var result = await new BookshelfRepository(context).DeleteBookshelf(bookshelf.Id, user.Id, CancellationToken.None);


            // Assert
            Assert.True(result);
            Assert.Null(await context.Bookshelves.FindAsync(bookshelf.Id));
        }

        [Fact]
        public async Task DeleteBookshelf_ThrowsAccessDeniedException_WhenDefaultShelf()
        {
            // Arrange
            await using var context = CreateContext();
            var bookshelf = CreateBookshelf(isDefaultShelf: true);
            var user = CreateUser();
            await SeedAsync(context, bookshelf, user);

            // Act & Assert
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await new BookshelfRepository(context).DeleteBookshelf(bookshelf.Id, user.Id, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteBookshelf_ThrowsAccessDeniedException_WhenBookshelfNotOwned()
        {
            // Arrange
            await using var context = CreateContext();
            var bookshelf = CreateBookshelf(isDefaultShelf: true);
            var user = CreateUser();
            await SeedAsync(context, bookshelf, user);
            var respository = new BookshelfRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await respository.DeleteBookshelf(bookshelf.Id, "not-the-owner-id", CancellationToken.None));
        }

        [Fact]
        public async Task DeleteBookshelf_ThrowsNotFound_WhenNotFound()
        {
            // Arrange
            await using var context = CreateContext();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await new BookshelfRepository(context).DeleteBookshelf(0, "abc", CancellationToken.None));
        }
    }
}