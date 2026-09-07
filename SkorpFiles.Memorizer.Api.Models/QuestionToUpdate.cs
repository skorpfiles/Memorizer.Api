using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class QuestionToUpdate:Question
    {
        public IEnumerable<string>? Labels { get; set; }
        public IEnumerable<string>? TypedAnswers { get; set; }
    }
}
