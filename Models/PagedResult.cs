namespace MyAuthDemo.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public string Search { get; set; } = "";
        public string Status { get; set; } = "";

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
