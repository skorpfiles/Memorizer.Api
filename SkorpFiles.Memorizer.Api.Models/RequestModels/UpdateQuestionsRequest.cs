using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class UpdateQuestionsRequest
    {
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionnaireCode { get; set; }
        public IEnumerable<Question>? CreatedQuestions { get; set; }
        public IEnumerable<Question>? UpdatedQuestions { get; set; }
        public IEnumerable<Question>? DeletedQuestions { get; set; }
    }
}
