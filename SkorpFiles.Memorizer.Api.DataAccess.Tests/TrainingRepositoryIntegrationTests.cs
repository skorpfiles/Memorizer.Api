using FluentAssertions;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.DataAccess.Tests.DataSources;
using System.Collections.Generic;
using System.Text.Json;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    public class TrainingRepositoryIntegrationTests:IntegrationTestsBase
    {
        [TestMethod]
        [DynamicData(nameof(TrainingRepositoryTestDataSource.GetQuestionsForTrainingAsync_CorrectParameters_ReturnsQuestions),typeof(TrainingRepositoryTestDataSource))]
        public async Task GetQuestionsForTrainingAsync_CorrectParameters_ReturnsExactQuestionsList(List<Questionnaire> questionnaires, 
            List<Guid> questionnairesIds, List<int> expectedTrainingTimesSeconds, List<string> expectedTypedAnswersJsonsWithoutSpaces)
        {
            // Arrange
            var userId = Constants.DefaultUserId;

            await RegisterUserAsync();

            await DbContext.Questionnaires.AddRangeAsync(questionnaires);
            await DbContext.SaveChangesAsync();

            List<Question> expectedQuestions = new List<Question>();
            foreach (var questionnaireId in questionnairesIds)
            {
                var questionnaire = questionnaires.First(x => x.QuestionnaireId == questionnaireId);
                var questions = questionnaire.Questions!.ToList();
                expectedQuestions.AddRange(questions);
            }

            var trainingRepository = new TrainingRepository(DbContext, Mapper);

            // Act
            var actualResults = await trainingRepository.GetQuestionsForTrainingAsync(userId, questionnairesIds);

            // Assert
            actualResults.Should().NotBeNull();
            actualResults.Should().HaveCount(expectedQuestions.Count);
            actualResults.Should().OnlyContain(actualResults => actualResults.QuestionnaireId != null && questionnairesIds.Contains(actualResults.QuestionnaireId.Value));

            for (int i = 0; i < expectedQuestions.Count; i++)
            {
                var actualResult = actualResults.First(x => x.Id == expectedQuestions[i].QuestionId);
                actualResult.Should().NotBeNull();
                actualResult.QuestionType.Should().Be(expectedQuestions[i].QuestionType);
                actualResult.QuestionText.Should().Be(expectedQuestions[i].QuestionText);
                actualResult.QuestionUntypedAnswer.Should().Be(expectedQuestions[i].QuestionUntypedAnswer);
                actualResult.QuestionIsEnabled.Should().Be(expectedQuestions[i].QuestionIsEnabled);
                actualResult.QuestionReference.Should().Be(expectedQuestions[i].QuestionReference);
                actualResult.QuestionEstimatedTrainingTimeSeconds.Should().Be(expectedQuestions[i].QuestionEstimatedTrainingTimeSeconds);
                if (expectedTypedAnswersJsonsWithoutSpaces[i] != null)
                {
                    actualResult.TypedAnswersJson.Should().NotBeNull();
                    JsonSerializer.Deserialize<List<TypedAnswer>>(actualResult.TypedAnswersJson!).Should().BeEquivalentTo(
                        JsonSerializer.Deserialize<List<TypedAnswer>>(expectedTypedAnswersJsonsWithoutSpaces[i]));
                }
                else
                {
                    actualResult.TypedAnswersJson.Should().BeNull();
                }
                actualResult.QuestionnaireName.Should().Be(expectedQuestions[i].Questionnaire!.QuestionnaireName);
                var userForQuestion = expectedQuestions[i].UsersForQuestion?.FirstOrDefault(x => x.UserId == userId.ToAspNetUserIdString());
                actualResult.QuestionUserIsNew.Should().Be(userForQuestion?.QuestionUserIsNew);
                actualResult.QuestionUserRating.Should().Be(userForQuestion?.QuestionUserRating);
                actualResult.QuestionUserPenaltyPoints.Should().Be(userForQuestion?.QuestionUserPenaltyPoints);
                actualResult.QuestionActualTrainingTimeSeconds.Should().Be(expectedTrainingTimesSeconds[i]);
            }
        }
    }
}
