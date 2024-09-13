namespace SkorpFiles.Memorizer.Api.Web.Models.Helpers
{
    public class EmailParameters
    {
        public required string FromAddress { get; set; }
        public required string FromName { get; set; }
        public required string ToAddress { get; set; }
        public required string ToName { get; set; }
        public string? Subject { get; set; }
        public required string Content { get; set; }
        public string? ApiKey { get; set; }
        public Dictionary<string, object>? Attributes { get; set; }
    }
}
