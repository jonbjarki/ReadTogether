using MediatR;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.RemoveBooksFromBookshelf
{
    public class RemoveBooksFromBookshelfHandler : IRequestHandler<RemoveBooksFromBookshelfCommand, bool>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public RemoveBooksFromBookshelfHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public Task<bool> Handle(RemoveBooksFromBookshelfCommand request, CancellationToken cancellationToken)
        {
            return _bookshelfRepository.RemoveBooksFromBookshelf(request.BookshelfId, request.BookIds, request.UserId, cancellationToken);
        }
    }
}