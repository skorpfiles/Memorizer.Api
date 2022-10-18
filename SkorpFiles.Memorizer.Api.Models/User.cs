using SkorpFiles.Memorizer.Api.Models.Abstract;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class User:Entity
    {
        public string? Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}
