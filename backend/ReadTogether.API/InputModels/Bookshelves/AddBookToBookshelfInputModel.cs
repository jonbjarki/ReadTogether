using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadTogether.API.InputModels.Bookshelves
{
    /// <summary>
    /// Request body for adding a book to a bookshelf. Carries the book metadata the client
    /// received from the Google Books API so the server can create the shared local
    /// <see cref="ReadTogether.Domain.Entities.Book"/> copy when it does not exist yet.
    /// </summary>
    public class AddBookToBookshelfInputModel
    {
        /// <summary>
        /// Gets or sets the book title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the author name, or <see langword="null"/> if unknown.
        /// </summary>
        public string? AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the year the book was first published, or <see langword="null"/> if unknown.
        /// </summary>
        public int? FirstPublishedYear { get; set; }

        /// <summary>
        /// Gets or sets the URL of the cover image, or <see langword="null"/> if unavailable.
        /// </summary>
        public string? CoverImageUrl { get; set; }
    }
}