using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class ApiQuestionExtensionsUnitTestsDataSource
    {
        static ApiQuestionExtensionsUnitTestsDataSource()
        {
            Randomizer.Seed = new Random(44472138);
        }

        public static IEnumerable<object[]> FullEstimatedTrainingTimeSeconds_CorrectQuestion_CorrectResult
        {
            get
            {
                var generalFaker = new Faker();
                var userQuestionStatusFaker = DataUtils.GetUserQuestionStatusFaker(generalFaker);
                var questionFaker = DataUtils.GetQuestionFaker(userQuestionStatusFaker);

                var question = questionFaker.Generate("default,usual");
                var expectedResult = question.EstimatedTrainingTimeSeconds;

                yield return new object[]
                {
                    question,
                    expectedResult
                };

                question = questionFaker.Generate("default,penalty");
                expectedResult = question.EstimatedTrainingTimeSeconds;

                yield return new object[]
                {
                    question,
                    expectedResult
                };

                question = questionFaker.Generate("default,new");
                expectedResult = (int)Math.Round(question.EstimatedTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);

                yield return new object[]
                {
                    question,
                    expectedResult
                };
            }
        }
    }
}
