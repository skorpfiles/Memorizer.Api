using FluentAssertions;
using Moq;
using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
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
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException), typeof(TrainingLogicTestDataSource))]
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
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions), typeof(TrainingLogicTestDataSource))]
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

        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectResult(double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue, List<Question> allQuestions)
        {

        }

    }
}