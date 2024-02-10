using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System.Diagnostics.CodeAnalysis;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests
{
    [TestClass]
    public class TrainingLogicUnitTests
    {
        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException(Guid userId, Guid trainingId, double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue, string expectedErrorMessage)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object);
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
        public async Task SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions(Guid userId, Guid trainingId, double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object);
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;
            trainingOptions.LengthValue = lengthValue;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().NotThrowAsync();
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_NoQuestions_EmptyResult), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_NoQuestions_EmptyResult(Guid userId, Guid trainingId, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(new List<Question>());

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, new TrainingOptions
            { 
                LengthType = lengthType, 
                LengthValue = lengthValue, 
                NewQuestionsFraction = newQuestionsFraction, 
                PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction 
            }).ConfigureAwait(false);

            //Assert
            actualResult.Should().BeEmpty();
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData),typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_AllQuestionsAreDistinct(Guid userId, Guid trainingId, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionFraction, List<Models.Question> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            var trainingOptions = new TrainingOptions
            {
                LengthType = lengthType,
                LengthValue = lengthValue,
                NewQuestionsFraction = newQuestionsFraction,
                PrioritizedPenaltyQuestionsFraction = penaltyQuestionFraction
            };

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, trainingOptions).ConfigureAwait(false);

            //Assert
            actualResult.Should().BeEquivalentTo(actualResult.Distinct(new GuidComparer()));
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectFullLengthDelta(Guid userId, Guid trainingId, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.Question> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert

            int fullLengthOfAllQuestions;
            int fullLengthOfActualResult;
            switch (lengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    fullLengthOfAllQuestions = allQuestions.Count;
                    fullLengthOfActualResult = actualResult.Count();
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    fullLengthOfAllQuestions = allQuestions.Sum(q => q.EstimatedTrainingTimeSeconds);
                    fullLengthOfActualResult = actualResult.Sum(q => q.EstimatedTrainingTimeSeconds);
                    break;
                default:
                    throw new FluentAssertions.Execution.AssertionFailedException("There is no such length type.");
            }

            if (fullLengthOfAllQuestions > lengthValue)
            {
                int fractionValue = Convert.ToInt32(Math.Ceiling(lengthValue * Constants.AllowableErrorFraction));
                fullLengthOfActualResult.Should().BeInRange(lengthValue - fractionValue, lengthValue + fractionValue);
            }
            else
            {
                fullLengthOfActualResult.Should().Be(fullLengthOfAllQuestions);
            }
        }

        //PRINCIPLES OF THE ALGORYTHM WE ARE TESTING:
        //Firstly, it chooses NEW questions until the percentage are reached OR the questions run out.
        //Secondly, it chooses questions with PENALTY POINTS until the percentage are reached OR the questions run out.
        //Finally, it fills the whole remaining room with usual questions while it's possible (including penalty questions).

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectNewDeltas(Guid userId, Guid trainingId, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.Question> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId,trainingId, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert

            int lengthOfAllNewQuestions, lengthOfAllQuestionsExceptNew;
            int lengthOfNewQuestionsInActualResult;
            int expectedLengthValue;

            switch (lengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    lengthOfAllNewQuestions = allQuestions.Where(q=>q.MyStatus!.IsNew).Count();
                    lengthOfAllQuestionsExceptNew = allQuestions.Count() - lengthOfAllNewQuestions;
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Count();
                    expectedLengthValue = lengthValue < allQuestions.Count ? lengthValue : allQuestions.Count;
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    lengthOfAllNewQuestions = allQuestions.Where(q => q.MyStatus!.IsNew).Sum(q => q.EstimatedTrainingTimeSeconds);
                    lengthOfAllQuestionsExceptNew = allQuestions.Sum(q=>q.EstimatedTrainingTimeSeconds) - lengthOfAllNewQuestions;
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Sum(q => q.EstimatedTrainingTimeSeconds);
                    int allQuestionsTimeSum = allQuestions.Sum(q => q.EstimatedTrainingTimeSeconds);
                    expectedLengthValue = lengthValue < allQuestionsTimeSum ? lengthValue : allQuestionsTimeSum;
                    break;
                default:
                    throw new FluentAssertions.Execution.AssertionFailedException("There is no such length type.");
            }

            double expectedLengthOfNewQuestions = expectedLengthValue * newQuestionsFraction;
            //Do we have probability to fill all the percent by NEW questions?
            bool canCompleteByNew = lengthOfAllNewQuestions >= expectedLengthOfNewQuestions - expectedLengthOfNewQuestions * Constants.AllowableErrorFraction;
            //Do we need to fill the list by new questions due to lack of other ones?
            bool needCompleteByNew = lengthOfAllQuestionsExceptNew + expectedLengthOfNewQuestions < expectedLengthValue;

            //If we need to fill the list by new questions, check whether we fill it in correct volume.
            if (needCompleteByNew)
            {
                double lengthOfNewQuestionsDueToNecessity = expectedLengthValue - lengthOfAllQuestionsExceptNew;
                lengthOfNewQuestionsInActualResult.Should().BeInRange(Convert.ToInt32(Math.Round(lengthOfNewQuestionsDueToNecessity - lengthOfNewQuestionsDueToNecessity * Constants.AllowableErrorFraction)),
                    Convert.ToInt32(Math.Round(lengthOfNewQuestionsDueToNecessity + lengthOfNewQuestionsDueToNecessity * Constants.AllowableErrorFraction)));
            }
            //If we can't fill the percent, check whether all the new questions are present in the actual list. If we can - check whether the percent is followed.
            else if (canCompleteByNew)
            {
                lengthOfNewQuestionsInActualResult.Should().BeInRange(Convert.ToInt32(Math.Round(expectedLengthOfNewQuestions - expectedLengthOfNewQuestions * Constants.AllowableErrorFraction)),
                    Convert.ToInt32(Math.Round(expectedLengthOfNewQuestions + expectedLengthOfNewQuestions * Constants.AllowableErrorFraction)));
            }
            else
            {
                var expectedNewQuestions = allQuestions.Where(q => q.MyStatus!.IsNew).ToList();
                var actualNewQuestions = actualResult.Where(q=>q.MyStatus!.IsNew).ToList();
                if (expectedNewQuestions.Any())
                {
                    actualNewQuestions.Should().BeEquivalentTo(expectedNewQuestions);
                }
                else
                {
                    actualNewQuestions.Should().BeEmpty();
                }
            }
        }
        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectPenaltyDeltas(Guid userId, Guid trainingId, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.Question> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert

            int lengthOfAllPenaltyQuestions;
            int lengthOfPenaltyQuestionsInActualResult;
            int expectedLengthValue;

            switch (lengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    lengthOfAllPenaltyQuestions = allQuestions.Where(q => q.MyStatus!.PenaltyPoints > 0).Count();
                    lengthOfPenaltyQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).Count();
                    expectedLengthValue = lengthValue < allQuestions.Count ? lengthValue : allQuestions.Count;
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    lengthOfAllPenaltyQuestions = allQuestions.Where(q => q.MyStatus!.PenaltyPoints > 0).Sum(q => q.EstimatedTrainingTimeSeconds);
                    lengthOfPenaltyQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).Sum(q => q.EstimatedTrainingTimeSeconds);
                    int allQuestionsTimeSum = allQuestions.Sum(q => q.EstimatedTrainingTimeSeconds);
                    expectedLengthValue = lengthValue < allQuestionsTimeSum ? lengthValue : allQuestionsTimeSum;
                    break;
                default:
                    throw new FluentAssertions.Execution.AssertionFailedException("There is no such length type.");
            }

            double expectedLengthOfPenaltyQuestions = expectedLengthValue * penaltyQuestionsFraction;
            //Do we have probability to fill all the percent by PENALTY questions?
            bool canCompleteByPenalty = lengthOfAllPenaltyQuestions >= expectedLengthOfPenaltyQuestions - expectedLengthOfPenaltyQuestions * Constants.AllowableErrorFraction;

            //If we can't fill the percent, check whether all the penalty questions are present in the actual list. If we can - check whether the bottom line of the allowable percent is followed.
            //We don't check the top line of the allowable percent, because the penalty questions may be chosen as usual questions from the basic list.
            if (canCompleteByPenalty)
            {
                lengthOfPenaltyQuestionsInActualResult.Should().BeGreaterThanOrEqualTo(Convert.ToInt32(Math.Round(expectedLengthOfPenaltyQuestions - expectedLengthOfPenaltyQuestions * Constants.AllowableErrorFraction)));
            }
            else
            {
                var expectedPenaltyQuestions = allQuestions.Where(q => q.MyStatus!.PenaltyPoints>0).ToList();
                var actualPenaltyQuestions = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).ToList();

                if (expectedPenaltyQuestions.Any())
                {
                    actualPenaltyQuestions.Should().BeEquivalentTo(expectedPenaltyQuestions);
                }
                else
                {
                    actualPenaltyQuestions.Should().BeEmpty();
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectDataThatIsImpossibleToMatchLength_CorrectSelection),typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectDataThatIsImpossibleToMatchLength_CorrectSelection(Guid userId, Guid trainingId,
            int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.Question> allQuestions, List<Guid> expectedQuestionIds)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, trainingId, new TrainingOptions { LengthType = TrainingLengthType.Time, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert
            actualResult.Select(q => q.Id).ToList().Should().BeEquivalentTo(expectedQuestionIds);
        }
    }

    class GuidComparer : IEqualityComparer<Models.Question>
    {
        public bool Equals(Question? x, Question? y)
        {
            return x?.Id.Equals(y?.Id) ?? x == y;
        }

        public int GetHashCode([DisallowNull] Question obj)
        {
            return obj.GetHashCode();
        }
    }
}