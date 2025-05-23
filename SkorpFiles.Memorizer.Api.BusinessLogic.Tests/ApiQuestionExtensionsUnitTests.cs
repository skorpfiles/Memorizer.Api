﻿using FluentAssertions;
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
        [DynamicData(nameof(ApiQuestionExtensionsUnitTestsDataSource.FullActualTrainingTimeSeconds_CorrectQuestion_CorrectResult),typeof(ApiQuestionExtensionsUnitTestsDataSource))]
        public void FullActualTrainingTimeSeconds_CorrectQuestion_CorrectResult(Models.Question question, int expectedResult)
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
