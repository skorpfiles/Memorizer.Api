using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.Abstract
{
    public abstract class Entity:EntityIdentifier
    {
        public DateTime? CreationTimeUtc { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? RemovalTimeUtc { get; set; }
    }
}
