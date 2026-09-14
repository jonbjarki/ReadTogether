using ReadTogether.Infrastructure.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadTogether.Infrastructure.Dtos
{
    public class PagedBookSearchDto
    {
        public IEnumerable<BookSearchItemDto> Results { get; set; } = null!;
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
