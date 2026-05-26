using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetQuestionnairesRequest:GetCollectionRequest
    {
        public Origin? Origin { get; set; }
        public Guid? OwnerId { get; set; }
        public Availability? Availability { get; set; }
        public string? PartOfName { get; set; }
        public QuestionnaireSortField SortField { get; set; } = QuestionnaireSortField.Name;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public IEnumerable<string>? LabelsNames { get; set; }
    }
}
