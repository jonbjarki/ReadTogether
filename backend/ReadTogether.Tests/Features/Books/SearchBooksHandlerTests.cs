using Moq;
using ReadTogether.Application.Features.Books.SearchBooks;
using ReadTogether.Domain.Common;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Interfaces;
using ReadTogether.Infrastructure.Responses;

namespace ReadTogether.Tests.Features.Books
{
    public class SearchBooksHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsData_WhenApiReturnsResult()
        {
            // Arrange
            var apiMock = new Mock<IGoogleBooksAPI>();
            var response = new PagedBookSearchDto
            {
                Page = 1,
                PageSize = 10,
                HasNext = false,
                HasPrevious = false,
                Results =
                [
                    new BookSearchItemDto { Id = "/works/OL123W", Title = "Test" }
                ]
            };
            apiMock.Setup(x => x.Search(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(response);

            var handler = new SearchBooksHandler(apiMock.Object);
            var query = new SearchBooksQuery("test", 1, 10, null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data.Results);
        }
    }
}
