using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class LabelInQuestionnaire
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid? ParentLabelId { get; set; }
    }
}
