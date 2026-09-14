using MediatR;
using ReadTogether.Application.Features.Bookshelves.CreateBookshelf;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.DeleteBookshelf
{
    public class DeleteBookshelfHandler : IRequestHandler<DeleteBookshelfCommand, bool>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public DeleteBookshelfHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public async Task<bool> Handle(DeleteBookshelfCommand request, CancellationToken cancellationToken)
        {
            return await _bookshelfRepository.DeleteBookshelf(request.BookshelfId, request.UserId, cancellationToken);
        }
    }
}