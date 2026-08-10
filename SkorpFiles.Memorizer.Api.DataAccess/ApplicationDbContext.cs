using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Utils;

namespace SkorpFiles.Memorizer.Api.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
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
                builder.Property(x => x.NormalizedLabelName)
                    .HasColumnType("nvarchar(max)")
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();

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

            modelBuilder.Entity<QuestionnaireLabel>()
                .HasIndex(x => new { x.QuestionnaireId, x.NormalizedLabelId })
                .IsUnique();
        }
    }
}
