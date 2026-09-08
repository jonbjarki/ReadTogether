using MediatR;
using ReadTogether.Domain.DTOs;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf
{
    public class AddBookToBookshelfHandler : IRequestHandler<AddBookToBookshelfCommand, AddBookToBookshelfDto>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public AddBookToBookshelfHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public async Task<AddBookToBookshelfDto> Handle(AddBookToBookshelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = await _bookshelfRepository.GetBookshelfById(request.BookshelfId, cancellationToken);
            if (bookshelf is null || bookshelf.UserId != request.UserId)
            {
                throw new NotFoundException("Bookshelf", request.BookshelfId.ToString());
            }

            var metadata = request.Metadata with { Id = request.BookId };
            var bookshelfBook = await _bookshelfRepository.AddBookToBookshelf(request.BookshelfId, metadata, cancellationToken);
            return new AddBookToBookshelfDto
            {
                BookshelfId = bookshelfBook.BookshelfId,
                Id = bookshelfBook.BookId
            };
        }
    }
}
