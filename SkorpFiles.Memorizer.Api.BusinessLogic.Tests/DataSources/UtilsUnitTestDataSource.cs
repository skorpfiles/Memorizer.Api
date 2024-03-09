using Bogus;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class UtilsUnitTestDataSource
    {
        static UtilsUnitTestDataSource()
        {
            Randomizer.Seed = new Random(44472135);
        }

        public static IEnumerable<object[]> FindBestQuestionsTimesCombination_CorrectParameters_CorrectResult
        {
            get
            {
                var generalFaker = new Faker();
                var userQuestionStatusFaker = DataUtils.GetUserQuestionStatusFaker(generalFaker);
                var questionFaker = DataUtils.GetQuestionFaker(userQuestionStatusFaker);

                yield return new object[]
                {
                    new List<Question>(),
                    0,
                    new List<Guid>()
                };

                yield return new object[]
                {
                    new List<Question>(),
                    generalFaker.Random.Number(1,int.MaxValue),
                    new List<Guid>()
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(30, 150)],
                        [],
                        out _
                        ),
                    0,
                    new List<Guid>()
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(30, 150)],
                        [],
                        out List<Guid> expectedQuestionsGuids
                        ),
                    5,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(30)],
                        [new(30)],
                        out expectedQuestionsGuids
                        ),
                    30,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(30, 140)],
                        [new(30, 140)],
                        out expectedQuestionsGuids
                        ),
                    150,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(1), new(2), new(120), new(121)],
                        [new(1), new(2)],
                        out expectedQuestionsGuids
                        ),
                    100,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(1), new(2), new(3), new(6), new(7), new(120), new(121)],
                        [new(2), new(3)],
                        out expectedQuestionsGuids
                        ),
                    5,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(1), new(2), new(3), new(6), new(120), new(121)],
                        [new(1), new(6)],
                        out expectedQuestionsGuids
                        ),
                    7,
                    expectedQuestionsGuids
                };

                yield return new object[]
                {
                    GenerateQuestionsUsingRules(generalFaker,questionFaker,
                        [new(1), new(3), new(6), new(120), new(121)],
                        [new(1), new(6)],
                        out expectedQuestionsGuids
                        ),
                    8,
                    expectedQuestionsGuids
                };
            }
        }

        public static IEnumerable<object[]> FindBestQuestionsTimesCombination_IncorrectTarget_ThrowArgumentExceptionOnTarget
        {
            get
            {
                var generalFaker = new Faker();
                var userQuestionStatusFaker = DataUtils.GetUserQuestionStatusFaker(generalFaker);
                var questionFaker = DataUtils.GetQuestionFaker(userQuestionStatusFaker);

                yield return new object[]
                {
                    new List<Question>(),
                    generalFaker.Random.Number(int.MinValue, -1)
                };

                yield return new object[]
                {
                    questionFaker.GenerateBetween(20,100,"default,usual"),
                    generalFaker.Random.Number(int.MinValue, -1)
                };
            }
        }

        private static List<Question> GenerateQuestionsUsingRules(Faker generalFaker, Faker<Question> questionFaker, List<CustomQuestionRule> rulesForUsualQuestions,
            List<CustomQuestionRule> expectedRules, out List<Guid> expectedQuestionsGuids)
        {
            List<Question> result = [];

            expectedQuestionsGuids = [];

            foreach (var rule in rulesForUsualQuestions)
            {
                Question question = GenerateQuestion(generalFaker, questionFaker, "default,usual", rule);
                if (question.Id != null && expectedRules.Any(rule.Equals))
                {
                    expectedQuestionsGuids.Add(question.Id.Value);
                }
                result.Add(question);
            }

            DataUtils.Shuffle(result, generalFaker);

            return result;
        }

        private static Question GenerateQuestion(Faker generalFaker, Faker<Question> questionFaker, string fakerRuleSet, CustomQuestionRule questionRule)
        {
            Question question = questionFaker.Generate(fakerRuleSet);
            if (questionRule.IsExactValue)
            {
                question.EstimatedTrainingTimeSeconds = questionRule.ExactValue;
            }
            else
            {
                question.EstimatedTrainingTimeSeconds = generalFaker.Random.Number(questionRule.MinValue, questionRule.MaxValue);
            }
            return question;
        }
    }
}
