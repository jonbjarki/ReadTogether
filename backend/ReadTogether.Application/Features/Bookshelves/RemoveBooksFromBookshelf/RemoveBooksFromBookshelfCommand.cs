using MediatR;

namespace ReadTogether.Application.Features.Bookshelves.RemoveBooksFromBookshelf
{
    public record RemoveBooksFromBookshelfCommand(int BookshelfId, string[] BookIds, string UserId, CancellationToken CancellationToken) : IRequest<bool>;
}