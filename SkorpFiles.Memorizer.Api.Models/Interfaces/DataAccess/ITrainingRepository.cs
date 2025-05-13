namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<GetQuestionsForTrainingResult>> GetQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds);
        Task UpdateQuestionStatusAsync(UserQuestionStatus newQuestionStatus, TrainingResult trainingResult, Api.Models.QuestionStatus defaultQuestionStatus);
        Task<UserQuestionStatus?> GetUserQuestionStatusAsync(Guid userId, Guid questionId);
    }
}
