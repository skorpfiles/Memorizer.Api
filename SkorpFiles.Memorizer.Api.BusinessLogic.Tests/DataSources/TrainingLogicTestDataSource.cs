﻿using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
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
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(0.01, 1),
                    faker.Random.Number(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Number(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,1),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Number(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(min:1.01),
                    faker.Random.Double(min:1.01),
                    faker.Random.Number(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,0.99),
                    1,
                    faker.Random.Number(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    1,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Number(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                var term1 = faker.Random.Double(0.02, 0.5);
                var term2 = faker.Random.Double(1-term1+0.01, 0.99);

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    term1,
                    term2,
                    faker.Random.Number(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    term2,
                    term1,
                    faker.Random.Number(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    0,
                    Constants.NonPositiveLengthValueMessage
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Number(max:1),
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
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    0,
                    0,
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    0,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,0.99),
                    0,
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    0,
                    1,
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    1,
                    0,
                    faker.Random.Number(min:1)
                };

                var term1 = faker.Random.Double(0.01, 0.98);
                var term2 = 1 - term1;

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    term1,
                    term2,
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    term2,
                    term1,
                    faker.Random.Number(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Number(min:1)
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
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, faker.Random.Number(2, 100), 0, 0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, faker.Random.Number(2, 100), 0, 1
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    faker.Random.Double(0.1,0.4), 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    0.5, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.QuestionsCount, 
                    faker.Random.Number(2,100), 
                    1, 
                    0
                };

                // === change length type ===

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1,1000), 
                    0, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1,1000), 
                    0, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time,
                    faker.Random.Number(1, 1000), 
                    0, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0, 
                    1
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4), 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4), 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    faker.Random.Double(0.1,0.4),
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    0
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    faker.Random.Double(0.1,0.4)
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    0.5, 
                    0.5
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    faker.Make(faker.Random.Number(1,10),faker.Random.Guid),
                    TrainingLengthType.Time, 
                    faker.Random.Number(1, 1000), 
                    1, 
                    0
                };
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_CorrectData
        {
            get
            {
                var generalFaker = new Faker();
                var userId = generalFaker.Random.Guid();
                var questionFaker = DataUtils.GetQuestionFaker(generalFaker);

                // test cases

                //var testCases = TestWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, trainingId, 0, 0, 10, 5);
                //foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 0 [40,200] -------------------------

                var newCount = 0;
                var penaltyCount = 0;
                var usualCount = generalFaker.Random.Number(40, 200);
                var lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                var testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 200);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 200);
                lengthValue = generalFaker.Random.Number(usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // -----------------------0[40, 200] 0------------------------ -

               newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(30, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = 0;
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [60,100] [40,50] -------------------------
                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(60, 100);
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [40,200] QP -------------------------

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 200);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- 0 [40,50] [60,100] -------------------------

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(30, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = 0;
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [40,200] 0 0 -------------------------

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(30, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [60,100] 0 [40,50] -------------------------

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(usualCount + 1, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(40, 50);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ----------------------- [40,200] 0 QN -------------------------

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = usualCount + newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 200);
                penaltyCount = 0;
                usualCount = newCount;
                lengthValue = generalFaker.Random.Number(usualCount + newCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [40,50] 0 [60,100] -------------------------

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(30, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(newCount + 1, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(40, 50);
                penaltyCount = 0;
                usualCount = generalFaker.Random.Number(60, 100);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [60,100] [40,50] 0 -------------------------

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(30, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = newCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(60, 100);
                penaltyCount = generalFaker.Random.Number(40, 50);
                usualCount = 0;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [270,310] [150,190] [50,70] -------------------------

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, usualCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = usualCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(270, 310);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(50, 70);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [310,350] [100,150] QP -------------------------

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + usualCount + 1, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = generalFaker.Random.Number(100, 150);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [150,190] [50,70] [270,310] -------------------------

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(30, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount + newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount + newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + newCount + 1, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, usualCount + newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + newCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(150, 190);
                penaltyCount = generalFaker.Random.Number(50, 70);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [310,350] QN [100,150] -------------------------

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(30, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(310, 350);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [50,70] QN QP -------------------------

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(30, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, penaltyCount * 2 - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount * 2;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount * 2 + 1, penaltyCount * 3 - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = penaltyCount * 3;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = newCount;
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount * 3 + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [100,150] QN [310,350] -------------------------

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(30, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount + newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = penaltyCount + newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(penaltyCount + newCount + 1, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = newCount;
                usualCount = generalFaker.Random.Number(310, 350);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [100,150] [310,350] QP -------------------------

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(30, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = usualCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(100, 150);
                penaltyCount = generalFaker.Random.Number(310, 350);
                usualCount = penaltyCount;
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                // ------------------------- [50,70] [150,190] [270,310] -------------------------

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(30, newCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + 1, penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(penaltyCount + 1, newCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + 1, usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + 1, newCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + usualCount + 1, usualCount + penaltyCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = usualCount + penaltyCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(usualCount + penaltyCount + 1, newCount + penaltyCount + usualCount - 1);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = newCount + penaltyCount + usualCount;

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }

                newCount = generalFaker.Random.Number(50, 70);
                penaltyCount = generalFaker.Random.Number(150, 190);
                usualCount = generalFaker.Random.Number(270, 310);
                lengthValue = generalFaker.Random.Number(newCount + penaltyCount + usualCount + 1, 1000);

                testCases = TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthValue);
                foreach (var testCase in testCases) { yield return testCase; }
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_CorrectDataThatIsImpossibleToMatchLength_CorrectSelection
        {
            get
            {
                var generalFaker = new Faker();
                var userId = generalFaker.Random.Guid();
                var trainingId = generalFaker.Random.Guid();
                var questionFaker = DataUtils.GetQuestionFaker(generalFaker);

                var testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [],
                    [new(30, 150)],
                    [new(30, 150)],
                    out List<Guid> expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [],
                    [new(20, 29), new(30, 39), new(40, 49)],
                    [new(20, 29)],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [],
                    [new(100), new(15), new(25)],
                    [new(25), new(15)],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [],
                    [new(121), new(120), new(1), new(2)],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [new(30, 150)],
                    [],
                    [ new(30, 150) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(20, 29), new(30, 39), new(40, 49) ],
                    [],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(30, 39), new(40, 49) ],
                    [ new(20, 29) ],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(20, 29) ],
                    [ new(30, 39), new(40, 49) ],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(100), new(15), new(25) ],
                    [],
                    [ new(15), new(25) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(100) ],
                    [ new(15), new(25) ],
                    [ new(15), new(25) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(15), new(25) ],
                    [ new(100) ],
                    [ new(15), new(25) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(121), new(120), new(1), new(2) ],
                    [],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(120), new(1), new(2) ],
                    [ new(121) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(1), new(2) ],
                    [ new(121), new(120) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(121) ],
                    [ new(120), new(1), new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [],
                    [ new(121), new(120) ],
                    [ new(1), new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(15, 125) ],
                    [],
                    [],
                    [ new(15, 125) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(10, 14), new(15, 19), new(20, 25) ],
                    [],
                    [],
                    [ new(10, 14) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(15, 19), new(20, 24) ],
                    [],
                    [ new(20, 29) ],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(10, 14) ],
                    [],
                    [ new(30, 39), new(40, 49) ],
                    [ new(10, 14) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(15, 19), new(20, 24) ],
                    [ new(20, 29) ],
                    [],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(10, 14) ],
                    [ new(30, 39), new(40, 49) ],
                    [],
                    [ new(10, 14) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(15, 19) ],
                    [ new(20, 29) ],
                    [ new(40, 49) ],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(15, 19) ],
                    [ new(40, 49) ],
                    [ new(20, 29) ],
                    [ new(20, 29) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(10, 14) ],
                    [ new(30, 39) ],
                    [ new(40, 49) ],
                    [ new(10, 14) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(50), new(7), new(12) ],
                    [],
                    [],
                    [ new(12), new(7) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(7), new(12) ],
                    [],
                    [ new(100) ],
                    [ new(12), new(7) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(50) ],
                    [],
                    [ new(25), new(15) ],
                    [ new(25), new(15) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(7), new(12) ],
                    [ new(100) ],
                    [],
                    [ new(12), new(7) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(50) ],
                    [ new(15), new(25) ],
                    [],
                    [ new(15), new(25) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(12) ],
                    [ new(15) ],
                    [ new(100) ],
                    [ new(12), new(15) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(12) ],
                    [ new(100) ],
                    [ new(15) ],
                    [ new(12), new(15) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(50) ],
                    [ new(25) ],
                    [ new(15) ],
                    [ new(25), new(15) ],
                    out expectedQuestionsIds),
                    50,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61), new(60), new(1), new(2) ],
                    [],
                    [],
                    [new(1), new(2), /*new(120) */],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61), new(60) ],
                    [],
                    [ new(1), new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61) ],
                    [],
                    [ new(1), new(2), new(120) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2) ],
                    [],
                    [ new(121), new(120) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2), new(60) ],
                    [],
                    [ new(121) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61), new(60) ],
                    [ new(1), new(2) ],
                    [],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61) ],
                    [ new(1), new(2), new(120) ],
                    [],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2) ],
                    [ new(121), new(120) ],
                    [],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2), new(60) ],
                    [ new(121) ],
                    [],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1) ],
                    [ new(2) ],
                    [ new(121),new(120)],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2) ],
                    [ new(121) ],
                    [ new(120) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1), new(2) ],
                    [ new(120) ],
                    [ new(121) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61) ],
                    [ new(120) ],
                    [ new(1), new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(60) ],
                    [ new(121) ],
                    [ new(1), new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(60) ],
                    [ new(1), new(2) ],
                    [ new(121) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61) ],
                    [ new(1), new(2) ],
                    [ new(120) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(1) ],
                    [ new(121), new(120) ],
                    [ new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(61), new(60) ],
                    [ new(1) ],
                    [ new(2) ],
                    [new(1), new(2), /*new(120)*/],
                    out expectedQuestionsIds),
                    100,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }

                testCases = TestCustomQuestionCollectionWithAllStandardFractions(generalFaker, userId, GenerateQuestionsUsingRules(generalFaker, questionFaker,
                    [ new(50) ],
                    [],
                    [ new(99) ],
                    [ new(99) ],
                    out expectedQuestionsIds),
                    5,
                    expectedQuestionsIds);
                foreach (var testCase in testCases) { yield return testCase; }
            }
        }

        public static IEnumerable<object[]> UpdateQuestionStatusAsync_CorrectData_CorrectResult
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    false, default(bool), default(int), default(int),
                    false,
                    true, Restrictions.MaxQuestionRating, 0
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    false, default(bool), default(int), default(int),
                    true,
                    false, Restrictions.InitialQuestionRating-1, 0
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, 0,
                    false,
                    false, Restrictions.MaxQuestionRating, 1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, 0,
                    true,
                    false, 0, 0
                };

                var penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, penaltyPoints,
                    false,
                    false, Restrictions.MaxQuestionRating, penaltyPoints+1
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, penaltyPoints,
                    true,
                    false, 0, penaltyPoints-1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, int.MaxValue,
                    false,
                    false, Restrictions.MaxQuestionRating, int.MaxValue
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, 0, int.MaxValue,
                    true,
                    false, 0, int.MaxValue-1
                };

                //t f Constants.MinQuestionRating
                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, 0,
                    false,
                    false, Restrictions.MaxQuestionRating, 1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, 0,
                    true,
                    false, Restrictions.MinQuestionRating, 0
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, penaltyPoints,
                    false,
                    false, Restrictions.MaxQuestionRating, penaltyPoints+1
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, penaltyPoints,
                    true,
                    false, Restrictions.MinQuestionRating, penaltyPoints-1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, int.MaxValue,
                    false,
                    false, Restrictions.MaxQuestionRating, int.MaxValue
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MinQuestionRating, int.MaxValue,
                    true,
                    false, Restrictions.MinQuestionRating, int.MaxValue-1
                };

                //t f [Constants.MinQuestionRating + 1, Constants.MaxQuestionRating - 1]
                var rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, 0,
                    false,
                    false, Restrictions.MaxQuestionRating, 1
                };

                rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, 0,
                    true,
                    false, rating-1, 0
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);
                rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, penaltyPoints,
                    false,
                    false, Restrictions.MaxQuestionRating, penaltyPoints+1
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);
                rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, penaltyPoints,
                    true,
                    false, rating, penaltyPoints-1
                };

                rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, int.MaxValue,
                    false,
                    false, Restrictions.MaxQuestionRating, int.MaxValue
                };

                rating = faker.Random.Number(Restrictions.MinQuestionRating + 1, Restrictions.MaxQuestionRating - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, rating, int.MaxValue,
                    true,
                    false, rating, int.MaxValue-1
                };

                //t f Constants.MaxQuestionRating

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, 0,
                    false,
                    false, Restrictions.MaxQuestionRating, 1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, 0,
                    true,
                    false, Restrictions.MaxQuestionRating-1, 0
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, penaltyPoints,
                    false,
                    false, Restrictions.MaxQuestionRating, penaltyPoints+1
                };

                penaltyPoints = faker.Random.Number(1, int.MaxValue - 1);

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, penaltyPoints,
                    true,
                    false, Restrictions.MaxQuestionRating, penaltyPoints-1
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, int.MaxValue,
                    false,
                    false, Restrictions.MaxQuestionRating, int.MaxValue
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, false, Restrictions.MaxQuestionRating, int.MaxValue,
                    true,
                    false, Restrictions.MaxQuestionRating, int.MaxValue-1
                };

                //t f Constants.MaxQuestionRating

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, true, Restrictions.MaxQuestionRating, 0,
                    true,
                    false, Restrictions.MaxQuestionRating-1, 0
                };

                yield return new object[]
                {
                    faker.Random.Guid(), faker.Random.Guid(), faker.Random.Number(0,int.MaxValue),
                    true, true, Restrictions.MaxQuestionRating, 0,
                    false,
                    true, Restrictions.MaxQuestionRating, 0
                };
            }
        }

        public static IEnumerable<object[]> UpdateQuestionStatusAsync_IncorrectArguments_ArgumentException
        {
            get
            {
                var faker = new Faker();
                yield return new object[]
                {
                    Guid.Empty,
                    new TrainingResult
                    {
                         AnswerTimeMilliseconds = faker.Random.Number(1,int.MaxValue),
                         IsAnswerCorrect = faker.Random.Bool(),
                         QuestionId = faker.Random.Guid()
                    }
                };
                yield return new object[]
                {
                    faker.Random.Guid(),
                    new TrainingResult
                    {
                         AnswerTimeMilliseconds = 0,
                         IsAnswerCorrect = faker.Random.Bool(),
                         QuestionId = faker.Random.Guid()
                    }
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    new TrainingResult
                    {
                         AnswerTimeMilliseconds = faker.Random.Int(int.MinValue,-1),
                         IsAnswerCorrect = faker.Random.Bool(),
                         QuestionId = faker.Random.Guid()
                    }
                };

                yield return new object[]
                {
                    faker.Random.Guid(),
                    new TrainingResult
                    {
                         AnswerTimeMilliseconds = faker.Random.Number(1,int.MaxValue),
                         IsAnswerCorrect = faker.Random.Bool(),
                         QuestionId = Guid.Empty
                    }
                };
            }
        }

        private static List<GetQuestionsForTrainingResult> GenerateQuestionsUsingRules(Faker generalFaker, Faker<GetQuestionsForTrainingResult> questionFaker, List<CustomQuestionRule> rulesForNewQuestions, List<CustomQuestionRule> rulesForPenaltyQuestions, List<CustomQuestionRule> rulesForUsualQuestions, 
            List<CustomQuestionRule> expectedRules, out List<Guid> expectedQuestionsGuids)
        {
            List<GetQuestionsForTrainingResult> result = [];

            expectedQuestionsGuids = [];

            foreach(var rule in rulesForNewQuestions)
            {
                GetQuestionsForTrainingResult question = GenerateQuestion(generalFaker, questionFaker, "default,new", rule);
                if (question.Id!=null && expectedRules.Any(rule.Equals))
                {
                    expectedQuestionsGuids.Add(question.Id.Value);
                }
                result.Add(question);
            }

            foreach (var rule in rulesForPenaltyQuestions)
            {
                GetQuestionsForTrainingResult question = GenerateQuestion(generalFaker, questionFaker, "default,penalty", rule);
                if (question.Id != null && expectedRules.Any(rule.Equals))
                {
                    expectedQuestionsGuids.Add(question.Id.Value);
                }
                result.Add(question);
            }

            foreach (var rule in rulesForUsualQuestions)
            {
                GetQuestionsForTrainingResult question = GenerateQuestion(generalFaker, questionFaker, "default,usual", rule);
                if (question.Id != null && expectedRules.Any(rule.Equals))
                {
                    expectedQuestionsGuids.Add(question.Id.Value);
                }
                result.Add(question);
            }

            DataUtils.Shuffle(result, generalFaker);

            return result;
        }

        private static GetQuestionsForTrainingResult GenerateQuestion(Faker generalFaker, Faker<GetQuestionsForTrainingResult> questionFaker, string fakerRuleSet, CustomQuestionRule questionRule)
        {
            GetQuestionsForTrainingResult question = questionFaker.Generate(fakerRuleSet);
            if (questionRule.IsExactValue)
            {
                question.QuestionActualTrainingTimeSeconds = questionRule.ExactValue;
            }
            else
            {
                question.QuestionActualTrainingTimeSeconds = generalFaker.Random.Number(questionRule.MinValue, questionRule.MaxValue);
            } 
            return question;
        }

        private static List<(double newQuestionsFraction, double penaltyQuestionsFraction)> GetAllStandardFractions(Faker generalFaker)
        {
            return
            [
                (0,0),
                (0,generalFaker.Random.Double(0.1, 0.4)),
                (0, generalFaker.Random.Double(0.6, 0.9)),
                (0,1),
                (generalFaker.Random.Double(0.1, 0.4), 0),
                (generalFaker.Random.Double(0.1, 0.4), generalFaker.Random.Double(0.1, 0.4)),
                (generalFaker.Random.Double(0.1, 0.25), generalFaker.Random.Double(0.6, 0.7)),
                (generalFaker.Random.Double(0.6, 0.9), 0),
                (generalFaker.Random.Double(0.6, 0.7), generalFaker.Random.Double(0.1, 0.25)),
                (1,0)
            ];
        }

        private static IEnumerable<object[]> TestRandomQuestionCollectionsWithAllStandardFractionsAndLengthTypes(Faker generalFaker, Faker<GetQuestionsForTrainingResult> questionFaker, Guid userId, int newCount, int penaltyCount, int usualCount, int lengthValue)
        {
            List<TrainingLengthType> lengthTypes = [TrainingLengthType.QuestionsCount, TrainingLengthType.Time];
            List<(double newQuestionsFraction, double penaltyQuestionsFraction)> allStandardFractions = GetAllStandardFractions(generalFaker);

            foreach(var lengthType in lengthTypes)
            {
                foreach(var (newQuestionsFraction, penaltyQuestionsFraction) in allStandardFractions)
                {
                    yield return GenerateQuestionData(generalFaker, questionFaker, userId, newCount, penaltyCount, usualCount, lengthType, lengthValue, newQuestionsFraction, penaltyQuestionsFraction);
                }
            }
        }

        private static IEnumerable<object[]> TestCustomQuestionCollectionWithAllStandardFractions(Faker generalFaker, Guid userId,
            List<GetQuestionsForTrainingResult> questions, int lengthValue, List<Guid> expectedQuestionsGuids)
        {
            List<(double newQuestionsFraction, double penaltyQuestionsFraction)> allStandardFractions = GetAllStandardFractions(generalFaker);
            foreach (var (newQuestionsFraction, penaltyQuestionsFraction) in allStandardFractions)
            {
                yield return new object[]
                {
                    userId,
                    generalFaker.Make(generalFaker.Random.Number(1,10),generalFaker.Random.Guid),
                    lengthValue,
                    newQuestionsFraction,
                    penaltyQuestionsFraction,
                    questions,
                    expectedQuestionsGuids
                };
            }
        }

        private static object[] GenerateQuestionData(Faker generalFaker, Faker<GetQuestionsForTrainingResult> questionFaker, Guid userId, int newCount, int penaltyCount, int usualCount, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction)
        {
            var questions = new List<GetQuestionsForTrainingResult>();
            if (lengthType == TrainingLengthType.QuestionsCount)
            {
                questions.AddRange(questionFaker.Generate(newCount, "default,new"));
                questions.AddRange(questionFaker.Generate(penaltyCount, "default,penalty"));
                questions.AddRange(questionFaker.Generate(usualCount, "default,usual"));
            }
            else if (lengthType == TrainingLengthType.Time)
            {
                questions.AddRange(CreateQuestionsCollectionByTime(generalFaker, questionFaker, "default,new", newCount));
                questions.AddRange(CreateQuestionsCollectionByTime(generalFaker, questionFaker, "default,penalty", penaltyCount));
                questions.AddRange(CreateQuestionsCollectionByTime(generalFaker, questionFaker, "default,usual", usualCount));
            }

            DataUtils.Shuffle(questions, generalFaker);

            return
            [
                userId,
                generalFaker.Make(generalFaker.Random.Number(1,10),generalFaker.Random.Guid),
                lengthType,
                lengthValue,
                newQuestionsFraction,
                penaltyQuestionsFraction,
                questions
            ];
        }

        private static List<GetQuestionsForTrainingResult> CreateQuestionsCollectionByTime(Faker generalFaker, Faker<GetQuestionsForTrainingResult> questionFaker, string ruleSet, int fullTime)
        {
            List<GetQuestionsForTrainingResult> questions = [];
            const int recommendedMinimumCountOfQuestions = 50;
            int minimumCountOfQuestions = fullTime > recommendedMinimumCountOfQuestions ? recommendedMinimumCountOfQuestions : fullTime;
            int currentTimeLength = 0;
            while (currentTimeLength < fullTime)
            {
                var question = questionFaker.Generate(ruleSet);
                var nextTime = generalFaker.Random.Number(1, fullTime / minimumCountOfQuestions);
                if (nextTime + currentTimeLength > fullTime)
                    nextTime = fullTime - currentTimeLength;
                question.QuestionActualTrainingTimeSeconds = nextTime;
                questions.Add(question);
                currentTimeLength += nextTime;
            }
            return questions;
        }
    }
}
