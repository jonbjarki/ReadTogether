using MediatR;
using ReadTogether.Infrastructure.Interfaces;
using ReadTogether.Infrastructure.Responses;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelf
{
    public class GetBookshelfHandler : IRequestHandler<GetBookshelfQuery, GetBookshelfDto?>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public GetBookshelfHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public async Task<GetBookshelfDto?> Handle(GetBookshelfQuery request, CancellationToken cancellationToken)
        {
            var bookshelf = await _bookshelfRepository.GetBookshelfById(request.Id, cancellationToken);
            if (bookshelf is null)
            {
                return null;
            }

            return new GetBookshelfDto
            {
                Id = bookshelf.Id,
                Name = bookshelf.Name,
                UserId = bookshelf.UserId,
                TotalBooks = bookshelf.BookshelfBooks.Count
            };
        }
    }
}
