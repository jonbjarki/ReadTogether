using Microsoft.EntityFrameworkCore;
using ReadTogether.Infrastructure.Implementations;

namespace ReadTogether.Tests.Infrastructure.BookshelfRepositoryTests
{
    public class CreateBookshelfTests : BookshelfRepositoryTestsBase
    {
        [Fact]
        public async Task CreateBookshelf_PersistsBookshelfWithRequestedValues()
        {
            await using var context = CreateContext();
            var user = CreateUser();
            await SeedAsync(context, user);

            var result = await new BookshelfRepository(context)
                .CreateBookshelf("Want to Read", user.Id, CancellationToken.None, true);

            Assert.Equal("Want to Read", result.Name);
            Assert.Equal(user.Id, result.UserId);
            Assert.True(result.IsDefaultShelf);

            await using var verificationContext = CreateContext();
            var persistedBookshelf = await verificationContext.Bookshelves.FindAsync(result.Id);
            Assert.NotNull(persistedBookshelf);
            Assert.Equal("Want to Read", persistedBookshelf.Name);
            Assert.True(persistedBookshelf.IsDefaultShelf);
        }
    }
}