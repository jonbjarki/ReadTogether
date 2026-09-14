using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class GetBookshelfTests : BookshelfRepositoryTestsBase
    {

        [Fact]
        public async Task GetBookshelfById_ReturnsNull_WhenBookshelfDoesNotExist()
        {
            await using var context = CreateContext();

            var result = await new BookshelfRepository(context)
                .GetBookshelfById(999, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookshelvesByUserId_ReturnsOnlyUsersBookshelvesInDescendingCreationOrder()
        {
            await using var context = CreateContext();
            var owner = CreateUser();
            var otherUser = CreateUser(OtherUserId, "John", "john@example.test");
            var olderShelf = CreateBookshelf(name: "Older");
            olderShelf.CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var newerShelf = CreateBookshelf(name: "Newer");
            newerShelf.CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            var otherUsersShelf = CreateBookshelf(OtherUserId, "Other user's shelf");
            await SeedAsync(context, owner, otherUser, olderShelf, newerShelf, otherUsersShelf);
            var book = CreateBook();
            var entry = CreateBookshelfBook(newerShelf.Id);
            await SeedAsync(context, book, entry);

            var result = await new BookshelfRepository(context)
                .GetBookshelvesByUserId(owner.Id, CancellationToken.None);

            Assert.Collection(result,
                bookshelf => Assert.Equal("Newer", bookshelf.Name),
                bookshelf => Assert.Equal("Older", bookshelf.Name));
        }

        [Fact]
        public async Task GetBookshelvesByUserId_ReturnsEmptyList_WhenUserHasNoBookshelves()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            await SeedAsync(context, user);

            var result = await new BookshelfRepository(context)
                .GetBookshelvesByUserId(user.Id, CancellationToken.None);

            Assert.Empty(result);
        }
    }
}