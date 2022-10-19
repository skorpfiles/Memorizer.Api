namespace SkorpFiles.Memorizer.Api.ApiModels.ApiEntities
{
    public class Label
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Number { get; set; }
        public Guid ParentLabelId { get; set; }
    }
}
