using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ReadTogether.Domain.Common;
using ReadTogether.Domain.DTOs;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Interfaces;
using ReadTogether.Infrastructure.Responses;


namespace ReadTogether.Infrastructure.Implementations
{
    public class GoogleBooksAPI : IGoogleBooksAPI
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<GoogleBooksAPI> _logger;
        public GoogleBooksAPI(HttpClient httpClient, ILogger<GoogleBooksAPI> logger)
        {
            this.httpClient = httpClient;
            _logger = logger;
        }


        public async Task<AuthorDetailsDto?> GetAuthorById(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<BookDetailsDto> GetBookById(string id, CancellationToken cancellationToken)
        {
            var url = $"volumes/{id}";
            _logger.LogInformation("Fetching book details with URL: {Url}", httpClient.BaseAddress + url);

            try
            {
                var res = await httpClient.GetAsync(url, cancellationToken);

                if (!res.IsSuccessStatusCode)
                {
                    var errorContent = await res.Content.ReadAsStringAsync(cancellationToken);
                    var statusCode = (int)res.StatusCode;

                    if (statusCode == 429)
                    {
                        _logger.LogWarning("Rate limit exceeded when fetching book with ID: {BookId}", id);
                        throw new GoogleBooksRateLimitExceededException();
                    }

                    if (statusCode == 404 || statusCode == 503) // Books API returns 503 if book is not found
                    {
                        _logger.LogWarning("Book not found with ID: {BookId}", id);
                        throw new NotFoundException("Book", id);
                    }

                    _logger.LogError("Google Books API returned error {StatusCode} for book ID {BookId}", statusCode, id);
                    throw new GoogleBooksApiException(
                        $"Google Books API returned {statusCode} when fetching book details",
                        url,
                        statusCode,
                        errorContent
                    );
                }

                var response = await res.Content.ReadFromJsonAsync<Volume>(cancellationToken);

                if (response == null || response.VolumeInfo == null)
                {
                    _logger.LogError("Google Books API returned empty response for book ID: {BookId}", id);
                    throw new GoogleBooksApiException(
                        "Google Books API returned an empty response for book details",
                        url,
                        (int)res.StatusCode,
                        null
                    );
                }

                var volumeInfo = response.VolumeInfo;

                // Extract the year from PublishedDate (format is typically either "YYYY" or "YYYY-MM-DD")
                int? publishedYear = null;
                if (!string.IsNullOrEmpty(volumeInfo.PublishedDate))
                {
                    if (int.TryParse(volumeInfo.PublishedDate.AsSpan(0, 4), out var year))
                    {
                        publishedYear = year;
                    }
                }

                var bookDetails = new BookDetailsDto
                {
                    Id = response.Id,
                    Title = volumeInfo.Title,
                    Description = volumeInfo.Description,
                    FirstPublishedYear = publishedYear,
                    AuthorName = volumeInfo.Authors?.FirstOrDefault(), // Just take the first author for simplicity
                    CoverImageUrl = volumeInfo.ImageLinks?.Thumbnail,
                };

                _logger.LogInformation("Successfully fetched book details for ID: {BookId}", id);
                return bookDetails;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (GoogleBooksRateLimitExceededException)
            {
                throw;
            }
            catch (GoogleBooksApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching book details for ID: {BookId}", id);
                throw new GoogleBooksApiException(
                    $"Unexpected error when fetching book details for ID '{id}'",
                    url,
                    null,
                    ex.Message,
                    ex
                );
            }
        }

        public static string GetSearchQueryUrl(string title, string? author, string? subject, int page, int pageSize)
        {
            var startIndex = (page - 1) * pageSize;
            var escapedString = Uri.EscapeDataString(title);
            var authorQuery = author != null ? $"+inauthor:{Uri.EscapeDataString(author)}" : "";
            var subjectQuery = subject != null ? $"+subject:{Uri.EscapeDataString(subject)}" : "";
            return $"volumes?q={escapedString}{authorQuery}{subjectQuery}&startIndex={startIndex}&maxResults={pageSize + 1}"; // We request pageSize + 1 items to determine if there is a next page
        }

        public async Task<PagedBookSearchDto> Search(string title, CancellationToken cancellationToken, string? author = null, string? subject = null, int page = 1, int pageSize = 10)
        {
            var query = GetSearchQueryUrl(title, author, subject, page, pageSize);
            _logger.LogInformation("Searching for books with url: {Url}", httpClient.BaseAddress + query);
            var res = await httpClient.GetAsync(query, cancellationToken);

            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync(cancellationToken);
                var statusCode = (int)res.StatusCode;
                if (statusCode == 429)
                {
                    throw new GoogleBooksRateLimitExceededException();
                }

                throw new GoogleBooksApiException(
                    $"Google Books API returned {(int)res.StatusCode} for book search",
                    query,
                    (int)res.StatusCode,
                    errorContent
                );
            }
            var response = await res.Content.ReadFromJsonAsync<GoogleBooksSearchResponse>(cancellationToken);

            // Since there were no errors, we can parse the response.
            if (response is null) // This means the API returned an empty response, which is unexpected
            {
                throw new GoogleBooksApiException(
                    "Google Books API returned an empty response for book search",
                    query,
                    (int)res.StatusCode,
                    null
                );
            }
            _logger.LogInformation("Successfully received response from Google Books API with items count: {ItemsCount}", response.Items?.Length ?? 0);

            if (response.Items is null || response.Items.Length == 0) // No results found, return empty paged response
            {
                return new PagedBookSearchDto
                {
                    Page = page,
                    PageSize = pageSize,
                    Results = [],
                    HasNext = false,
                    HasPrevious = page > 1 // Assume that the client is not asking for an out of bounds page, so if page > 1, we can assume there is a previous page.
                };
            }


            var data = response.Items.Take(pageSize).Select(book => new BookSearchItemDto
            {
                Id = book.Id ?? throw new GoogleBooksApiException("Google Books API returned a book item with missing ID", query, (int)res.StatusCode, null),
                Title = book.VolumeInfo?.Title ?? "",
                Author = book.VolumeInfo?.Authors?.FirstOrDefault() ?? "",
                FirstPublished = ParsePublishedDate(book.VolumeInfo?.PublishedDate),
                CoverImageUrl = book.VolumeInfo?.ImageLinks?.Thumbnail ?? "",

            }).ToList();

            return new PagedBookSearchDto
            {
                Page = page,
                PageSize = pageSize,
                Results = data,
                HasNext = response.Items.Length > pageSize,
                HasPrevious = page > 1
            };
        }

        /// <summary>
        /// Helper method to extract the year from the published date string returned by the Google Books API
        /// </summary>
        /// <param name="publishedDate"></param>
        /// <returns></returns>
        private static string? ParsePublishedDate(string? publishedDate)
        {
            if (string.IsNullOrEmpty(publishedDate))
                return null;

            try
            {
                // The published date is either in the format "YYYY" or "YYYY-MM-DD". We just want the year part.
                var publishedYear = publishedDate.AsSpan(0, 4);
                if (int.TryParse(publishedYear, out _)) // If the first 4 characters can be parsed as an int, we assume it's a valid year
                {
                    return publishedYear.ToString();
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the publishedDate is shorter than 4 characters, we will get an ArgumentOutOfRangeException. In that case, we just return null.
                return null;
            }

            return null; // If we can't parse the year, return null
        }
    }
}