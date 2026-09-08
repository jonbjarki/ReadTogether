namespace ReadTogether.Infrastructure.Exceptions
{
    /// <summary>
    /// The exception thrown when a book is added to a bookshelf that already contains it.
    /// </summary>
    [Serializable]
    public class BookshelfBookConflictException : Exception
    {
        /// <summary>
        /// Gets the ID of the bookshelf involved in the conflict.
        /// </summary>
        public int BookshelfId { get; }

        /// <summary>
        /// Gets the ID of the book involved in the conflict.
        /// </summary>
        public string BookId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookshelfBookConflictException"/> class.
        /// </summary>
        /// <param name="bookshelfId">The ID of the bookshelf.</param>
        /// <param name="bookId">The ID of the book that is already on the shelf.</param>
        public BookshelfBookConflictException(int bookshelfId, string bookId)
            : base($"Book with ID '{bookId}' is already in bookshelf '{bookshelfId}'.")
        {
            BookshelfId = bookshelfId;
            BookId = bookId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookshelfBookConflictException"/> class.
        /// </summary>
        /// <param name="bookshelfId">The ID of the bookshelf.</param>
        /// <param name="bookId">The ID of the book that is already on the shelf.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public BookshelfBookConflictException(int bookshelfId, string bookId, Exception innerException)
            : base($"Book with ID '{bookId}' is already in bookshelf '{bookshelfId}'.", innerException)
        {
            BookshelfId = bookshelfId;
            BookId = bookId;
        }
    }
}
