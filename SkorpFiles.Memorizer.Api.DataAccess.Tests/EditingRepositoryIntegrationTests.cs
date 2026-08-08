using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class EditingRepositoryIntegrationTests : IntegrationTestsBase
    {
        private EditingRepository CreateRepository() => new(DbContext, Mapper);

        // ---------------------------------------------------------------- Questionnaires

        [TestMethod]
        public async Task GetQuestionnairesAsync_ReturnsPublicAndOwnQuestionnaires_HidesForeignPrivate()
        {
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();

            var expectedCount = await DbContext.Questionnaires.CountAsync(q =>
                !q.ObjectIsRemoved &&
                (q.QuestionnaireAvailability == Availability.Public || q.OwnerId == userIdString));

            var repository = CreateRepository();

            var result = await repository.GetQuestionnairesAsync(userId, new GetQuestionnairesRequest());

            result.TotalCount.Should().Be(expectedCount);
            result.Items.Should().HaveCount(expectedCount);
            // Never a foreign private questionnaire, never a removed one.
            result.Items.Should().OnlyContain(q =>
                q.Availability == Availability.Public || q.OwnerId == userId);
            result.Items.Select(q => q.Id).Should().Contain(ScriptData.AlicePublicQuestionnaire);
            result.Items.Select(q => q.Id).Should().NotContain(ScriptData.BobPrivateQuestionnaire);
            result.Items.Select(q => q.Id).Should().NotContain(ScriptData.AliceRemovedQuestionnaire);
        }

        [TestMethod]
        public async Task GetQuestionnairesAsync_FilterOwnPrivate_ReturnsOnlyOwnPrivate()
        {
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();

            var expectedIds = await DbContext.Questionnaires
                .Where(q => !q.ObjectIsRemoved && q.OwnerId == userIdString &&
                            q.QuestionnaireAvailability == Availability.Private)
                .Select(q => q.QuestionnaireId)
                .ToListAsync();

            var repository = CreateRepository();

            var result = await repository.GetQuestionnairesAsync(userId, new GetQuestionnairesRequest
            {
                Origin = Origin.Own,
                Availability = Availability.Private
            });

            result.Items.Select(q => q.Id!.Value).Should().BeEquivalentTo(expectedIds);
        }

        [TestMethod]
        public async Task GetQuestionnaireAsync_ById_ReturnsQuestionnaireWithCounts()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;

            var expected = await DbContext.Questionnaires.SingleAsync(q => q.QuestionnaireId == questionnaireId);
            var expectedTotalQuestions = await DbContext.Questions.CountAsync(q => q.QuestionnaireId == questionnaireId);

            var repository = CreateRepository();

            var result = await repository.GetQuestionnaireAsync(userId, questionnaireId, calculateTime: true);

            result.Should().NotBeNull();
            result!.Id.Should().Be(questionnaireId);
            result.Name.Should().Be(expected.QuestionnaireName);
            result.Availability.Should().Be(expected.QuestionnaireAvailability);
            result.OwnerId.Should().Be(userId);
            result.CountsOfQuestions!.Total.Should().Be(expectedTotalQuestions);
        }

        [TestMethod]
        public async Task GetQuestionnaireAsync_ByCode_ReturnsSameQuestionnaire()
        {
            var userId = ScriptData.Alice;
            var expected = await DbContext.Questionnaires.SingleAsync(q => q.QuestionnaireId == ScriptData.AlicePublicQuestionnaire);

            var repository = CreateRepository();

            var result = await repository.GetQuestionnaireAsync(userId, expected.QuestionnaireCode, calculateTime: true);

            result.Should().NotBeNull();
            result!.Id.Should().Be(expected.QuestionnaireId);
        }

        [TestMethod]
        public async Task GetQuestionnaireAsync_ForeignPrivate_ThrowsAccessDenied()
        {
            var repository = CreateRepository();

            var act = async () => await repository.GetQuestionnaireAsync(ScriptData.Alice, ScriptData.BobPrivateQuestionnaire, calculateTime: true);

            await act.Should().ThrowAsync<AccessDeniedForUserException>();
        }

        [TestMethod]
        public async Task CreateQuestionnaireAsync_CreatesOwnedQuestionnaire()
        {
            var userId = ScriptData.Alice;
            var request = new UpdateQuestionnaireRequest
            {
                Name = "Brand new questionnaire",
                Availability = Availability.Public
            };

            var repository = CreateRepository();

            var result = await repository.CreateQuestionnaireAsync(userId, request);

            result.Name.Should().Be("Brand new questionnaire");
            result.Id.Should().NotBeNull();

            var saved = await FreshContext().Questionnaires.SingleAsync(q => q.QuestionnaireId == result.Id!.Value);
            saved.QuestionnaireName.Should().Be("Brand new questionnaire");
            saved.QuestionnaireAvailability.Should().Be(Availability.Public);
            saved.OwnerId.Should().Be(userId.ToAspNetUserIdString());
            saved.ObjectIsRemoved.Should().BeFalse();
        }

        [TestMethod]
        public async Task UpdateQuestionnaireAsync_ChangesNameAndAvailability()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;

            var request = new UpdateQuestionnaireRequest
            {
                Id = questionnaireId,
                Name = "Renamed questionnaire",
                Availability = Availability.Private
            };

            var repository = CreateRepository();

            var result = await repository.UpdateQuestionnaireAsync(userId, request);

            result.Name.Should().Be("Renamed questionnaire");

            var saved = await FreshContext().Questionnaires.SingleAsync(q => q.QuestionnaireId == questionnaireId);
            saved.QuestionnaireName.Should().Be("Renamed questionnaire");
            saved.QuestionnaireAvailability.Should().Be(Availability.Private);
        }

        [TestMethod]
        public async Task DeleteQuestionnaireAsync_ById_SoftDeletesQuestionnaire()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;

            var repository = CreateRepository();

            await repository.DeleteQuestionnaireAsync(userId, questionnaireId);

            var saved = await FreshContext().Questionnaires.SingleAsync(q => q.QuestionnaireId == questionnaireId);
            saved.ObjectIsRemoved.Should().BeTrue();
            saved.ObjectRemovalTimeUtc.Should().NotBeNull();
        }

        [TestMethod]
        public async Task DeleteQuestionnaireAsync_ByCode_SoftDeletesQuestionnaire()
        {
            var userId = ScriptData.Alice;
            var questionnaire = await DbContext.Questionnaires.SingleAsync(q => q.QuestionnaireId == ScriptData.AlicePrivateQuestionnaire);

            var repository = CreateRepository();

            await repository.DeleteQuestionnaireAsync(userId, questionnaire.QuestionnaireCode);

            var saved = await FreshContext().Questionnaires.SingleAsync(q => q.QuestionnaireId == questionnaire.QuestionnaireId);
            saved.ObjectIsRemoved.Should().BeTrue();
        }

        // ---------------------------------------------------------------- Questions

        [TestMethod]
        public async Task GetQuestionsAsync_ReturnsNonRemovedQuestionsWithStatuses()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;

            var expectedLiveQuestionIds = await DbContext.Questions
                .Where(q => q.QuestionnaireId == questionnaireId && !q.ObjectIsRemoved)
                .Select(q => q.QuestionId)
                .ToListAsync();

            var repository = CreateRepository();

            // A large page so pagination cannot truncate the result.
            var result = await repository.GetQuestionsAsync(userId, new GetQuestionsRequest
            {
                QuestionnaireId = questionnaireId,
                PageSize = 1000
            });

            result.Items.Should().OnlyContain(q => q.QuestionnaireId == questionnaireId && !q.IsRemoved);
            // Every non-removed question of the questionnaire is present.
            result.Items.Select(q => q.Id!.Value).Distinct().Should().BeEquivalentTo(expectedLiveQuestionIds);
            // Alice has statuses on some questions of this questionnaire.
            result.Items.Should().Contain(q => q.MyStatus != null);
        }

        [TestMethod]
        public async Task UpdateQuestionsAsync_CreatesQuestion()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;

            var request = new UpdateQuestionsRequest
            {
                QuestionnaireId = questionnaireId,
                CreatedQuestions = new List<QuestionToUpdate>
                {
                    new()
                    {
                        Text = "A freshly created question?",
                        Type = QuestionType.Task,
                        IsEnabled = true,
                        EstimatedTrainingTimeSeconds = 45
                    }
                }
            };

            var repository = CreateRepository();

            await repository.UpdateQuestionsAsync(userId, request);

            var created = await FreshContext().Questions
                .SingleOrDefaultAsync(q => q.QuestionnaireId == questionnaireId && q.QuestionText == "A freshly created question?");
            created.Should().NotBeNull();
            created!.QuestionType.Should().Be(QuestionType.Task);
            created.QuestionEstimatedTrainingTimeSeconds.Should().Be(45);
            created.ObjectIsRemoved.Should().BeFalse();
        }

        [TestMethod]
        public async Task UpdateQuestionsAsync_DeletesQuestion()
        {
            var userId = ScriptData.Alice;
            var questionnaireId = ScriptData.AlicePublicQuestionnaire;
            // First non-removed question of the questionnaire (code 1).
            var questionId = ScriptData.QuestionId(2, 1);

            var request = new UpdateQuestionsRequest
            {
                QuestionnaireId = questionnaireId,
                DeletedQuestions = new List<QuestionIdentifier>
                {
                    new() { Id = questionId }
                }
            };

            var repository = CreateRepository();

            await repository.UpdateQuestionsAsync(userId, request);

            var deleted = await FreshContext().Questions.SingleAsync(q => q.QuestionId == questionId);
            deleted.ObjectIsRemoved.Should().BeTrue();
            deleted.ObjectRemovalTimeUtc.Should().NotBeNull();
        }

        [TestMethod]
        public async Task UpdateUserQuestionStatusAsync_InsertsStatusForQuestionWithoutOne()
        {
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();
            var questionId = ScriptData.QuestionId(2, 1); // no status for alice yet

            var request = new UpdateUserQuestionStatusesRequest
            {
                Items = new List<UserQuestionStatus>
                {
                    new() { QuestionId = questionId, IsNew = false, Rating = 33, PenaltyPoints = 1 }
                }
            };

            var repository = CreateRepository();

            await repository.UpdateUserQuestionStatusAsync(userId, request);

            var saved = await FreshContext().QuestionsUsers
                .SingleAsync(qu => qu.UserId == userIdString && qu.QuestionId == questionId);
            saved.QuestionUserRating.Should().Be(33);
            saved.QuestionUserIsNew.Should().BeFalse();
            saved.QuestionUserPenaltyPoints.Should().Be(1);
        }

        // ---------------------------------------------------------------- Trainings

        [TestMethod]
        public async Task GetTrainingsForUserAsync_ReturnsOwnNonRemovedTrainings()
        {
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();

            var expected = await DbContext.Trainings
                .Where(t => !t.ObjectIsRemoved && t.OwnerId == userIdString)
                .OrderByDescending(t => t.TrainingLastTimeUtc)
                .Select(t => t.TrainingId)
                .ToListAsync();

            var repository = CreateRepository();

            var result = await repository.GetTrainingsForUserAsync(userId, new GetCollectionRequest());

            result.TotalCount.Should().Be(expected.Count);
            result.Items.Select(t => t.Id!.Value).Should().ContainInOrder(expected);
        }

        [TestMethod]
        public async Task GetTrainingAsync_ReturnsTrainingWithQuestionnaires()
        {
            var userId = ScriptData.Alice;
            var trainingId = ScriptData.AliceTraining;

            var expected = await DbContext.Trainings.SingleAsync(t => t.TrainingId == trainingId);
            var expectedQuestionnaireCount = await DbContext.TrainingsQuestionnaires
                .CountAsync(tq => tq.TrainingId == trainingId);

            var repository = CreateRepository();

            var result = await repository.GetTrainingAsync(userId, trainingId, calculateTime: true);

            result.Id.Should().Be(trainingId);
            result.Name.Should().Be(expected.TrainingName);
            result.LengthType.Should().Be(expected.TrainingLengthType);
            result.Questionnaires.Should().HaveCount(expectedQuestionnaireCount);
        }

        [TestMethod]
        public async Task GetTrainingAsync_ForeignTraining_ThrowsAccessDenied()
        {
            // Training 2 belongs to bob.
            var repository = CreateRepository();

            var act = async () => await repository.GetTrainingAsync(ScriptData.Alice, ScriptData.TrainingId(2), calculateTime: true);

            await act.Should().ThrowAsync<AccessDeniedForUserException>();
        }

        [TestMethod]
        public async Task CreateTrainingAsync_CreatesTrainingWithQuestionnaire()
        {
            var userId = ScriptData.Alice;
            var request = new UpdateTrainingRequest
            {
                Name = "New training",
                LengthType = TrainingLengthType.QuestionsCount,
                QuestionsCount = 10,
                TimeMinutes = 0,
                NewQuestionsFraction = 0.2m,
                PenaltyQuestionsFraction = 0.1m,
                QuestionnairesIds = new List<Guid> { ScriptData.AlicePublicQuestionnaire }
            };

            var repository = CreateRepository();

            var result = await repository.CreateTrainingAsync(userId, request);

            result.Name.Should().Be("New training");
            result.Id.Should().NotBeNull();

            var saved = await FreshContext().Trainings.SingleAsync(t => t.TrainingId == result.Id!.Value);
            saved.OwnerId.Should().Be(userId.ToAspNetUserIdString());
            saved.TrainingQuestionsCount.Should().Be(10);

            var link = await FreshContext().TrainingsQuestionnaires
                .SingleOrDefaultAsync(tq => tq.TrainingId == result.Id!.Value && tq.QuestionnaireId == ScriptData.AlicePublicQuestionnaire);
            link.Should().NotBeNull();
        }

        [TestMethod]
        public async Task UpdateTrainingAsync_ChangesNameAndLength()
        {
            var userId = ScriptData.Alice;
            var trainingId = ScriptData.AliceTraining;

            var request = new UpdateTrainingRequest
            {
                Id = trainingId,
                Name = "Renamed training",
                TimeMinutes = 123
            };

            var repository = CreateRepository();

            var result = await repository.UpdateTrainingAsync(userId, request);

            result.Name.Should().Be("Renamed training");

            var saved = await FreshContext().Trainings.SingleAsync(t => t.TrainingId == trainingId);
            saved.TrainingName.Should().Be("Renamed training");
            saved.TrainingTimeMinutes.Should().Be(123);
        }

        [TestMethod]
        public async Task DeleteTrainingAsync_SoftDeletesTraining()
        {
            var userId = ScriptData.Alice;
            var trainingId = ScriptData.AliceTraining;

            var repository = CreateRepository();

            await repository.DeleteTrainingAsync(userId, trainingId);

            var saved = await FreshContext().Trainings.SingleAsync(t => t.TrainingId == trainingId);
            saved.ObjectIsRemoved.Should().BeTrue();
            saved.ObjectRemovalTimeUtc.Should().NotBeNull();
        }
    }
}
