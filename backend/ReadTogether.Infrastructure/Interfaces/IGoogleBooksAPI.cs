using ReadTogether.Domain.Common;
using ReadTogether.Domain.DTOs;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Responses;

namespace ReadTogether.Infrastructure.Interfaces
{
    public interface IGoogleBooksAPI
    {
        Task<PagedBookSearchDto> Search(string title, CancellationToken cancellationToken, string? author = null, string? subject = null, int page = 1, int pageSize = 10);
        Task<BookDetailsDto> GetBookById(string id, CancellationToken cancellationToken);
        Task<AuthorDetailsDto?> GetAuthorById(string id, CancellationToken cancellationToken);
    }
}