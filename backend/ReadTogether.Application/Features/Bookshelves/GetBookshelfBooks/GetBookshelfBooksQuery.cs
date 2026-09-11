using MediatR;
using static ReadTogether.Domain.Common.SortingTypes;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelfBooks
{
    public record GetBookshelfBooksQuery(int BookshelfId, int PageNumber, int PageSize, OrderBy OrderBy, OrderDir OrderDir) : IRequest<GetBookshelfBooksDto>;
}
