using SkorpFiles.Memorizer.Api.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class Label:Entity
    {
        public string? Name { get; set; }
        public Guid? OwnerId { get; set; }
        public LabelInQuestionnaire? StatusInQuestionnaire { get; set; }
    }
}
