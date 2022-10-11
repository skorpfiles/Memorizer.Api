using SkorpFiles.Memorizer.Api.Enums.Db;
using SkorpFiles.Memorizer.Api.Models.Db.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db
{
    [Table("nnEntityLabel")]
    public class EntityLabel:ObjectWithCreationTime
    {
        [Key]
        public Guid EntityLabelId { get; set; }
        public Guid? QuestionnaireId { get; set; }
        public Guid? QuestionId { get; set; }
        public Guid LabelId { get; set; }
        public Guid? ParentLabelId { get; set; }
        public int LabelNumber { get; set; }
        public EntityType EntityType { get; set; }
        public Guid OwnerId { get; set; }
    }
}
