using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static ReadTogether.Domain.Common.SortingTypes;

namespace ReadTogether.API.InputModels.Bookshelves
{
    public class GetBookshelfBooksInputModel
    {
        [FromRoute]
        public int Id { get; set; }

        [FromQuery]
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;
        [FromQuery]
        public int PageSize { get; set; } = 10;
        [FromQuery]
        public OrderBy OrderBy { get; set; } = OrderBy.DateAdded;
        [FromQuery]
        public OrderDir OrderDir { get; set; } = OrderDir.Desc;

    };
}