using Bogus;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests.DataSources
{
    public static class EditingRepositoryTestDataSource
    {
        public static IEnumerable<object[]> GetQuestionnaireAsync_CorrectData_ReturnResultAsync
        {
            get
            {
                yield return new object[]
                {
                    new Faker<Questionnaire>()
                        .RuleFor(q=>q.QuestionnaireName,f=>f.Lorem.Word())
                        .RuleFor(q=>q.QuestionnaireAvailability, f=>f.PickRandom<Availability>())
                        .RuleFor(q=>q.ObjectCreationTimeUtc, DateTime.UtcNow)
                        .RuleFor(q=>q.OwnerId, Constants.DefaultUserId.ToAspNetUserIdString())
                        .RuleFor(q=>q.ObjectIsRemoved, false)
                        .RuleFor(q=>q.LabelsForQuestionnaire, new List<EntityLabel>())
                        .Generate(),
                    0
                };
            }
        }
    }
}
