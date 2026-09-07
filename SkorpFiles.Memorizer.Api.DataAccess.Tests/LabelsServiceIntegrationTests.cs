using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        /// <summary>
        /// Records every SaveChanges failure that is a real unique-constraint violation on the
        /// underlying SQL Server unique index (error 2601/2627) - the exact condition
        /// LabelsService.IsUniqueConstraintViolation checks for - so a test can prove that
        /// condition was actually hit, not just that the overall operation happened to succeed.
        /// </summary>
        private sealed class UniqueConstraintViolationObserver : SaveChangesInterceptor
        {
            private int _violationCount;

            public int ViolationCount => _violationCount;

            public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
            {
                if (eventData.Exception is DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } })
                    Interlocked.Increment(ref _violationCount);

                return base.SaveChangesFailedAsync(eventData, cancellationToken);
            }
        }

        private sealed class SingleOptionsDbContextFactory(DbContextOptions<ApplicationDbContext> options)
            : IDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext() => new(options);
        }

        [TestMethod]
        public async Task EnsureLabelsAsync_TwoUsersConcurrentlyAddTheSameNewLabel_BothQuestionsShareOneNormalizedLabel()
        {
            const string labelName = "Concurrent Shared Label";
            const string normalizedLabelName = "CONCURRENT SHARED LABEL";

            // A LabelsService instrumented to observe every SaveChanges attempt it makes, so the
            // assertions below can prove a real unique-constraint violation happened during the
            // race - not just that the end result looks right, which could otherwise be true even
            // if the two requests happened to run sequentially and never actually collided.
            var violationObserver = new UniqueConstraintViolationObserver();
            var interceptedOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(DbContext.Database.GetConnectionString())
                .AddInterceptors(violationObserver)
                .Options;
            var labelsService = new LabelsService(new SingleOptionsDbContextFactory(interceptedOptions));

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

            // Proof that IsUniqueConstraintViolation's retry path, not just a lucky non-colliding
            // schedule, is what made this succeed: the loser of the race really did get a
            // DbUpdateException wrapping SQL Server error 2601/2627 from SaveChangesAsync, and
            // Task.WhenAll above still completed without throwing - which is only possible if that
            // exception was caught and retried rather than left to propagate.
            violationObserver.ViolationCount.Should().BeGreaterThanOrEqualTo(1,
                "one of the two concurrent EnsureLabelsAsync attempts should have collided on the " +
                "unique index over NormalizedLabelName and been caught by IsUniqueConstraintViolation");

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
