using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Utils;

namespace SkorpFiles.Memorizer.Api.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
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
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();

                builder.HasIndex(x => x.NormalizedLabelName)
                    .IsUnique();
            });

            modelBuilder.Entity<QuestionLabel>(builder =>
            {
                builder.Property(x => x.QuestionLabelName)
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();
            });

            modelBuilder.Entity<QuestionnaireLabel>(builder =>
            {
                builder.Property(x => x.QuestionnaireLabelName)
                    .HasMaxLength(Restrictions.LabelNameMaxLength)
                    .IsRequired();
            });

            modelBuilder.Entity<QuestionnaireLabel>()
                .HasIndex(x => new { x.QuestionnaireId, x.NormalizedLabelId, x.QuestionnaireLabelName })
                .IsUnique();
        }
    }
}
