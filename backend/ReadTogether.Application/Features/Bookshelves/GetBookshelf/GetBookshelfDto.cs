using ReadTogether.Domain.Common;
using ReadTogether.Infrastructure.Responses;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelf
{
    public class GetBookshelfDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string UserId { get; set; }
        public required int TotalBooks { get; set; }
    }
}
