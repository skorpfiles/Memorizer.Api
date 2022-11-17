using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class QuestionToUpdate:Question
    {
        public IEnumerable<Guid>? LabelsIds { get; set; }
        public IEnumerable<string>? TypedAnswers { get; set; }
    }
}
