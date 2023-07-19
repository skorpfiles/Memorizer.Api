using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("nnTrainingQuestionnaire", Schema = Constants.MemorizerSchemaName)]
    public class TrainingQuestionnaire:ObjectWithCreationTime
    {
        [Key]
        public Guid TrainingQuestionnaireId { get; set; }
        public Guid TrainingId { get; set; }
        public Guid QuestionnaireId { get; set; }

        [ForeignKey(nameof(TrainingId))]
        public Training? Training { get; set; }

        [ForeignKey(nameof(QuestionnaireId))]
        public Questionnaire? Questionnaire { get; set; }
    }
}
