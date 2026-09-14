using MediatR;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelfBooks
{
    public class GetBookshelfBooksHandler : IRequestHandler<GetBookshelfBooksQuery, GetBookshelfBooksDto>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public GetBookshelfBooksHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public async Task<GetBookshelfBooksDto> Handle(GetBookshelfBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookshelfRepository.GetBookshelfBooks(
                request.BookshelfId,
                request.PageNumber,
                request.PageSize,
                orderBy: request.OrderBy,
                orderDir: request.OrderDir,
                cancellationToken: cancellationToken);

            return new GetBookshelfBooksDto
            {
                Data = books
            };
        }
    }
}
