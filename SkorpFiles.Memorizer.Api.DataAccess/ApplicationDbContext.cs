using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models;

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
        public DbSet<Label> Labels => Set<Label>();
        public DbSet<EntityLabel> EntitiesLabels => Set<EntityLabel>();
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
        }
    }
}
