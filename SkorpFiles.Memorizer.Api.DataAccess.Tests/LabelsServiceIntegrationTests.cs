using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Interfaces;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class LabelsServiceIntegrationTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task EnsureLabelsAsync_TwoUsersConcurrentlyAddTheSameNewLabel_BothQuestionsShareOneNormalizedLabel()
        {
            const string labelName = "Concurrent Shared Label";
            const string normalizedLabelName = "CONCURRENT SHARED LABEL";

            var labelsService = ServiceProvider.GetRequiredService<ILabelsService>();

            // A separate DbContext per "request" - the way ASP.NET Core would give each request
            // its own scoped context - since a single DbContext cannot run two operations
            // concurrently the way the base class's shared DbContext (used by CreateRepository()
            // in the other repository tests) would.
            var aliceRepository = new EditingRepository(FreshContext(), Mapper, labelsService);
            var bobRepository = new EditingRepository(FreshContext(), Mapper, labelsService);

            var aliceRequest = new UpdateQuestionsRequest
            {
                QuestionnaireId = ScriptData.AlicePublicQuestionnaire,
                CreatedQuestions = new List<QuestionToUpdate>
                {
                    new()
                    {
                        Text = "Alice's question with a brand-new shared label",
                        Type = QuestionType.Task,
                        IsEnabled = true,
                        EstimatedTrainingTimeSeconds = 30,
                        Labels = new[] { labelName }
                    }
                }
            };

            var bobRequest = new UpdateQuestionsRequest
            {
                QuestionnaireId = ScriptData.BobPublicQuestionnaire,
                CreatedQuestions = new List<QuestionToUpdate>
                {
                    new()
                    {
                        Text = "Bob's question with the same brand-new shared label",
                        Type = QuestionType.Task,
                        IsEnabled = true,
                        EstimatedTrainingTimeSeconds = 30,
                        Labels = new[] { labelName }
                    }
                }
            };

            // Act: Alice and Bob each add a question carrying the same never-before-seen label at
            // the same time, neither aware the other is doing the same. Whichever of them commits
            // their new rNormalizedLabel row first makes the other's EnsureLabelsAsync attempt
            // collide with the unique index on NormalizedLabelName; LabelsService's
            // IsUniqueConstraintViolation check must catch that DbUpdateException and retry,
            // discovering and reusing the label the other user just created instead of throwing or
            // leaving a duplicate normalized label behind.
            await Task.WhenAll(
                aliceRepository.UpdateQuestionsAsync(ScriptData.Alice, aliceRequest),
                bobRepository.UpdateQuestionsAsync(ScriptData.Bob, bobRequest));

            var db = FreshContext();

            var normalizedLabels = await db.NormalizedLabels
                .Where(nl => nl.NormalizedLabelName == normalizedLabelName)
                .ToListAsync();
            normalizedLabels.Should().ContainSingle(
                "both users' requests should converge on one normalized label, not create a duplicate");

            var normalizedLabelId = normalizedLabels.Single().NormalizedLabelId;

            var aliceQuestionLabel = await db.QuestionsLabels
                .Include(l => l.Question)
                .SingleAsync(l => l.Question!.QuestionnaireId == ScriptData.AlicePublicQuestionnaire &&
                                   l.Question.QuestionText == "Alice's question with a brand-new shared label");
            var bobQuestionLabel = await db.QuestionsLabels
                .Include(l => l.Question)
                .SingleAsync(l => l.Question!.QuestionnaireId == ScriptData.BobPublicQuestionnaire &&
                                   l.Question.QuestionText == "Bob's question with the same brand-new shared label");

            aliceQuestionLabel.NormalizedLabelId.Should().Be(normalizedLabelId);
            bobQuestionLabel.NormalizedLabelId.Should().Be(normalizedLabelId);
        }
    }
}
