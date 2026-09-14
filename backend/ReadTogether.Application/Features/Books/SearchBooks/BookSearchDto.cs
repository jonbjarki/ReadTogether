using ReadTogether.Domain.Common;
using ReadTogether.Infrastructure.Dtos;
using ReadTogether.Infrastructure.Responses;

namespace ReadTogether.Application.Features.Books.SearchBooks
{
    public class BookSearchDto
    {
        public PagedBookSearchDto Data { get; set; } = null!;
    }
}