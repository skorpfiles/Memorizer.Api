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
        [DataRow(-0.1, 0.1)]
        [DataRow(-0.3, -0.2)]
        [DataRow(-1.0, -0.9)]
        public async Task SelectQuestionsForTrainingAsync_NegativeValues_IncorrectTrainingOptionsException(double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object);
            var userId = Guid.NewGuid();
            var trainingId = Guid.NewGuid();
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().ThrowAsync<IncorrectTrainingOptionsException>().WithMessage("New questions fraction and penalty questions fraction cannot be negative.").ConfigureAwait(false);
        }
    }
}