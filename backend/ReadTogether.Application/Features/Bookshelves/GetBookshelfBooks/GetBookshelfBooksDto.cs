using ReadTogether.Domain.Common;
using ReadTogether.Domain.DTOs;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelfBooks
{
    public class GetBookshelfBooksDto
    {
        public PagedResponse<BookshelfBookDto> Data { get; set; } = null!;
    }
}
