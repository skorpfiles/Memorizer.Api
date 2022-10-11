using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.Models.Db.Abstract
{
    public abstract class ObjectWithCreationTime
    {
        [Column("ObjectCreationTime")]
        public DateTime ObjectCreationTimeUtc { get; set; }
    }
}
