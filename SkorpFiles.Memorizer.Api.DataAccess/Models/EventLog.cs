using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("jEventLog", Schema = Constants.MemorizerSchemaName)]
    public class EventLog
    {
        [Key]
        public Guid EventId { get; set; }
        [Column("EventTime")]
        public DateTime EventTimeUtc { get; set; }
        public string? EventMessage { get; set; }
    }
}
