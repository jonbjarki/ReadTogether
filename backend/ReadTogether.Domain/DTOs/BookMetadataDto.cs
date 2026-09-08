namespace ReadTogether.Domain.DTOs
{
    /// <summary>
    /// Carries the book metadata needed to create a local <see cref="ReadTogether.Domain.Entities.Book"/>
    /// record when a book is added to a bookshelf for the first time.
    /// </summary>
    /// <remarks>
    /// Mirrors the fields of <see cref="BookDetailsDto"/> returned by the Google Books API so
    /// clients can forward the details they already fetched when the user selected the book.
    /// </remarks>
    public record BookMetadataDto
    {
        /// <summary>
        /// Gets the book ID.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Gets the book title.
        /// </summary>
        public required string Title { get; init; }

        /// <summary>
        /// Gets the author name, or <see langword="null"/> if unknown.
        /// </summary>
        public string? AuthorName { get; init; }

        /// <summary>
        /// Gets the year the book was first published, or <see langword="null"/> if unknown.
        /// </summary>
        public int? FirstPublishedYear { get; init; }

        /// <summary>
        /// Gets the URL of the cover image, or <see langword="null"/> if unavailable.
        /// </summary>
        public string? CoverImageUrl { get; init; }
    }
}
