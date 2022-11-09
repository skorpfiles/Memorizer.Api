using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class UpdateQuestionnaireRequest
    {
        public string? Name { get; set; }
        public Availability Availability { get; set; }
        public IEnumerable<LabelInQuestionnaire>? Labels { get; set; }
    }
}
