using FluentAssertions;
using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests
{
    [TestClass]
    public class ApiQuestionExtensionsUnitTests
    {
        [TestMethod]
        [DynamicData(nameof(ApiQuestionExtensionsUnitTestsDataSource.FullEstimatedTrainingTimeSeconds_CorrectQuestion_CorrectResult),typeof(ApiQuestionExtensionsUnitTestsDataSource))]
        public void FullEstimatedTrainingTimeSeconds_CorrectQuestion_CorrectResult(Models.Question question, int expectedResult)
        {
            //Arrange
            //Act
            int actualResult = question.FullEstimatedTrainingTimeSeconds();
            //Assert
            actualResult.Should().Be(expectedResult);
        }

        [TestMethod]
        public void FullEstimatedTrainingTimeSeconds_NullQuestion_ArgumentNullException()
        {
            //Arrange
            Models.Question question = null;
            //Act
            Action act = () => { question.FullEstimatedTrainingTimeSeconds(); };
            //Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
