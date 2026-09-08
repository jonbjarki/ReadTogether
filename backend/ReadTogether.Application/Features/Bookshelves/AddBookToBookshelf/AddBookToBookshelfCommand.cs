using MediatR;
using ReadTogether.Domain.DTOs;

namespace ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf
{
    /// <summary>
    /// Command to add a book to a bookshelf. <paramref name="Metadata"/> supplies the data for
    /// creating the shared local book copy when it has not been stored by a previous add.
    /// </summary>
    public record AddBookToBookshelfCommand(int BookshelfId, string BookId, BookMetadataDto Metadata, string UserId) : IRequest<AddBookToBookshelfDto>;
}