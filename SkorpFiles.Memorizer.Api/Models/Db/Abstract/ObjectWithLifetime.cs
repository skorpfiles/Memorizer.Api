using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db.Abstract
{
    public abstract class ObjectWithLifetime:ObjectWithCreationTime
    {
        public bool ObjectIsRemoved { get; set; }

        [Column("ObjectRemovalTime")]
        public DateTime? ObjectRemovalTimeUtc { get; set; }
    }
}
