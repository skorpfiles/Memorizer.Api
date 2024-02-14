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
        public void FindBestQuestionsTimesCombination_CorrectParameters_CorrectResult(List<Question> sourceList, int target, List<Guid> expectedGuids)
        {
            //Arrange
            //Act
            var actualResult = Utils.FindBestQuestionsTimesCombination(sourceList, target);

            //Assert
            actualResult.Select(q=>q.Id).ToList().Should().BeEquivalentTo(expectedGuids);
        }
    }
}
