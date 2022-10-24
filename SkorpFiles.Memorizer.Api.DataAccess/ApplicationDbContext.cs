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

        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionUser> QuestionsUsers { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<EntityLabel> EntitiesLabels { get; set; }
        public DbSet<TypedAnswer> TypedAnswers { get; set; }
        public DbSet<TrainingResult> TrainingResults { get; set; }
        public DbSet<TrainingResultTypedAnswer> TrainingResultTypedAnswers { get; set; }
        public DbSet<EventLog> EventLog { get; set; }

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
