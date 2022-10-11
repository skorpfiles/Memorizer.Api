using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.Models.Db;

namespace SkorpFiles.Memorizer.Api
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserActivity>? UserActivities { get; set; }
        public DbSet<Questionnaire>? Questionnaires { get; set; }
        public DbSet<Question>? Questions { get; set; }
        public DbSet<QuestionUser>? QuestionsUsers { get; set; }
        public DbSet<Label>? Labels { get; set; }
        public DbSet<EntityLabel>? EntitiesLabels { get; set; }
        public DbSet<TypedAnswer>? TypedAnswers { get; set; }
        public DbSet<EventLog>? EventLog { get; set; }
    }
}
