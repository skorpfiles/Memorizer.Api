using SkorpFiles.Memorizer.Api.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class Question:Entity
    {
        public QuestionType Type { get; set; }
        public string? Text { get; set; }
        public string? UntypedAnswer { get; set; }
        public bool IsEnabled { get; set; }
        public string? Reference { get; set; }
        public bool IsFixed { get; set; }
        public int EstimatedTrainingTimeSeconds { get; set; }
        public Guid QuestionnaireId { get; set; }
        public int? CodeInQuestionnaire { get; set; }
        public UserQuestionStatus? MyStatus { get; set; }
    }
}
