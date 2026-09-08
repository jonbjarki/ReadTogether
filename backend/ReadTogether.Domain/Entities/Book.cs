using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReadTogether.Domain.Entities
{
    /// <summary>
    /// Represents a locally stored copy of a book fetched from the Google Books API.
    /// A single <see cref="Book"/> record exists per Google Books volume and is shared
    /// by every bookshelf that references it through <see cref="BookshelfBook"/>.
    /// </summary>
    /// <remarks>
    /// The local copy exists so books can be added to bookshelves without depending on
    /// the external API at read time. Metadata mirrors <c>BookDetailsDto</c> and is set
    /// by the first client to add the book; subsequent adds reuse the existing record.
    /// </remarks>
    [PrimaryKey(nameof(Id))]
    public class Book
    {
        /// <summary>
        /// Gets or sets the Google Books volume ID. Used as the primary key.
        /// </summary>
        public required string Id { get; set; }

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

        // Navigation properties
        /// <summary>
        /// Gets or sets the join entries linking this book to bookshelves.
        /// </summary>
        public ICollection<BookshelfBook> BookshelfBooks { get; set; } = new List<BookshelfBook>();
    }
}
