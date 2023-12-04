using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources;
using SkorpFiles.Memorizer.Api.Models;
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

        public async Task SelectQuestionsForTrainingAsync_CorrectData_CorrectResult(Guid userId, Guid trainingId, TrainingOptions options, List<Models.Question> allQuestions)
        {
            //Arrange
            var trainingRepositoryMock = new Mock<ITrainingRepository>();
            trainingRepositoryMock.Setup(x => x.GetQuestionsForTrainingAsync(userId, trainingId)).ReturnsAsync(allQuestions);

            var trainingLogic = new TrainingLogic(trainingRepositoryMock.Object);

            //Act
            var actualResult = await trainingLogic.SelectQuestionsForTrainingAsync(userId,trainingId,options).ConfigureAwait(false);

            //Assert
            //1) all questions must be unique
            actualResult.Should().BeEquivalentTo(actualResult.Distinct(new GuidComparer()));

            //2) error of full length of training mustn't be more than 10% of specified

            int fullLengthOfAllQuestions;
            int fullLengthOfActualResult;
            switch (options.LengthType)
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

            if (fullLengthOfAllQuestions > options.LengthValue)
            {
                int fractionValue = Convert.ToInt32(Math.Round(options.LengthValue / 100 * Constants.AllowableErrorFraction));
                fullLengthOfActualResult.Should().BeInRange(options.LengthValue - fractionValue, options.LengthValue + fractionValue);
            }
            else
            {
                fullLengthOfActualResult.Should().Be(fullLengthOfAllQuestions);
            }

            //3) length of new and penalty questions must meet options

            int lengthOfAllNewQuestions;
            int lengthOfNewQuestionsInActualResult;

            switch(options.LengthType)
            {
                case Models.Enums.TrainingLengthType.QuestionsCount:
                    lengthOfAllNewQuestions = allQuestions.Where(q=>q.MyStatus!.IsNew).Count();
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Count();
                    break;
                case Models.Enums.TrainingLengthType.Time:
                    lengthOfAllNewQuestions = allQuestions.Where(q => q.MyStatus!.IsNew).Sum(q => q.EstimatedTrainingTimeSeconds);
                    lengthOfNewQuestionsInActualResult = actualResult.Where(q => q.MyStatus!.IsNew).Sum(q => q.EstimatedTrainingTimeSeconds);
                    break;
                default:
                    throw new FluentAssertions.Execution.AssertionFailedException("There is no such length type.");
            }

            //сначала проверим, будто бы новых вопросов в исходном списке гораздо больше.
            //потом нужно добавить условие, по которому процент вопросов может варьироваться, если вопросов в исходном списке не хватает для достижения заявленного процента.

            int recommendedNewQuestionsLengthValue = Convert.ToInt32(Math.Round(options.LengthValue / 100 * options.NewQuestionsFraction));
            int newQuestionsLengthValueFraction = Convert.ToInt32(Math.Round(recommendedNewQuestionsLengthValue / 100 * Constants.AllowableErrorFraction));

            lengthOfNewQuestionsInActualResult.Should().BeInRange(recommendedNewQuestionsLengthValue - newQuestionsLengthValueFraction, recommendedNewQuestionsLengthValue + newQuestionsLengthValueFraction);

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