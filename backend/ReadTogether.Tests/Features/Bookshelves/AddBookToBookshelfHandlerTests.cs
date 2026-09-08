using Moq;
using ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Tests.Features.Bookshelves
{
    public class AddBookToBookshelfHandlerTests
    {
        private const string UserId = "user-1";
        private const int BookshelfId = 12;
        private const string BookId = "vol-42";

        private static readonly BookMetadataDto Metadata = new()
        {
            Id = BookId,
            Title = "Sample Book",
            AuthorName = "Sample Author",
            CoverImageUrl = "https://example.com/thumb.jpg"
        };

        [Fact]
        public async Task Handle_AddsBook_WhenBookshelfExistsAndOwnedByUser()
        {
            // Arrange
            var repositoryMock = new Mock<IBookshelfRepository>();
            repositoryMock
                .Setup(r => r.GetBookshelfById(BookshelfId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Bookshelf { Id = BookshelfId, Name = "Want to Read", UserId = UserId });
            repositoryMock
                .Setup(r => r.AddBookToBookshelf(BookshelfId, It.Is<BookMetadataDto>(m => m.Id == BookId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookshelfBook
                {
                    BookshelfId = BookshelfId,
                    BookId = BookId
                });

            var handler = new AddBookToBookshelfHandler(repositoryMock.Object);
            var command = new AddBookToBookshelfCommand(BookshelfId, BookId, Metadata, UserId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(BookshelfId, result.BookshelfId);
            Assert.Equal(BookId, result.Id);
            repositoryMock.Verify(r => r.AddBookToBookshelf(BookshelfId, It.Is<BookMetadataDto>(m => m.Id == BookId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsNotFound_WhenBookshelfDoesNotExist()
        {
            // Arrange
            var repositoryMock = new Mock<IBookshelfRepository>();
            repositoryMock
                .Setup(r => r.GetBookshelfById(BookshelfId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Bookshelf?)null);

            var handler = new AddBookToBookshelfHandler(repositoryMock.Object);
            var command = new AddBookToBookshelfCommand(BookshelfId, BookId, Metadata, UserId);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
            repositoryMock.Verify(r => r.AddBookToBookshelf(It.IsAny<int>(), It.IsAny<BookMetadataDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ThrowsNotFound_WhenBookshelfBelongsToAnotherUser()
        {
            // Arrange
            var repositoryMock = new Mock<IBookshelfRepository>();
            repositoryMock
                .Setup(r => r.GetBookshelfById(BookshelfId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Bookshelf { Id = BookshelfId, Name = "Read", UserId = "another-user" });

            var handler = new AddBookToBookshelfHandler(repositoryMock.Object);
            var command = new AddBookToBookshelfCommand(BookshelfId, BookId, Metadata, UserId);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
            repositoryMock.Verify(r => r.AddBookToBookshelf(It.IsAny<int>(), It.IsAny<BookMetadataDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_PropagatesConflict_WhenBookAlreadyExistsInBookshelf()
        {
            // Arrange
            var repositoryMock = new Mock<IBookshelfRepository>();
            repositoryMock
                .Setup(r => r.GetBookshelfById(BookshelfId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Bookshelf { Id = BookshelfId, Name = "Read", UserId = UserId });
            repositoryMock
                .Setup(r => r.AddBookToBookshelf(BookshelfId, It.IsAny<BookMetadataDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BookshelfBookConflictException(BookshelfId, BookId));

            var handler = new AddBookToBookshelfHandler(repositoryMock.Object);
            var command = new AddBookToBookshelfCommand(BookshelfId, BookId, Metadata, UserId);

            // Act + Assert
            await Assert.ThrowsAsync<BookshelfBookConflictException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
