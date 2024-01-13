using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class TrainingLogicTestDataSource
    {
        static TrainingLogicTestDataSource()
        {
            Randomizer.Seed = new Random(44472134);
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(0.01, 1),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,1),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(min:1.01),
                    faker.Random.Double(min:1.01),
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,0.99),
                    1,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    1,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                var term1 = faker.Random.Double(0.02, 0.5);
                var term2 = faker.Random.Double(1-term1+0.01, 0.99);

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    term1,
                    term2,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    term2,
                    term1,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    0,
                    Constants.NonPositiveLengthValueMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Int(max:1),
                    Constants.NonPositiveLengthValueMessage
                };
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    0,
                    0,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    0,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,0.99),
                    0,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    0,
                    1,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    1,
                    0,
                    faker.Random.Int(min:1)
                };

                var term1 = faker.Random.Double(0.01, 0.98);
                var term2 = 1 - term1;

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    term1,
                    term2,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    term2,
                    term1,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Int(min:1)
                };
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_NoQuestions_EmptyResult
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, faker.Random.Number(2, 100), 0, 0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, faker.Random.Number(2, 100), 0, 1
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    1, 
                    0
                };

                // === change length type ===

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1,1000), 
                    0, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1,1000), 
                    0, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time,
                    faker.Random.Number(1, 1000), 
                    0, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0, 
                    1
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4), 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4), 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4),
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Random.Guid(),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    1, 
                    0
                };
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_CorrectData_AllQuestionsAreDistinct
        {
            get
            {
                var generalFaker = new Faker();

                var userId = generalFaker.Random.Guid();
                var trainingId = generalFaker.Random.Guid();

                var userQuestionStatusFaker = new Faker<UserQuestionStatus>()
                    .RuleSet("new", rs =>
                    {
                        rs
                        .RuleFor(uqs => uqs.PenaltyPoints, 0)
                        .RuleFor(uqs => uqs.IsNew, true)
                        .RuleFor(uqs => uqs.Rating, Constants.InitialQuestionRating);
                    })
                    .RuleSet("penalty", rs =>
                    {
                        rs
                        .RuleFor(uqs => uqs.PenaltyPoints, f => f.Random.Number(min: 1))
                        .RuleFor(uqs => uqs.IsNew, false)
                        .RuleFor(uqs => uqs.Rating, Constants.InitialQuestionRating);
                    })
                    .RuleSet("usual", rs =>
                    {
                        rs
                        .RuleFor(uqs => uqs.PenaltyPoints, 0)
                        .RuleFor(uqs => uqs.IsNew, false)
                        .RuleFor(uqs => uqs.Rating, f => f.Random.Number(Constants.MinQuestionRating, Constants.MaxQuestionRating));
                    });

                var questionFaker = new Faker<Question>()
                    .RuleFor(q => q.Type, f => f.PickRandom<QuestionType>())
                    .RuleFor(q => q.Text, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionTextMaxLength))
                    .RuleFor(q => q.IsEnabled, true)
                    .RuleFor(q => q.EstimatedTrainingTimeSeconds, f => f.Random.Number(Restrictions.QuestionEstimatedTrainingTimeSecondsMinValue, Restrictions.QuestionEstimatedTrainingTimeSecondsMaxValue))
                    .RuleFor(q => q.QuestionnaireId, f => f.Random.Guid())
                    .RuleFor(q => q.CodeInQuestionnaire, f => f.IndexFaker)
                    .RuleFor(q => q.CreationTimeUtc, DateTime.UtcNow)
                    .RuleFor(q => q.UntypedAnswer, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionUntypedAnswerMaxLength))
                    .RuleSet("new", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, userQuestionStatusFaker.Generate("new"));
                    })
                    .RuleSet("penalty", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, userQuestionStatusFaker.Generate("penalty"));
                    })
                    .RuleSet("usual", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, userQuestionStatusFaker.Generate("usual"));
                    });

                // test cases
                // ----------------------- 0 0 [10,200] -------------------------

                var newCount = 0;
                var penaltyCount = 0;
                var usualCount = generalFaker.Random.Number(10, 200);
                var lengthValue = generalFaker.Random.Number(5, usualCount-1);

                var testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 200);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 200);
                lengthValue = generalFaker.Random.Number(usualCount+1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [10,200] 0 -------------------------

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(5, penaltyCount-1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = 0;
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(penaltyCount+1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [60,100] [10,50] -------------------------
                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(5, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(usualCount+1, penaltyCount-1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [10,200] QP -------------------------

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(5, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [10,50] [60,100] -------------------------

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(5, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [5,200] 0 0 -------------------------

                newCount = generalFaker.Random.Number(10, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(5, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [60,100] 0 [10,50] -------------------------

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(5, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(usualCount + 1, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(10, 50);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [20,200] 0 QN -------------------------

                newCount = generalFaker.Random.Number(20, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(5, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(20, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(20, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(20, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = usualCount + newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(20, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(usualCount + newCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [10,50] 0 [60,100] -------------------------

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(5, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(newCount + 1, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(10, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [60,100] [10,50] 0 -------------------------

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(5, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = newCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(10, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [270,310] [150,190] [50,70] -------------------------

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(5, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, usualCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = usualCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [310,350] [100,150] QP -------------------------

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(3, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount + usualCount - 1 );

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, newCount-1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [150,190] [50,70] [270,310] -------------------------

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(5, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount + newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount + newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + newCount + 1, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, usualCount + newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + newCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [310,350] QN [100,150] -------------------------

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(3, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [50,70] QN QP -------------------------

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(5, penaltyCount-1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount * 2 - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount * 2;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount * 2 + 1, penaltyCount * 3 - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount * 3;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount * 3 + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [100,150] QN [310,350] -------------------------

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(3, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount + newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = penaltyCount + newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(penaltyCount + newCount + 1, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [100,150] [310,350] QP -------------------------

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(3, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = usualCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [50,70] [150,190] [270,310] -------------------------

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(5, newCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, newCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + penaltyCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }
            }
        }

        private static IEnumerable<object[]> TestWithAllStandardFractionsAndLengthTypes(Faker generalFaker, Faker<Question> questionFaker, Guid userId, Guid trainingId, int newCount, int penaltyCount, int usualCount, int lengthValue)
        {
            List<TrainingLengthType> lengthTypes = new() { TrainingLengthType.QuestionsCount, TrainingLengthType.Time };
            foreach (var lengthType in lengthTypes)
            {
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, 0, 0);
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, 0, generalFaker.Random.Double(0.1, 0.4));
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, 0, generalFaker.Random.Double(0.6, 0.9));
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, 0, 1);
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, generalFaker.Random.Double(0.1, 0.4), 0);
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, generalFaker.Random.Double(0.1, 0.4), generalFaker.Random.Double(0.1, 0.4));
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, generalFaker.Random.Double(0.1, 0.25), generalFaker.Random.Double(0.6, 0.7));
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, generalFaker.Random.Double(0.6, 0.9), 0);
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, generalFaker.Random.Double(0.6, 0.7), generalFaker.Random.Double(0.1, 0.25));
                yield return GenerateQuestionData(generalFaker, questionFaker, userId, trainingId, newCount, penaltyCount, usualCount, lengthType, lengthValue, 1, 0);
            }
        }

        private static object[] GenerateQuestionData(Faker generalFaker, Faker<Question> questionFaker, Guid userId, Guid trainingId, int newCount, int penaltyCount, int usualCount, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction)
        {
            var questions = new List<Question>();
            questions.AddRange(questionFaker.GenerateBetween(newCount, newCount, "new"));
            questions.AddRange(questionFaker.GenerateBetween(penaltyCount, penaltyCount, "penalty"));
            questions.AddRange(questionFaker.GenerateBetween(usualCount, usualCount, "usual"));

            Shuffle(questions, generalFaker);

            return new object[]
            {
                userId,
                trainingId,
                lengthType,
                lengthValue,
                newQuestionsFraction,
                penaltyQuestionsFraction,
                questions
            };
        }

        private static void Shuffle<T>(List<T> list, Faker generalFaker)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = generalFaker.Random.Number(0, n);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
