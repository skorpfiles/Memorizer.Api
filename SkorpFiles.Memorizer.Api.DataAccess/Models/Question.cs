﻿using SkorpFiles.Memorizer.Api.DataAccess.Enums;
using SkorpFiles.Memorizer.Api.DataAccess.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("rQuestion", Schema = Constants.MemorizerSchemaName)]
    public class Question:ObjectWithLifetime
    {
        [Key]
        public Guid QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public string QuestionText { get; set; } = null!;
        public string? QuestionUntypedAnswer { get; set; }
        public bool QuestionIsEnabled { get; set; }
        public string? QuestionReference { get; set; }
        public bool QuestionIsFixed { get; set; }
        public int QuestionEstimatedTrainingTimeSeconds { get; set; }
        public Guid QuestionnaireId { get; set; }
        public int QuestionQuestionnaireCode { get; set; }

        public List<EntityLabel>? LabelsForQuestion { get; set; }
        public Questionnaire? Questionnaire { get; set; }
        public List<QuestionUser>? UsersForQuestion { get; set; }
        public List<TypedAnswer>? TypedAnswers { get; set; }
        public List<TrainingResult>? TrainingResults { get; set; }
    }
}