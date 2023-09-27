namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<Question>> GetQuestionsForTrainingAsync(Guid userId, Guid trainingId);
        Task UpdateQuestionStatusAsync(UserQuestionStatus questionStatus);
    }
}
