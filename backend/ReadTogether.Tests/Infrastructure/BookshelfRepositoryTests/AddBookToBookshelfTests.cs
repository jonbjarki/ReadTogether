using Microsoft.EntityFrameworkCore;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class AddBookToBookshelfTests : BookshelfRepositoryTestsBase
    {
        [Fact]
        public async Task AddBookToBookshelf_PersistsJoinEntry_WhenBookIsNotAlreadyInShelf()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            var book = CreateBook("volume-1", title: "Test Book");
            await SeedAsync(context, user, bookshelf, book);

            var result = await new BookshelfRepository(context)
                .AddBookToBookshelf(bookshelf.Id, book.Id, CancellationToken.None);

            Assert.Equal(bookshelf.Id, result.BookshelfId);
            Assert.Equal(book.Id, result.BookId);
            Assert.NotEqual(default, result.AddedAt);

            await using var verificationContext = CreateContext();
            var persistedEntry = await verificationContext.BookshelfBooks.FindAsync(bookshelf.Id, book.Id);
            Assert.NotNull(persistedEntry);
        }

        [Fact]
        public async Task AddBookToBookshelf_ReferencesExistingBook_WhenAddingSameBookToAnotherShelf()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var shelfOne = CreateBookshelf(name: "Fantasy");
            var shelfTwo = CreateBookshelf(name: "Sci-fi");
            var book = CreateBook("volume-1", title: "Original Title");
            await SeedAsync(context, user, shelfOne, shelfTwo, book);
            await SeedAsync(context, CreateBookshelfBook(shelfOne.Id, "volume-1"));

            var result = await new BookshelfRepository(context)
                .AddBookToBookshelf(shelfTwo.Id, book.Id, CancellationToken.None);

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
                .AddBookToBookshelf(bookshelf.Id, "volume-1", CancellationToken.None));
        }

        [Fact]
        public async Task AddBookToBookshelf_ThrowsNotFound_WhenBookshelfDoesNotExist()
        {
            await using var context = CreateContext();
            var book = CreateBook("volume-1");
            await SeedAsync(context, book);

            await Assert.ThrowsAsync<NotFoundException>(() => new BookshelfRepository(context)
                .AddBookToBookshelf(999, book.Id, CancellationToken.None));
        }

        [Fact]
        public async Task AddBookToBookshelf_ThrowsNotFound_WhenBookDoesNotExist()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            var bookshelf = CreateBookshelf();
            await SeedAsync(context, user, bookshelf);

            await Assert.ThrowsAsync<NotFoundException>(() => new BookshelfRepository(context)
                .AddBookToBookshelf(bookshelf.Id, "volume", CancellationToken.None));
        }
    }
}