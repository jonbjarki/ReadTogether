namespace ReadTogether.Application.Features.Bookshelves.GetBookshelvesByUser
{
    public class GetBookshelvesByUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDefaultShelf { get; set; }
        public DateTime CreatedAt { get; set; }
        // This property will only be present when a BookId is provided in the query
        // It indicates whether the specified book is in this bookshelf
        public bool? IsBookInShelf { get; set; } = null;
    }
}
