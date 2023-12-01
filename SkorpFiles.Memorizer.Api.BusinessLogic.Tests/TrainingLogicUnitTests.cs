using FluentAssertions;
using Moq;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests
{
    [TestClass]
    public class TrainingLogicUnitTests
    {
        [TestMethod]
        [DataRow(-0.1234, 0.1234, 5, Constants.NegativeFractionsMessage)]
        [DataRow(-0.3456, -0.2345, 5, Constants.NegativeFractionsMessage)]
        [DataRow(-1.0, -0.9876, 5, Constants.NegativeFractionsMessage)]
        [DataRow(2.5678, 1.4567, 5, Constants.SumOfFractionsCannotBeMoreThan1Message)]
        [DataRow(0.1234, 1, 5, Constants.SumOfFractionsCannotBeMoreThan1Message)]
        [DataRow(1, 0.2345, 5, Constants.SumOfFractionsCannotBeMoreThan1Message)]
        [DataRow(0.567, 0.678, 5, Constants.SumOfFractionsCannotBeMoreThan1Message)]
        [DataRow(0.789, 0.456, 5, Constants.SumOfFractionsCannotBeMoreThan1Message)]
        [DataRow(0.1,0.2,0,Constants.NonPositiveLengthValueMessage)]
        [DataRow(0.1,0.2,-5, Constants.NonPositiveLengthValueMessage)]
        public async Task SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException(double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue, string expectedErrorMessage)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object);
            var userId = Guid.NewGuid();
            var trainingId = Guid.NewGuid();
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;
            trainingOptions.LengthValue = lengthValue;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().ThrowAsync<IncorrectTrainingOptionsException>().WithMessage(expectedErrorMessage).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow(0,0,5)]
        [DataRow(0,0.234,5)]
        [DataRow(0.345,0,5)]
        [DataRow(0, 1, 5)]
        [DataRow(1,0, 5)]
        [DataRow(0.123,0.3456,5)]
        [DataRow(0.1,0.9,5)]
        [DataRow(0.8, 0.2,5)]
        public async Task SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions(double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object);
            var userId = Guid.NewGuid();
            var trainingId = Guid.NewGuid();
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;
            trainingOptions.LengthValue = lengthValue;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().NotThrowAsync();
        }

        //public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectResult()
    }
}