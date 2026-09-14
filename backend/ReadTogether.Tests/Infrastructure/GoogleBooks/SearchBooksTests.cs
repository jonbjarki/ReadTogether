using Microsoft.Extensions.Logging;
using Moq.Contrib.HttpClient;
using ReadTogether.Domain.Common;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Implementations;
using ReadTogether.Infrastructure.Responses;
using Xunit.Abstractions;

namespace ReadTogether.Tests.Infrastructure.GoogleBooks
{
    public class SearchBooksTests(ITestOutputHelper testOutputHelper) : GoogleBooksTestsBase(testOutputHelper)
    {
        [Fact]
        public async Task SearchBooks_ReturnsResults()
        {
            // Arrange
            var handler = CreateMockHandler();
            var query = GoogleBooksAPI.GetSearchQueryUrl("test", null, null, 1, 10);
            var fullPath = BaseUrl + query;
            handler.SetupRequest(HttpMethod.Get, fullPath)
                .ReturnsJsonResponse(new GoogleBooksSearchResponse
                {
                    Kind = "books#volumes",
                    TotalItems = 2,
                    Items = [MockVolume1, MockVolume2]
                });

            var client = CreateMockHttpClient(handler);
            var logger = loggerFactory.CreateLogger<GoogleBooksAPI>();
            var api = new GoogleBooksAPI(client, logger);

            // Act
            var result = await api.Search("test", CancellationToken.None, null, null, 1, 10);

            // Assert
            Assert.IsType<PagedBookSearchDto>(result);
            Assert.Equal(2, result.Results.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Single(result.Results, r => r.Id == MockVolume1.Id);
            Assert.Single(result.Results, r => r.Id == MockVolume2.Id);
        }

        [Fact]
        public async Task SearchBooks_ReturnsEmptyResults_WhenNoBooksFound()
        {
            // Arrange
            var handler = CreateMockHandler();
            var query = GoogleBooksAPI.GetSearchQueryUrl("test", null, null, 1, 10);
            var fullPath = BaseUrl + query;
            handler.SetupRequest(HttpMethod.Get, fullPath)
                .ReturnsJsonResponse(new GoogleBooksSearchResponse
                {
                    Kind = "books#volumes",
                    TotalItems = 0,
                    Items = []
                });

            var client = CreateMockHttpClient(handler);
            var logger = loggerFactory.CreateLogger<GoogleBooksAPI>();
            var api = new GoogleBooksAPI(client, logger);

            // Act
            var result = await api.Search("test", CancellationToken.None, null, null, 1, 10);

            // Assert
            Assert.IsType<PagedBookSearchDto>(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Empty(result.Results);
        }

        [Fact]
        public async Task SearchBooks_ThrowsGoogleBooksRateLimitExceededException_WhenRateLimitExceeded()
        {
            // Arrange
            var handler = CreateMockHandler();
            var query = GoogleBooksAPI.GetSearchQueryUrl("test", null, null, 1, 10);
            var fullPath = BaseUrl + query;
            handler.SetupRequest(HttpMethod.Get, fullPath)
                .ReturnsResponse(System.Net.HttpStatusCode.TooManyRequests);

            var client = CreateMockHttpClient(handler);
            var api = new GoogleBooksAPI(client, loggerFactory.CreateLogger<GoogleBooksAPI>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GoogleBooksRateLimitExceededException>(
                async () => await api.Search("test", CancellationToken.None, null, null, 1, 10));
            Assert.IsType<GoogleBooksRateLimitExceededException>(exception);
        }

        [Theory]
        [InlineData(System.Net.HttpStatusCode.InternalServerError)]
        [InlineData(System.Net.HttpStatusCode.BadRequest)]
        public async Task SearchBooks_ThrowsGoogleBooksApiException_OnNonSuccessStatusCodes(System.Net.HttpStatusCode statusCode)
        {
            // Arrange
            var handler = CreateMockHandler();
            var query = GoogleBooksAPI.GetSearchQueryUrl("test", null, null, 1, 10);
            var fullPath = BaseUrl + query;
            handler.SetupRequest(HttpMethod.Get, fullPath)
                .ReturnsResponse(statusCode);

            var client = CreateMockHttpClient(handler);
            var api = new GoogleBooksAPI(client, loggerFactory.CreateLogger<GoogleBooksAPI>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GoogleBooksApiException>(
                async () => await api.Search("test", CancellationToken.None, null, null, 1, 10));
            Assert.IsType<GoogleBooksApiException>(exception);
            Assert.NotEqual(exception.Message, string.Empty);
        }


        [Fact]
        public void GetSearchQueryUrl_GeneratesCorrectQueryString()
        {
            // simple case
            var url = GoogleBooksAPI.GetSearchQueryUrl("book title", "author", null, 3, 5);
            // page 3, pageSize 5 -> startIndex = 10
            Assert.Equal("volumes?q=book%20title+inauthor:author&startIndex=10&maxResults=6", url);

            // verify that special characters are escaped
            var url2 = GoogleBooksAPI.GetSearchQueryUrl("c# books", null, null, 1, 1);
            Assert.Equal("volumes?q=c%23%20books&startIndex=0&maxResults=2", url2);
        }

    }
}