using Microsoft.EntityFrameworkCore;
using ReadTogether.Domain.DTOs;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class AddBookToBookshelfTests : BookshelfRepositoryTestsBase
    {
        private static BookMetadataDto Metadata(
            string id = "volume-1",
            string title = "Test Book",
            string? coverImageUrl = "https://example.test/book.jpg")
            => new()
            {
                Id = id,
                Title = title,
                AuthorName = "Test Author",
                CoverImageUrl = coverImageUrl
            };

        [Fact]
        public async Task AddBookToBookshelf_PersistsBook_WhenBookIsNotAlreadyInShelf()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, user, bookshelf);

            var result = await new BookshelfRepository(context)
                .AddBookToBookshelf(bookshelf.Id, Metadata(), CancellationToken.None);

            Assert.Equal(bookshelf.Id, result.BookshelfId);
            Assert.Equal("volume-1", result.BookId);
            Assert.NotEqual(default, result.AddedAt);

            await using var verificationContext = CreateContext();
            var persistedEntry = await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, "volume-1");
            Assert.NotNull(persistedEntry);
            var persistedBook = await verificationContext.Books.FindAsync("volume-1");
            Assert.NotNull(persistedBook);
            Assert.Equal("Test Book", persistedBook.Title);
            Assert.Equal("Test Author", persistedBook.AuthorName);
        }

        [Fact]
        public async Task AddBookToBookshelf_ReusesExistingBook_WhenAddingSameBookToAnotherShelf()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var shelfOne = CreateBookshelf(name: "Fantasy");
            var shelfTwo = CreateBookshelf(name: "Sci-fi");
            var book = CreateBook("volume-1", title: "Original Title");
            await SeedAsync(context, user, shelfOne, shelfTwo, book);
            await SeedAsync(context, CreateBookshelfBook(shelfOne.Id, "volume-1"));

            var result = await new BookshelfRepository(context)
                .AddBookToBookshelf(shelfTwo.Id, Metadata(title: "Conflicting Title"), CancellationToken.None);

            Assert.Equal(shelfTwo.Id, result.BookshelfId);

            await using var verificationContext = CreateContext();
            var storedBook = await verificationContext.Books.FindAsync("volume-1");
            Assert.NotNull(storedBook);
            // First writer wins: the existing shared copy is not overwritten.
            Assert.Equal("Original Title", storedBook.Title);
            var bookCount = await verificationContext.Books.CountAsync();
            Assert.Equal(1, bookCount);
            Assert.NotNull(await verificationContext.BookshelfBooks.FindAsync(shelfTwo.Id, "volume-1"));
        }

        [Fact]
        public async Task AddBookToBookshelf_ThrowsConflict_WhenBookAlreadyExistsInShelf()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, user, bookshelf, CreateBook("volume-1"));
            await SeedAsync(context, CreateBookshelfBook(bookshelf.Id, "volume-1"));

            await Assert.ThrowsAsync<BookshelfBookConflictException>(() => new BookshelfRepository(context)
                .AddBookToBookshelf(bookshelf.Id, Metadata(), CancellationToken.None));
        }

        [Fact]
        public async Task AddBookToBookshelf_ThrowsConflict_WhenDatabaseRejectsInvalidBookshelfId()
        {
            await using var context = CreateContext();

            await Assert.ThrowsAsync<BookshelfBookConflictException>(() => new BookshelfRepository(context)
                .AddBookToBookshelf(999, Metadata(), CancellationToken.None));
        }
    }
}