using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class ExistingQuestion:Question
    {
        public IEnumerable<TypedAnswer>? TypedAnswers { get; set; }
    }
}
