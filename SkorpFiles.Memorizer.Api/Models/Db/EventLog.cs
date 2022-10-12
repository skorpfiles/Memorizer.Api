using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("jEventLog", Schema = Constants.MemorizerSchemaName)]
    public class EventLog
    {
        [Key]
        public Guid EventId { get; set; }
        [Column("EventTime")]
        public DateTime EventTimeUtc { get; set; }
        public Guid? EventQuestionId { get; set; }
        public bool? EventQuestionIsNew { get; set; }
        public string? EventTypedAnswers { get; set; }
        public int? EventResultRating { get; set; }
        public int? EventResultPenaltyPoints { get; set; }
        public string? EventMessage { get; set; }

        public Question? Question { get; set; }
    }
}
