using Bogus;
using Bogus.Extensions;
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
    public class TrainingRepositoryTestDataSource
    {
        static TrainingRepositoryTestDataSource()
        {
            Randomizer.Seed = new Random(54412789);
        }

        public static IEnumerable<object[]> GetQuestionsForTrainingAsync_CorrectParameters_ReturnsQuestions
        {
            get
            {
                // there are 2 questionnaires, the first has 5 questions of all types, the second has 2 questions.
                // there are questions without past trainings, with 1 training, with 6 trainings, with 12 trainings
                // trainings have times that make median time not equal to average time
                // there is another questionnaire that should not be included in the result

                Faker generalFaker = new();

                var maximumDateTimeForQuestion = new DateTime(2020, 1, 1);

                Faker<Questionnaire> questionnaireFaker = new Faker<Questionnaire>()
                    .RuleFor(q => q.QuestionnaireId, f => Guid.NewGuid())
                    .RuleFor(q => q.QuestionnaireName, f => f.Lorem.Word().ClampLength(1, Restrictions.QuestionnaireNameMaxLength))
                    .RuleFor(q => q.QuestionnaireAvailability, f => f.PickRandom<Availability>())
                    .RuleFor(q => q.ObjectCreationTimeUtc, f => f.Date.Between(DateTime.MinValue, maximumDateTimeForQuestion))
                    .RuleFor(q => q.OwnerId, Constants.DefaultUserId.ToAspNetUserIdString())
                    .RuleFor(q => q.ObjectIsRemoved, false);

                var questionnaires = questionnaireFaker.Generate(3);

                Faker<QuestionUser> questionUserFaker = new Faker<QuestionUser>()
                    .RuleFor(q => q.QuestionUserIsNew, f => f.Random.Bool())
                    .RuleFor(q => q.QuestionUserRating, f => f.Random.Number(Restrictions.MinQuestionRating, Restrictions.MaxQuestionRating))
                    .RuleFor(q => q.QuestionUserPenaltyPoints, f => f.Random.Number())
                    .RuleFor(q => q.UserId, Constants.DefaultUserId.ToAspNetUserIdString());

                Faker<Question> questionFaker = new Faker<Question>()
                    .RuleFor(q => q.QuestionId, f => Guid.NewGuid())
                    .RuleFor(q => q.QuestionType, QuestionType.Task)
                    .RuleFor(q => q.QuestionText, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionTextMaxLength))
                    .RuleFor(q => q.QuestionIsEnabled, true)
                    .RuleFor(q => q.ObjectCreationTimeUtc, f => f.Date.Between(DateTime.MinValue, maximumDateTimeForQuestion))
                    .RuleFor(q => q.QuestionReference, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionReferenceMaxLength))
                    .RuleFor(q => q.UsersForQuestion, questionUserFaker.Generate(1));

                Faker<TypedAnswer> typedAnswerFaker = new Faker<TypedAnswer>()
                    .RuleFor(ta => ta.TypedAnswerId, f => Guid.NewGuid())
                    .RuleFor(ta => ta.TypedAnswerText, f => f.Lorem.Word().ClampLength(1, Restrictions.TypedAnswerTextMaxLength));

                questionnaires[0].Questions = questionFaker.Generate(5);

                var expectedAverageTimes = new List<int>();
                var expectedTypedAnswersJsonsWithoutSpaces = new List<string?>();

                Faker<TrainingResult> trainingResultFaker = new Faker<TrainingResult>()
                    .RuleFor(tr => tr.TrainingResultRecordingTime, f => f.Date.Between(maximumDateTimeForQuestion, DateTime.UtcNow))
                    .RuleFor(tr => tr.TrainingResultUserId, Constants.DefaultUserId.ToAspNetUserIdString())
                    .RuleFor(tr => tr.TrainingResultInitialNewStatus, f => f.Random.Bool())
                    .RuleFor(tr => tr.TrainingResultInitialRating, f => f.Random.Number(Restrictions.MinQuestionRating, Restrictions.MaxQuestionRating))
                    .RuleFor(tr => tr.TrainingResultInitialPenaltyPoints, f => f.Random.Number())
                    .RuleFor(tr => tr.TrainingResultAnswerIsCorrect, f => f.Random.Bool())
                    .RuleFor(tr => tr.TrainingResultIsNew, f => f.Random.Bool())
                    .RuleFor(tr => tr.TrainingResultRating, f => f.Random.Number(Restrictions.MinQuestionRating, Restrictions.MaxQuestionRating))
                    .RuleFor(tr => tr.TrainingResultPenaltyPoints, f => f.Random.Number())
                    .RuleFor(tr => tr.TrainingResultTimeSeconds, f => f.Random.Number(1, Restrictions.QuestionTrainingTimeSecondsMaxValue));

                // question without trainings: expected average time should be equal to estimated time.
                questionnaires[0].Questions![0].QuestionType = QuestionType.Task;
                questionnaires[0].Questions![0].QuestionEstimatedTrainingTimeSeconds = generalFaker.PickRandom(1, Restrictions.QuestionTrainingTimeSecondsMaxValue);

                expectedAverageTimes.Add(questionnaires[0].Questions![0].QuestionEstimatedTrainingTimeSeconds);
                expectedTypedAnswersJsonsWithoutSpaces.Add(null);

                // question with 1 training: estimated time = 10, actual time = 5. Expected average time = 7.5.
                questionnaires[0].Questions![1].QuestionType = QuestionType.UntypedAnswer;
                questionnaires[0].Questions![1].QuestionUntypedAnswer = generalFaker.Lorem.Text().ClampLength(1, Restrictions.QuestionUntypedAnswerMaxLength);
                questionnaires[0].Questions![1].QuestionEstimatedTrainingTimeSeconds = 10;

                questionnaires[0].Questions![1].TrainingResults = trainingResultFaker.Generate(1);
                questionnaires[0].Questions![1].TrainingResults![0].TrainingResultTimeSeconds = 5;

                expectedAverageTimes.Add(7);
                expectedTypedAnswersJsonsWithoutSpaces.Add(null);

                //question with 8 trainings: estimated time = 10, actual times = 5, 5, 10, 12, 14, 15, 18, 1000. Expected average time = (13+10)/2 = 11.5 (not (152+10)/2 because of median)
                questionnaires[0].Questions![2].QuestionType = QuestionType.TypedAnswers;
                questionnaires[0].Questions![2].TypedAnswers = typedAnswerFaker.Generate(2);
                questionnaires[0].Questions![2].QuestionEstimatedTrainingTimeSeconds = 10;

                questionnaires[0].Questions![2].TrainingResults = trainingResultFaker.Generate(8);
                questionnaires[0].Questions![2].TrainingResults![0].TrainingResultTimeSeconds = 5;
                questionnaires[0].Questions![2].TrainingResults![1].TrainingResultTimeSeconds = 5;
                questionnaires[0].Questions![2].TrainingResults![2].TrainingResultTimeSeconds = 10;
                questionnaires[0].Questions![2].TrainingResults![3].TrainingResultTimeSeconds = 12;
                questionnaires[0].Questions![2].TrainingResults![4].TrainingResultTimeSeconds = 14;
                questionnaires[0].Questions![2].TrainingResults![5].TrainingResultTimeSeconds = 15;
                questionnaires[0].Questions![2].TrainingResults![6].TrainingResultTimeSeconds = 18;
                questionnaires[0].Questions![2].TrainingResults![7].TrainingResultTimeSeconds = 1000;

                expectedAverageTimes.Add(11);
                expectedTypedAnswersJsonsWithoutSpaces.Add(TypedAnswersListToJsonString([questionnaires[0].Questions![2].TypedAnswers![0], questionnaires[0].Questions![2].TypedAnswers![1]]));

                //question with 12 trainings: estimated time = 20000, actual times = 5, 5, 10, 12, 14, 14, 17, 21, 40, 52, 64, 8000. Expected average time = 15.5. (estimated time does not matter)
                questionnaires[0].Questions![3].QuestionType = QuestionType.UntypedAndTypedAnswers;
                questionnaires[0].Questions![3].QuestionUntypedAnswer = generalFaker.Lorem.Text().ClampLength(1, Restrictions.QuestionUntypedAnswerMaxLength);
                questionnaires[0].Questions![3].TypedAnswers = typedAnswerFaker.Generate(2);
                questionnaires[0].Questions![3].QuestionEstimatedTrainingTimeSeconds = 20000;

                questionnaires[0].Questions![3].TrainingResults = trainingResultFaker.Generate(12);
                questionnaires[0].Questions![3].TrainingResults![0].TrainingResultTimeSeconds = 5;
                questionnaires[0].Questions![3].TrainingResults![1].TrainingResultTimeSeconds = 5;
                questionnaires[0].Questions![3].TrainingResults![2].TrainingResultTimeSeconds = 10;
                questionnaires[0].Questions![3].TrainingResults![3].TrainingResultTimeSeconds = 12;
                questionnaires[0].Questions![3].TrainingResults![4].TrainingResultTimeSeconds = 14;
                questionnaires[0].Questions![3].TrainingResults![5].TrainingResultTimeSeconds = 14;
                questionnaires[0].Questions![3].TrainingResults![6].TrainingResultTimeSeconds = 17;
                questionnaires[0].Questions![3].TrainingResults![7].TrainingResultTimeSeconds = 21;
                questionnaires[0].Questions![3].TrainingResults![8].TrainingResultTimeSeconds = 40;
                questionnaires[0].Questions![3].TrainingResults![9].TrainingResultTimeSeconds = 52;
                questionnaires[0].Questions![3].TrainingResults![10].TrainingResultTimeSeconds = 64;
                questionnaires[0].Questions![3].TrainingResults![11].TrainingResultTimeSeconds = 8000;

                expectedAverageTimes.Add(15);
                expectedTypedAnswersJsonsWithoutSpaces.Add(TypedAnswersListToJsonString([questionnaires[0].Questions![3].TypedAnswers![0], questionnaires[0].Questions![3].TypedAnswers![1]]));

                //question with 3 trainings: estimated time = 1000, actual times = 10, 20, 30. Expected average time = (20+1000)/2 = 510.
                questionnaires[0].Questions![4].QuestionEstimatedTrainingTimeSeconds = 1000;

                questionnaires[0].Questions![4].TrainingResults = trainingResultFaker.Generate(3);
                questionnaires[0].Questions![4].TrainingResults![0].TrainingResultTimeSeconds = 10;
                questionnaires[0].Questions![4].TrainingResults![1].TrainingResultTimeSeconds = 20;
                questionnaires[0].Questions![4].TrainingResults![2].TrainingResultTimeSeconds = 30;

                expectedAverageTimes.Add(510);
                expectedTypedAnswersJsonsWithoutSpaces.Add(null);

                // questionnaire 2
                questionnaires[1].Questions = questionFaker.Generate(2);
                questionnaires[1].Questions![0].QuestionEstimatedTrainingTimeSeconds = 20;
                expectedAverageTimes.Add(20);
                expectedTypedAnswersJsonsWithoutSpaces.Add(null);

                questionnaires[1].Questions![1].QuestionEstimatedTrainingTimeSeconds = 25;
                expectedAverageTimes.Add(25);
                expectedTypedAnswersJsonsWithoutSpaces.Add(null);

                // questionnaire 3
                questionnaires[2].Questions = questionFaker.Generate(1);
                questionnaires[2].Questions![0].QuestionEstimatedTrainingTimeSeconds = 500;

                yield return new object[]
                {
                    questionnaires,
                    new List<Guid>{ questionnaires[0].QuestionnaireId, questionnaires[1].QuestionnaireId },
                    expectedAverageTimes,
                    expectedTypedAnswersJsonsWithoutSpaces
                };
            }
        }

        private static string TypedAnswersListToJsonString(List<TypedAnswer>? typedAnswers)
        {
            if (typedAnswers == null)
                return null;
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < typedAnswers.Count; i++)
            {
                sb.Append("{");
                sb.Append($"\"TypedAnswerId\":\"{typedAnswers[i].TypedAnswerId}\",");
                sb.Append($"\"TypedAnswerText\":\"{typedAnswers[i].TypedAnswerText}\"");
                sb.Append("}");
                if (i != typedAnswers.Count - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
