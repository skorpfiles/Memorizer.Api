using FluentAssertions;
using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests
{
    [TestClass]
    public class UtilsUnitTests
    {
        [TestMethod]
        [DynamicData(nameof(UtilsUnitTestDataSource.FindBestQuestionsTimesCombination_CorrectParameters_CorrectResult), typeof(UtilsUnitTestDataSource))]
        public void FindBestQuestionsTimesCombination_CorrectParameters_CorrectResult(List<GetQuestionsForTrainingResult> sourceList, int target, List<Guid> expectedGuids)
        {
            //Arrange
            //Act
            var actualResult = Utils.FindBestQuestionsTimesCombination(sourceList, target);

            //Assert
            actualResult.Select(q=>q.Id).ToList().Should().BeEquivalentTo(expectedGuids);
        }

        [TestMethod]
        public void FindBestQuestionsTimesCombination_IncorrectList_ThrowArgumentNullExceptionOnList()
        {
            //Arrange
            //Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action act = () => Utils.FindBestQuestionsTimesCombination(null, 5);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            //Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("questions");
        }

        [TestMethod]
        [DynamicData(nameof(UtilsUnitTestDataSource.FindBestQuestionsTimesCombination_IncorrectTarget_ThrowArgumentExceptionOnTarget), typeof(UtilsUnitTestDataSource))]
        public void FindBestQuestionsTimesCombination_IncorrectTarget_ThrowArgumentExceptionOnTarget(List<GetQuestionsForTrainingResult> sourceList, int target)
        {
            //Arrange
            //Act
            Action act = () => Utils.FindBestQuestionsTimesCombination(sourceList, target);
            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DynamicData(nameof(UtilsUnitTestDataSource.SoftmaxSample_CorrectParameters_CorrectResult), typeof(UtilsUnitTestDataSource))]
        public void SoftmaxSample_CorrectParameters_CorrectResult(Dictionary<Guid, int> sourceDictionary, Random random, double temperature)
        {
            //Arrange
            Func<int, int> weightCalculationFunc = (value) => value;

            //Act
            var actualResult = Utils.SoftmaxSample(sourceDictionary, weightCalculationFunc, random, temperature);

            //Assert
            actualResult.Should().NotBeNull();
            sourceDictionary.Should().Contain(actualResult.Value);
        }

        [TestMethod]
        public void FindBestQuestionsTimesCombination_NullDictionary_ThrowArgumentNullExceptionOnList()
        {
            //Arrange
            Func<int, int> weightCalculationFunc = (value) => value;

            //Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Action act = () => Utils.SoftmaxSample((IDictionary<int, int>)null, weightCalculationFunc, new Random(), 5);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            //Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("elements");
        }

        [TestMethod]
        public void SoftmaxSample_EmptyList_ReturnsNull()
        {
            //Arrange
            Func<int, int> weightCalculationFunc = (value) => value;

            //Act
            var actualResult = Utils.SoftmaxSample(new Dictionary<int, int>(), weightCalculationFunc, new Random(), 5);

            //Assert
            actualResult.Should().BeNull();
        }
    }
}
