using Bogus;
using Bogus.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Enums;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests.DataSources
{
    public static class EditingRepositoryTestDataSource
    {
        static EditingRepositoryTestDataSource()
        {
            Randomizer.Seed = new Random(54412789);
        }

        public static IEnumerable<object[]> GetQuestionnaireAsync_CorrectData_ReturnResultAsync
        {
            get
            {
                var questionnaire = new Faker<Questionnaire>()
                    .RuleFor(q => q.QuestionnaireName, f => f.Lorem.Word().ClampLength(1, Restrictions.QuestionnaireNameMaxLength))
                    .RuleFor(q => q.QuestionnaireAvailability, f => f.PickRandom<Availability>())
                    .RuleFor(q => q.ObjectCreationTimeUtc, DateTime.UtcNow)
                    .RuleFor(q => q.OwnerId, Constants.DefaultUserId.ToAspNetUserIdString())
                    .RuleFor(q => q.ObjectIsRemoved, false)
                    .Generate();
                questionnaire.Questions = new Faker<Question>()
                    .RuleFor(q=>q.QuestionType,QuestionType.Task)
                    .RuleFor(q=>q.QuestionText, f=>f.Lorem.Text().ClampLength(1,Restrictions.QuestionTextMaxLength))
                    .RuleFor(q=>q.QuestionIsEnabled, true)
                    .RuleFor(q=>q.QuestionEstimatedTrainingTimeSeconds, f=>f.Random.Number(Restrictions.QuestionTrainingTimeSecondsMinValue,Restrictions.QuestionTrainingTimeSecondsMaxValue))
                    .RuleFor(q=>q.QuestionnaireId,questionnaire.QuestionnaireId)
                    .RuleFor(q=>q.QuestionQuestionnaireCode,f=>f.IndexFaker)
                    .RuleFor(q=>q.ObjectCreationTimeUtc,DateTime.UtcNow)
                    .RuleFor(q=>q.QuestionUntypedAnswer,f=>f.Lorem.Text().ClampLength(1,Restrictions.QuestionUntypedAnswerMaxLength))
                    .GenerateBetween(0, 100);
                questionnaire.LabelsForQuestionnaire = new Faker<EntityLabel>()
                    .RuleFor(el => el.LabelNumber, f => f.IndexFaker)
                    .RuleFor(el=>el.EntityType,EntityType.Questionnaire)
                    .RuleFor(el=>el.ObjectCreationTimeUtc,DateTime.UtcNow)
                    .GenerateBetween(0, 10);
                foreach (var entityLabel in questionnaire.LabelsForQuestionnaire)
                {
                    entityLabel.LabelId = Guid.NewGuid();
                    entityLabel.Label = new Faker<Label>()
                        .RuleFor(l => l.LabelName, f => f.Lorem.Word().ClampLength(1, Restrictions.LabelNameMaxLength))
                        .RuleFor(l => l.ObjectCreationTimeUtc, DateTime.UtcNow)
                        .RuleFor(l=>l.OwnerId, Constants.DefaultUserId.ToAspNetUserIdString())
                        .Generate();
                }

                yield return new object[]
                {
                    questionnaire,
                    false
                };

                yield return new object[]
                {
                    questionnaire,
                    true
                };
            }
        }
    }
}
