namespace ReadTogether.Domain.Common
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Results { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}