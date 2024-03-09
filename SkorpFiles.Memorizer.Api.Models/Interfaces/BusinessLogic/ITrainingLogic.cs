using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic
{
    public interface ITrainingLogic
    {
        public Task<IEnumerable<ExistingQuestion>> SelectQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds, TrainingOptions options);
        public Task<UserQuestionStatus> UpdateQuestionStatusAsync(Guid userId, TrainingResult requestData);
    }
}
