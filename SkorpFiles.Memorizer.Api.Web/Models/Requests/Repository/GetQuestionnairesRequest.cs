namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository
{
    public class GetQuestionnairesRequest
    {
        public string? Origin { get; set; }
        public Guid OwnerId { get; set; }
        public string? Availability { get; set; }
        public string? PartOfName { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public List<string>? LabelsNames { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
