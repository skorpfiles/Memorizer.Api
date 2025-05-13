using AutoMapper;
using FluentAssertions;
using Moq;
using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.BusinessLogic.Mapping;
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
        protected IMapper Mapper { get; private set; }

        public TrainingLogicUnitTests()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });

            Mapper = mapperConfig.CreateMapper();
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException(Guid userId, IEnumerable<Guid> questionnairesIds, double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue, string expectedErrorMessage)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object, Mapper);
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;
            trainingOptions.LengthValue = lengthValue;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().ThrowAsync<IncorrectTrainingOptionsException>().WithMessage(expectedErrorMessage).ConfigureAwait(false);
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions(Guid userId, IEnumerable<Guid> questionnairesIds, double newQuestionsFraction, double prioritizedPenaltyQuestionsFraction, int lengthValue)
        {
            //Arrange
            var trainingLogic = new TrainingLogic(new Mock<ITrainingRepository>().Object, Mapper);
            var trainingOptions = new Mock<TrainingOptions>().Object;
            trainingOptions.NewQuestionsFraction = newQuestionsFraction;
            trainingOptions.PrioritizedPenaltyQuestionsFraction = prioritizedPenaltyQuestionsFraction;
            trainingOptions.LengthValue = lengthValue;

            //Act
            Func<Task> act = async () => await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, trainingOptions).ConfigureAwait(false);

            //Assert
            await act.Should().NotThrowAsync();
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_NoQuestions_EmptyResult), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_NoQuestions_EmptyResult(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(new List<GetQuestionsForTrainingResult>());

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, new TrainingOptions
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
        public async Task SelectQuestionsForTrainingAsync_CorrectData_AllQuestionsAreDistinct(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionFraction, List<Models.GetQuestionsForTrainingResult> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            var trainingOptions = new TrainingOptions
            {
                LengthType = lengthType,
                LengthValue = lengthValue,
                NewQuestionsFraction = newQuestionsFraction,
                PrioritizedPenaltyQuestionsFraction = penaltyQuestionFraction
            };

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, trainingOptions).ConfigureAwait(false);

            //Assert
            actualResult.Should().BeEquivalentTo(actualResult.Distinct(new GuidComparer()));
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectFullLengthDelta(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.GetQuestionsForTrainingResult> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

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
                    fullLengthOfAllQuestions = allQuestions.Sum(q=>q.FullActualTrainingTimeSeconds());
                    fullLengthOfActualResult = actualResult.Sum(q=>q.FullActualTrainingTimeSeconds());
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
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectNewDeltas(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.GetQuestionsForTrainingResult> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert

            int lengthOfAllNewQuestions, lengthOfAllQuestionsExceptNew;
            int lengthOfNewQuestionsInActualResult;
            int expectedLengthValue;

            switch (lengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    lengthOfAllNewQuestions = allQuestions.Where(q => q.QuestionUserIsNew == true).Count();
                    lengthOfAllQuestionsExceptNew = allQuestions.Count - lengthOfAllNewQuestions;
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Count();
                    expectedLengthValue = lengthValue < allQuestions.Count ? lengthValue : allQuestions.Count;
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    lengthOfAllNewQuestions = allQuestions.Where(q => q.QuestionUserIsNew == true).Sum(q => q.FullActualTrainingTimeSeconds());
                    lengthOfAllQuestionsExceptNew = allQuestions.Sum(q => q.FullActualTrainingTimeSeconds()) - lengthOfAllNewQuestions;
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Sum(q => q.FullActualTrainingTimeSeconds());
                    int allQuestionsTimeSum = allQuestions.Sum(q => q.FullActualTrainingTimeSeconds());
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

                lengthOfNewQuestionsInActualResult.Should().BeInRange(0, //zero - because it may not pick appropriate questions for endless number of attempts
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
                var expectedNewQuestions = allQuestions.Where(q => q.QuestionUserIsNew == true).ToList();
                var actualNewQuestions = actualResult.Where(q=>q.MyStatus!.IsNew).ToList();
                if (expectedNewQuestions.Count != 0)
                {
                    actualNewQuestions.Should().BeEquivalentTo(Mapper.Map<List<ExistingQuestion>>(expectedNewQuestions));
                }
                else
                {
                    actualNewQuestions.Should().BeEmpty();
                }
            }
        }
        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectData), typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectPenaltyDeltas(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingLengthType lengthType, int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.GetQuestionsForTrainingResult> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, new TrainingOptions { LengthType = lengthType, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert

            int lengthOfAllPenaltyQuestions;
            int lengthOfPenaltyQuestionsInActualResult;
            int expectedLengthValue;

            switch (lengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    lengthOfAllPenaltyQuestions = allQuestions.Where(q => q.QuestionUserPenaltyPoints! > 0).Count();
                    lengthOfPenaltyQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).Count();
                    expectedLengthValue = lengthValue < allQuestions.Count ? lengthValue : allQuestions.Count;
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    lengthOfAllPenaltyQuestions = allQuestions.Where(q => q.QuestionUserPenaltyPoints! > 0).Sum(q => q.FullActualTrainingTimeSeconds());
                    lengthOfPenaltyQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).Sum(q => q.FullActualTrainingTimeSeconds());
                    int allQuestionsTimeSum = allQuestions.Sum(q => q.FullActualTrainingTimeSeconds());
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
                var expectedPenaltyQuestions = allQuestions.Where(q => q.QuestionUserPenaltyPoints! > 0).ToList();
                var actualPenaltyQuestions = actualResult.Where(q => q.MyStatus!.PenaltyPoints > 0).ToList();

                if (expectedPenaltyQuestions.Count != 0)
                {
                    actualPenaltyQuestions.Should().BeEquivalentTo(Mapper.Map<List<ExistingQuestion>>(expectedPenaltyQuestions));
                }
                else
                {
                    actualPenaltyQuestions.Should().BeEmpty();
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.SelectQuestionsForTrainingAsync_CorrectDataThatIsImpossibleToMatchLength_CorrectSelection),typeof(TrainingLogicTestDataSource))]
        public async Task SelectQuestionsForTrainingAsync_CorrectDataThatIsImpossibleToMatchLength_CorrectSelection(Guid userId, IEnumerable<Guid> questionnairesIds,
            int lengthValue, double newQuestionsFraction, double penaltyQuestionsFraction, List<Models.GetQuestionsForTrainingResult> allQuestions, List<Guid> expectedQuestionIds)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId, questionnairesIds, new TrainingOptions { LengthType = TrainingLengthType.Time, LengthValue = lengthValue, NewQuestionsFraction = newQuestionsFraction, PrioritizedPenaltyQuestionsFraction = penaltyQuestionsFraction }).ConfigureAwait(false);

            //Assert
            actualResult.Select(q => q.Id).ToList().Should().BeEquivalentTo(expectedQuestionIds);
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.UpdateQuestionStatusAsync_CorrectData_CorrectResult),typeof(TrainingLogicTestDataSource))]
        public async Task UpdateQuestionStatusAsync_CorrectData_CorrectResult(Guid userId, Guid questionId, int answerTimeMilliseconds,
            bool doesSourceStatusExist, bool sourceIsNewStatus, int sourceRating, int sourcePenaltyPoints,
            bool isAnswerCorrect,
            bool expectedIsNewStatus, int expectedRating, int expectedPenaltyPoints)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetUserQuestionStatusAsync(userId, questionId)).ReturnsAsync(
                doesSourceStatusExist ?
                new UserQuestionStatus { QuestionId = questionId, UserId = userId, IsNew = sourceIsNewStatus, Rating = sourceRating, PenaltyPoints = sourcePenaltyPoints } :
                null);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            var actualResult = await trainingLogic.UpdateQuestionStatusAsync(userId, new TrainingResult { IsAnswerCorrect = isAnswerCorrect, QuestionId = questionId, AnswerTimeMilliseconds = answerTimeMilliseconds }).ConfigureAwait(false);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.IsNew.Should().Be(expectedIsNewStatus);
            actualResult.Rating.Should().Be(expectedRating);
            actualResult.PenaltyPoints.Should().Be(expectedPenaltyPoints);
        }

        [TestMethod]
        [DynamicData(nameof(TrainingLogicTestDataSource.UpdateQuestionStatusAsync_IncorrectArguments_ArgumentException), typeof(TrainingLogicTestDataSource))]
        public async Task UpdateQuestionStatusAsync_IncorrectArguments_ArgumentException(Guid userId, TrainingResult request)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object, Mapper);

            //Act
            Func<Task> act = async () => await trainingLogic.UpdateQuestionStatusAsync(userId, request).ConfigureAwait(false);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().ConfigureAwait(false);
        }
    }

    class GuidComparer : IEqualityComparer<Models.ExistingQuestion>
    {
        public bool Equals(ExistingQuestion? x, ExistingQuestion? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode([DisallowNull] ExistingQuestion obj)
        {
            return obj.GetHashCode();
        }
    }
}