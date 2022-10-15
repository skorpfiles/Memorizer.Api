namespace SkorpFiles.Memorizer.Api.Models.Requests.Repository
{
    public class GetQuestionnariesRequest
    {
        public string? Origin { get; set; }
        public Guid OwnerId { get; set; }
        public string? Availability { get; set; }
        public string? SearchInName { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public List<string>? Labels { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
