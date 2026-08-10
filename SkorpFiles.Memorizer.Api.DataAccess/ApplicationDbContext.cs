using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Utils;

namespace SkorpFiles.Memorizer.Api.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Shadow column holding the SHA-256 hash of NormalizedLabelName; the unique index
        // that guards label de-duplication is built on this hash rather than the (long) name.
        private const string NormalizedLabelNameHashColumn = "NormalizedLabelNameHash";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext() : base() { }

        public DbSet<UserActivity> UserActivities => Set<UserActivity>();
        public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionUser> QuestionsUsers => Set<QuestionUser>();
        public DbSet<NormalizedLabel> NormalizedLabels => Set<NormalizedLabel>();
        public DbSet<QuestionLabel> QuestionsLabels => Set<QuestionLabel>();
        public DbSet<QuestionnaireLabel> QuestionnairesLabels => Set<QuestionnaireLabel>();
        public DbSet<TypedAnswer> TypedAnswers => Set<TypedAnswer>();
        public DbSet<Training> Trainings => Set<Training>();
        public DbSet<TrainingQuestionnaire> TrainingsQuestionnaires => Set<TrainingQuestionnaire>();
        public DbSet<TrainingResult> TrainingResults => Set<TrainingResult>();
        public DbSet<TrainingResultTypedAnswer> TrainingResultTypedAnswers => Set<TrainingResultTypedAnswer>();
        public DbSet<EventLog> EventLog => Set<EventLog>();
        public DbSet<AuthenticationCache> AuthenticationCache => Set<AuthenticationCache>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Api.Models.GetQuestionsForTrainingResult>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null);
            });

            modelBuilder.Entity<NormalizedLabel>(builder =>
            {
                // A label can be up to Restrictions.LabelNameMaxLength (10,000) characters,
                // which is far longer than a unique index key can be. Store the name as
                // nvarchar(max) and enforce uniqueness on a persisted SHA-256 hash of it
                // instead. LabelsService still relies on the resulting unique-constraint
                // violation (SQL error 2601/2627) to serialise concurrent inserts.
                builder.Property(x => x.NormalizedLabelName)
                    .HasColumnType("nvarchar(max)")
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();

                // Non-nullable (the source name is required, so the hash is never null),
                // which keeps the unique index un-filtered — SQL Server rejects a filtered
                // index whose predicate references a computed column.
                builder.Property<byte[]>(NormalizedLabelNameHashColumn)
                    .HasColumnType("varbinary(32)")
                    .HasComputedColumnSql(
                        $"HASHBYTES('SHA2_256', [{nameof(NormalizedLabel.NormalizedLabelName)}])",
                        stored: true)
                    .IsRequired();

                builder.HasIndex(NormalizedLabelNameHashColumn)
                    .IsUnique();
            });

            modelBuilder.Entity<QuestionLabel>(builder =>
            {
                builder.Property(x => x.QuestionLabelName)
                    .HasColumnType("nvarchar(max)")
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();
            });

            modelBuilder.Entity<QuestionnaireLabel>(builder =>
            {
                builder.Property(x => x.QuestionnaireLabelName)
                    .HasColumnType("nvarchar(max)")
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();
            });

            // The normalized label id already identifies the label uniquely, so the
            // uniqueness of a label per questionnaire is captured by (QuestionnaireId,
            // NormalizedLabelId) alone; the (nvarchar(max)) display name is not part of
            // the key.
            modelBuilder.Entity<QuestionnaireLabel>()
                .HasIndex(x => new { x.QuestionnaireId, x.NormalizedLabelId })
                .IsUnique();
        }
    }
}
