using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using FluentAssertions;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests
{
    [TestClass]
    public class GetQuestionsForTrainingResultExtensionsUnitTests
    {
        [TestMethod]
        [DynamicData(nameof(GetQuestionsForTrainingResultExtensionsUnitTestsDataSource.FullActualTrainingTimeSeconds_CorrectQuestion_CorrectResult), typeof(GetQuestionsForTrainingResultExtensionsUnitTestsDataSource))]
        public void FullActualTrainingTimeSeconds_CorrectQuestion_CorrectResult(Models.GetQuestionsForTrainingResult question, int expectedResult)
        {
            //Arrange
            //Act
            int actualResult = question.FullActualTrainingTimeSeconds();
            //Assert
            actualResult.Should().Be(expectedResult);
        }

        [TestMethod]
        public void FullActualTrainingTimeSeconds_NullQuestion_ArgumentNullException()
        {
            //Arrange
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Models.Question question = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            //Act
#pragma warning disable CS8604 // Possible null reference argument.
            Action act = () => { question.FullActualTrainingTimeSeconds(); };
#pragma warning restore CS8604 // Possible null reference argument.
            //Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
