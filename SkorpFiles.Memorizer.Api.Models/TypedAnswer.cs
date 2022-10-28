using SkorpFiles.Memorizer.Api.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class TypedAnswer:Entity
    {
        public Guid QuestionId { get; set; }
        public string? Text { get; set; }
    }
}
