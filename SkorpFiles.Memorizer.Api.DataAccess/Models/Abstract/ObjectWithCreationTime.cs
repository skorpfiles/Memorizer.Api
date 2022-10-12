using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract
{
    public abstract class ObjectWithCreationTime
    {
        [Column("ObjectCreationTime")]
        public DateTime ObjectCreationTimeUtc { get; set; }
    }
}
