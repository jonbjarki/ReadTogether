namespace ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf
{
    /// <summary>
    /// Result of adding a book to a bookshelf, identifying the affected shelf and book.
    /// </summary>
    public class AddBookToBookshelfDto
    {
        /// <summary>
        /// Gets or sets the ID of the bookshelf the book was added to.
        /// </summary>
        public required int BookshelfId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the added book (the Google Books volume ID).
        /// </summary>
        public required string Id { get; set; }
    }
}
