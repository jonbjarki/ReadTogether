using System;
using System.Collections.Generic;
using System.Text;

namespace ReadTogether.Domain.DTOs
{
    public class BookshelfBookDto
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public int? FirstPublishedYear { get; set; }
        public string? CoverImageUrl { get; set; }

    }
}
