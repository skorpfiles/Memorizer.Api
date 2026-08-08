using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models;
using System.Text.Json;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class TrainingRepositoryIntegrationTests : IntegrationTestsBase
    {
        private TrainingRepository CreateRepository() => new(DbContext, Mapper);

        [TestMethod]
        public async Task GetQuestionsForTrainingAsync_CorrectParameters_ReturnsExactQuestionsList()
        {
            // Arrange - two questionnaires owned by alice: one private, one public.
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();
            var questionnairesIds = new List<Guid> { ScriptData.AlicePrivateQuestionnaire, ScriptData.AlicePublicQuestionnaire };

            // The query returns every non-removed question of the given questionnaires.
            var expectedQuestions = await DbContext.Questions
                .Include(q => q.Questionnaire)
                .Include(q => q.TypedAnswers)
                .Where(q => !q.ObjectIsRemoved && questionnairesIds.Contains(q.QuestionnaireId))
                .ToListAsync();

            var expectedStatuses = await DbContext.QuestionsUsers
                .Where(qu => qu.UserId == userIdString)
                .ToListAsync();

            // Alice's past attempts (section 9 of the script). These drive the actual-time
            // computation; other users' attempts must not affect it.
            var expectedResults = (await DbContext.TrainingResults
                    .Where(tr => tr.TrainingResultUserId == userIdString)
                    .ToListAsync())
                .GroupBy(tr => tr.TrainingResultQuestionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var repository = CreateRepository();

            // Act
            var actualResults = (await repository.GetQuestionsForTrainingAsync(userId, questionnairesIds)).ToList();

            // Assert
            actualResults.Should().HaveCount(expectedQuestions.Count);
            actualResults.Should().OnlyContain(r => r.QuestionnaireId != null && questionnairesIds.Contains(r.QuestionnaireId.Value));

            foreach (var expected in expectedQuestions)
            {
                var actual = actualResults.Single(r => r.Id == expected.QuestionId);

                actual.QuestionType.Should().Be(expected.QuestionType);
                actual.QuestionText.Should().Be(expected.QuestionText);
                actual.QuestionUntypedAnswer.Should().Be(expected.QuestionUntypedAnswer);
                actual.QuestionIsEnabled.Should().Be(expected.QuestionIsEnabled);
                actual.QuestionReference.Should().Be(expected.QuestionReference);
                actual.QuestionEstimatedTrainingTimeSeconds.Should().Be(expected.QuestionEstimatedTrainingTimeSeconds);
                actual.QuestionnaireId.Should().Be(expected.QuestionnaireId);
                actual.QuestionnaireName.Should().Be(expected.Questionnaire!.QuestionnaireName);

                // The actual training time blends the estimate with the median of the recorded
                // times; without any records it falls back to the estimate.
                expectedResults.TryGetValue(expected.QuestionId, out var results);
                actual.QuestionActualTrainingTimeSeconds.Should().Be(
                    ExpectedActualTime(expected.QuestionEstimatedTrainingTimeSeconds, results));
                actual.LastTrainingTimeUtc.Should().Be(
                    results is { Count: > 0 } ? results.Max(r => r.TrainingResultRecordingTime) : null);

                var expectedLiveAnswers = expected.TypedAnswers!
                    .Where(a => !a.ObjectIsRemoved)
                    .Select(a => new { a.TypedAnswerId, a.TypedAnswerText })
                    .ToList();

                if (expectedLiveAnswers.Count > 0)
                {
                    actual.TypedAnswersJson.Should().NotBeNull();
                    var actualAnswers = JsonSerializer.Deserialize<List<DataAccess.Models.TypedAnswer>>(actual.TypedAnswersJson!)!
                        .Select(a => new { a.TypedAnswerId, a.TypedAnswerText });
                    actualAnswers.Should().BeEquivalentTo(expectedLiveAnswers);
                }
                else
                {
                    actual.TypedAnswersJson.Should().BeNull();
                }

                var expectedStatus = expectedStatuses.SingleOrDefault(s => s.QuestionId == expected.QuestionId);
                actual.QuestionUserIsNew.Should().Be(expectedStatus?.QuestionUserIsNew);
                actual.QuestionUserRating.Should().Be(expectedStatus?.QuestionUserRating);
                actual.QuestionUserPenaltyPoints.Should().Be(expectedStatus?.QuestionUserPenaltyPoints);
            }

            // The four seeded questions exercise the branches of the formula and must differ
            // from a plain estimate fallback, so a broken seed cannot pass silently.
            ActualTimeFor(actualResults, ScriptData.QuestionId(2, 1)).Should().Be(18); // 1 attempt
            ActualTimeFor(actualResults, ScriptData.QuestionId(2, 2)).Should().Be(21); // 8 attempts
            ActualTimeFor(actualResults, ScriptData.QuestionId(2, 3)).Should().Be(11); // 10 attempts
            ActualTimeFor(actualResults, ScriptData.QuestionId(2, 4)).Should().Be(21); // 12 attempts
        }

        private static int ActualTimeFor(IEnumerable<GetQuestionsForTrainingResult> results, Guid questionId) =>
            results.Single(r => r.Id == questionId).QuestionActualTrainingTimeSeconds;

        /// <summary>
        /// Mirrors the CASE expression in GetQuestionsForTrainingQueryTemplate.sql: no attempts
        /// gives the estimate, 1..10 attempts blend the estimate with the median, and more than
        /// 10 attempts use the median alone.
        /// </summary>
        private static int ExpectedActualTime(int estimate, List<DataAccess.Models.TrainingResult>? results)
        {
            if (results is not { Count: > 0 })
                return estimate;

            var count = results.Count;
            var median = Median(results.Select(r => r.TrainingResultTimeSeconds).ToList());
            if (count <= 10)
                return (int)((estimate * (10 - count) + median) / (10 - count + 1));
            return (int)median;
        }

        private static double Median(IReadOnlyList<int> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            var n = sorted.Count;
            return n % 2 == 1 ? sorted[n / 2] : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
        }

        [TestMethod]
        public async Task GetQuestionsForTrainingAsync_EmptyQuestionnairesIds_Throws()
        {
            var repository = CreateRepository();

            var act = async () => await repository.GetQuestionsForTrainingAsync(ScriptData.Alice, new List<Guid>());

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task GetUserQuestionStatusAsync_ExistingStatus_ReturnsIt()
        {
            // Arrange - a question alice already has a status for.
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();
            var expected = await DbContext.QuestionsUsers.FirstAsync(qu => qu.UserId == userIdString);

            var repository = CreateRepository();

            // Act
            var actual = await repository.GetUserQuestionStatusAsync(userId, expected.QuestionId);

            // Assert
            actual.Should().NotBeNull();
            actual!.IsNew.Should().Be(expected.QuestionUserIsNew);
            actual.Rating.Should().Be(expected.QuestionUserRating);
            actual.PenaltyPoints.Should().Be(expected.QuestionUserPenaltyPoints);
        }

        [TestMethod]
        public async Task GetUserQuestionStatusAsync_NoStatus_ReturnsNull()
        {
            // Question 1 of the public questionnaire has no status for alice (statuses exist
            // only for every fifth question).
            var questionId = ScriptData.QuestionId(2, 1);

            var repository = CreateRepository();

            var actual = await repository.GetUserQuestionStatusAsync(ScriptData.Alice, questionId);

            actual.Should().BeNull();
        }

        [TestMethod]
        public async Task UpdateQuestionStatusAsync_NoExistingStatus_InsertsStatusAndLogsResult()
        {
            // Arrange - a public question alice has neither a status nor any past attempt for
            // (codes 1-4 carry seeded training results, multiples of 5 carry statuses; 6 has neither).
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();
            var questionId = ScriptData.QuestionId(2, 6);

            var newStatus = new UserQuestionStatus
            {
                QuestionId = questionId,
                UserId = userId,
                IsNew = false,
                Rating = 42,
                PenaltyPoints = 3
            };
            var trainingResult = BuildTrainingResult(userId, questionId);
            var defaultStatus = new QuestionStatus { IsNew = true, Rating = 50, PenaltyPoints = 0 };

            var repository = CreateRepository();

            // Act
            await repository.UpdateQuestionStatusAsync(newStatus, trainingResult, defaultStatus);

            // Assert - a status row was created with the new values.
            var savedStatus = await FreshContext().QuestionsUsers
                .SingleOrDefaultAsync(qu => qu.UserId == userIdString && qu.QuestionId == questionId);
            savedStatus.Should().NotBeNull();
            savedStatus!.QuestionUserIsNew.Should().BeFalse();
            savedStatus.QuestionUserRating.Should().Be(42);
            savedStatus.QuestionUserPenaltyPoints.Should().Be(3);

            // Assert - the training attempt was logged with the default status as the initial one.
            var loggedResult = await FreshContext().TrainingResults
                .SingleOrDefaultAsync(tr => tr.TrainingResultUserId == userIdString && tr.TrainingResultQuestionId == questionId);
            loggedResult.Should().NotBeNull();
            loggedResult!.TrainingResultInitialNewStatus.Should().Be(defaultStatus.IsNew);
            loggedResult.TrainingResultInitialRating.Should().Be(defaultStatus.Rating);
            loggedResult.TrainingResultInitialPenaltyPoints.Should().Be(defaultStatus.PenaltyPoints);
        }

        [TestMethod]
        public async Task UpdateQuestionStatusAsync_ExistingStatus_UpdatesStatusAndLogsInitial()
        {
            // Arrange - a question alice already has a status for.
            var userId = ScriptData.Alice;
            var userIdString = userId.ToAspNetUserIdString();
            var existing = await DbContext.QuestionsUsers.FirstAsync(qu => qu.UserId == userIdString);
            var initialRating = existing.QuestionUserRating;
            var initialIsNew = existing.QuestionUserIsNew;
            var initialPenalty = existing.QuestionUserPenaltyPoints;

            var newStatus = new UserQuestionStatus
            {
                QuestionId = existing.QuestionId,
                UserId = userId,
                IsNew = false,
                Rating = 7,
                PenaltyPoints = 2
            };
            var trainingResult = BuildTrainingResult(userId, existing.QuestionId);

            var repository = CreateRepository();

            // Act - defaultQuestionStatus is ignored on the existing path.
            await repository.UpdateQuestionStatusAsync(newStatus, trainingResult, new QuestionStatus());

            // Assert - the existing row was updated in place.
            var savedStatus = await FreshContext().QuestionsUsers
                .SingleAsync(qu => qu.UserId == userIdString && qu.QuestionId == existing.QuestionId);
            savedStatus.QuestionUserRating.Should().Be(7);
            savedStatus.QuestionUserIsNew.Should().BeFalse();
            savedStatus.QuestionUserPenaltyPoints.Should().Be(2);

            // Assert - the logged training result carries the status as it was before the update.
            var loggedResult = await FreshContext().TrainingResults
                .SingleAsync(tr => tr.TrainingResultUserId == userIdString && tr.TrainingResultQuestionId == existing.QuestionId);
            loggedResult.TrainingResultInitialRating.Should().Be(initialRating);
            loggedResult.TrainingResultInitialNewStatus.Should().Be(initialIsNew);
            loggedResult.TrainingResultInitialPenaltyPoints.Should().Be(initialPenalty);
        }

        [TestMethod]
        public async Task SeededTrainingResults_RecordTypedAnswersConsistently()
        {
            // Guards the invariant of section 9 of the script: every attempt on a question that
            // has typed answers records one given answer per undeleted typed answer, and the given
            // answers match the originals exactly when the attempt is marked correct.
            var results = await DbContext.TrainingResults.ToListAsync();

            var givenByResult = (await DbContext.TrainingResultTypedAnswers.ToListAsync())
                .GroupBy(g => g.TrainingResultId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var originalsByQuestion = (await DbContext.TypedAnswers.Where(ta => !ta.ObjectIsRemoved).ToListAsync())
                .GroupBy(ta => ta.QuestionId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(a => a.TypedAnswerText).ToList());

            bool anyCorrect = false, anyIncorrect = false, anyMultiAnswer = false;

            foreach (var result in results)
            {
                var originals = originalsByQuestion.GetValueOrDefault(result.TrainingResultQuestionId, new List<string>());
                var given = givenByResult.GetValueOrDefault(result.TrainingResultId, new List<DataAccess.Models.TrainingResultTypedAnswer>());

                // One recorded answer per undeleted typed answer (zero for questions without any).
                given.Should().HaveCount(originals.Count);
                if (originals.Count == 0)
                    continue;
                if (originals.Count > 1)
                    anyMultiAnswer = true;

                if (result.TrainingResultAnswerIsCorrect)
                {
                    anyCorrect = true;
                    given.Should().OnlyContain(g => originals.Contains(g.TrtaAnswer));
                }
                else
                {
                    anyIncorrect = true;
                    given.Should().Contain(g => !originals.Contains(g.TrtaAnswer));
                }
            }

            // The seed is meant to exercise all of these cases.
            anyCorrect.Should().BeTrue();
            anyIncorrect.Should().BeTrue();
            anyMultiAnswer.Should().BeTrue();
        }

        private static Api.Models.TrainingResult BuildTrainingResult(Guid userId, Guid questionId) => new()
        {
            QuestionId = questionId,
            UserId = userId,
            RecordingTime = DateTime.UtcNow,
            IsAnswerCorrect = true,
            AnswerTimeMilliseconds = 5000,
            ResultQuestionStatus = new QuestionStatus { IsNew = false, Rating = 42, PenaltyPoints = 0 }
        };
    }
}
