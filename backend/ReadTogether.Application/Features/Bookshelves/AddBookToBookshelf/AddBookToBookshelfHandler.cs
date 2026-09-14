using MediatR;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf
{
    public class AddBookToBookshelfHandler : IRequestHandler<AddBookToBookshelfCommand, AddBookToBookshelfDto>
    {
        private readonly IBookshelfRepository _bookshelfRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IGoogleBooksAPI _googleBooksAPI;

        public AddBookToBookshelfHandler(IBookshelfRepository bookshelfRepository, IGoogleBooksAPI googleBooksAPI, IBookRepository bookRepository)
        {
            _bookshelfRepository = bookshelfRepository;
            _googleBooksAPI = googleBooksAPI;
            _bookRepository = bookRepository;
        }

        public async Task<AddBookToBookshelfDto> Handle(AddBookToBookshelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = await _bookshelfRepository.GetBookshelfById(request.BookshelfId, cancellationToken);
            if (bookshelf is null || bookshelf.UserId != request.UserId)
            {
                throw new NotFoundException("Bookshelf", request.BookshelfId.ToString());
            }

            // Fetch stored cached book entry if already created
            var book = await _bookRepository.GetBook(request.BookId, cancellationToken);
            if (book is null)
            {
                // If book doesnt exist locally, then fetch it from the API and create it
                var metadata = await _googleBooksAPI.GetBookById(request.BookId, cancellationToken);
                if (metadata is null)
                {
                    throw new NotFoundException("Book", request.BookId);
                }
                book = await _bookRepository.CreateBook(metadata, cancellationToken);
            }

            var bookshelfBook = await _bookshelfRepository.AddBookToBookshelf(request.BookshelfId, book.Id, cancellationToken);
            return new AddBookToBookshelfDto
            {
                BookshelfId = bookshelfBook.BookshelfId,
                Id = bookshelfBook.BookId
            };
        }
    }
}
