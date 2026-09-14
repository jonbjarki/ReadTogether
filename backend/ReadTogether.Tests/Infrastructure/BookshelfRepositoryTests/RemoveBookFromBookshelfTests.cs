using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class RemoveBookFromBookshelfTests : BookshelfRepositoryTestsBase
    {
        [Fact]
        public async Task RemoveBookFromBookshelf_ReturnsTrueAndDeletesBook_WhenShelfIsOwnedByUser()
        {
            // Arrange
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, user, bookshelf);
            var book = CreateBook();
            var entry = CreateBookshelfBook(bookshelf.Id);
            await SeedAsync(context, book, entry);

            // Act
            var result = await new BookshelfRepository(context)
                .RemoveBooksFromBookshelf(bookshelf.Id, [entry.BookId], user.Id, CancellationToken.None);

            // Assert
            Assert.True(result);

            await using var verificationContext = CreateContext();
            Assert.Null(await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, entry.BookId));
        }

        [Fact]
        public async Task RemoveBookFromBookshelf_ReturnsTrueAndDeletesBooks_WhenMultipleBooksProvided()
        {
            // Arrange
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, user, bookshelf);
            var book1 = CreateBook();
            var book2 = CreateBook("book-2");
            var book3 = CreateBook("book-3");
            var entry1 = CreateBookshelfBook(bookshelf.Id, book1.Id);
            var entry2 = CreateBookshelfBook(bookshelf.Id, book2.Id);
            var entry3 = CreateBookshelfBook(bookshelf.Id, book3.Id);

            await SeedAsync(context, book1, book2, book3, entry1, entry2, entry3);

            // Act
            var result = await new BookshelfRepository(context)
                .RemoveBooksFromBookshelf(bookshelf.Id, [book1.Id, book2.Id, book3.Id], user.Id, CancellationToken.None);

            // Assert
            Assert.True(result);

            await using var verificationContext = CreateContext();
            Assert.Null(await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, book1.Id));
            Assert.Null(await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, book2.Id));
            Assert.Null(await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, book3.Id));

        }

        [Fact]
        public async Task RemoveBookFromBookshelf_ThrowsAccessDenied_WhenShelfIsNotOwnedByUser()
        {
            // Arrange
            await using var context = CreateContext();
            var owner = CreateUser();
            var otherUser = CreateUser(OtherUserId, "John", "john@example.test");
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, owner, otherUser, bookshelf);
            var book = CreateBook();
            var entry = CreateBookshelfBook(bookshelf.Id);
            await SeedAsync(context, book, entry);

            // Act & Assert
            await Assert.ThrowsAsync<AccessDeniedException>(() => new BookshelfRepository(context)
                .RemoveBooksFromBookshelf(bookshelf.Id, [entry.BookId], otherUser.Id, CancellationToken.None));

            await using var verificationContext = CreateContext();
            Assert.NotNull(await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, entry.BookId));
        }

        [Fact]
        public async Task RemoveBookFromBookshelf_ThrowsNotFound_WhenShelfDoesNotExist()
        {
            // Arrange
            await using var context = CreateContext();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => new BookshelfRepository(context)
                .RemoveBooksFromBookshelf(0, ["non-existent-book"], "non-existent-user", CancellationToken.None));

        }
    }
}