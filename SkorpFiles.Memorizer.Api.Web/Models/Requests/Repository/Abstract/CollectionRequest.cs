namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract
{
    public class CollectionRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
    }
}
