using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReadTogether.Domain.Entities
{
    /// <summary>
    /// Join entity linking a <see cref="Bookshelf"/> to a shared <see cref="Book"/> record.
    /// Book metadata lives on <see cref="Book"/> so multiple shelves can reference the same
    /// local copy instead of duplicating it per shelf.
    /// </summary>
    [PrimaryKey(nameof(BookshelfId), nameof(BookId))]
    public class BookshelfBook
    {

        /// <summary>
        /// Gets or sets the ID of the bookshelf this entry belongs to.
        /// </summary>
        public int BookshelfId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the referenced book (the Google Books volume ID).
        /// </summary>
        public required string BookId { get; set; }

        /// <summary>
        /// Gets or sets when the book was added to the bookshelf. Defaults to the current UTC time.
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Gets or sets the bookshelf this entry belongs to.
        /// </summary>
        public Bookshelf Bookshelf { get; set; } = null!;

        /// <summary>
        /// Gets or sets the shared book record this entry references.
        /// </summary>
        public Book Book { get; set; } = null!;
    }
}