using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class GetQuestionsForTrainingResultExtensionsUnitTestsDataSource
    {
        static GetQuestionsForTrainingResultExtensionsUnitTestsDataSource()
        {
            Randomizer.Seed = new Random(44472138);
        }

        public static IEnumerable<object[]> FullActualTrainingTimeSeconds_CorrectQuestion_CorrectResult
        {
            get
            {
                var generalFaker = new Faker();
                var questionFaker = DataUtils.GetQuestionFaker(generalFaker);

                var question = questionFaker.Generate("default,usual");
                var expectedResult = question.QuestionActualTrainingTimeSeconds;

                yield return new object[]
                {
                    question,
                    expectedResult
                };

                question = questionFaker.Generate("default,penalty");
                expectedResult = question.QuestionActualTrainingTimeSeconds;

                yield return new object[]
                {
                    question,
                    expectedResult
                };

                question = questionFaker.Generate("default,new");
                expectedResult = (int)Math.Round(question.QuestionActualTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);

                yield return new object[]
                {
                    question,
                    expectedResult
                };
            }
        }
    }
}
