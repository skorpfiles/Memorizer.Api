using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class QuestionIdentifier:EntityIdentifier
    {
        public int? CodeInQuestionnaire { get; set; }
        public new int? Code
        {
            get => CodeInQuestionnaire;
            set => CodeInQuestionnaire = value;
        }
    }
}
