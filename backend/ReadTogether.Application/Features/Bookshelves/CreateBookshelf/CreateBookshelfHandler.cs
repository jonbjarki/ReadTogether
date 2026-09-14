using MediatR;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Bookshelves.CreateBookshelf
{
    public class CreateBookshelfHandler : IRequestHandler<CreateBookshelfCommand, CreateBookshelfDto>
    {
        private readonly IBookshelfRepository _bookshelfRepository;

        public CreateBookshelfHandler(IBookshelfRepository bookshelfRepository)
        {
            _bookshelfRepository = bookshelfRepository;
        }

        public async Task<CreateBookshelfDto> Handle(CreateBookshelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = await _bookshelfRepository.CreateBookshelf(request.Name, request.UserId, cancellationToken);
            return new CreateBookshelfDto
            {
                Id = bookshelf.Id,
                Name = bookshelf.Name,
                UserId = bookshelf.UserId
            };
        }
    }
}