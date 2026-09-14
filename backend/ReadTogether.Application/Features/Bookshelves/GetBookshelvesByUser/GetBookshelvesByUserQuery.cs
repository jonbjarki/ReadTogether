using MediatR;

namespace ReadTogether.Application.Features.Bookshelves.GetBookshelvesByUser
{
    public record GetBookshelvesByUserQuery(string UserName, string? BookId = null) : IRequest<List<GetBookshelvesByUserDto>?>;
}
