using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.Abstract
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public DateTime? CreatedTimeUtc { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovalTimeUtc { get; set; }
    }
}
