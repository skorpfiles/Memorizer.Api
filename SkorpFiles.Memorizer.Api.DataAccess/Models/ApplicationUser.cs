using Microsoft.AspNetCore.Identity;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual UserActivity? UserActivity { get; set; }
        public virtual List<Label>? LabelsThatUserOwns { get; set; }
        public virtual List<Questionnaire>? QuestionnairesThatUserOwns { get; set; }
        public virtual List<QuestionUser>? QuestionsForUser { get; set; }
        public virtual List<TrainingResult>? TrainingResultsForUser { get; set; }
    }
}
