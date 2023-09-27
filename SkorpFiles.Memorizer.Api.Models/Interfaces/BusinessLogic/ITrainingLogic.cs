using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic
{
    public interface ITrainingLogic
    {
        public Task<IEnumerable<Question>> SelectQuestionsForTrainingAsync(Guid userId, Guid trainingId, TrainingOptions options);
    }
}
