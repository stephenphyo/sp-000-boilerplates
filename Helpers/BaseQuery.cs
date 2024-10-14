namespace SP_000.Helpers
{
    public class BaseQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "";
        public string SortOrder { get; set; } = "asc";
    }
}