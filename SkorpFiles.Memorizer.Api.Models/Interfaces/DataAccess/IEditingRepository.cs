using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface IEditingRepository
    {
        Task<PaginatedCollection<Questionnaire>> GetQuestionnairesAsync(Guid userId, GetQuestionnairesRequest request);
        Task<Questionnaire?> GetQuestionnaireAsync(Guid userId, Guid questionnaireId, bool calculateTime);
        Task<Questionnaire?> GetQuestionnaireAsync(Guid userId, int questionnaireCode, bool calculateTime);
        Task<Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request);
        Task<Questionnaire> UpdateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request);
        Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId);
        Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode);
        Task<PaginatedCollection<ExistingQuestion>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request);
        Task UpdateQuestionsAsync(Guid userId, UpdateQuestionsRequest request);
        Task UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusRequest request);

        Task<PaginatedCollection<Label>> GetLabelsAsync(Guid userId, GetLabelsRequest request);
        Task<Label> GetLabelAsync(Guid userId, Guid labelId);
        Task<Label> GetLabelAsync(Guid userId, int labelCode);
        Task<Label> CreateLabelAsync(Guid userId, string labelName);
        Task DeleteLabelAsync(Guid userId, Guid labelId);
        Task DeleteLabelAsync(Guid userId, int labelCode);

        Task<PaginatedCollection<Training>> GetTrainingsForUserAsync(Guid userId, GetCollectionRequest request);
        Task<Training> GetTrainingAsync(Guid userId, Guid trainingId);
        Task<Training> CreateTrainingAsync(Guid userId, UpdateTrainingRequest request);
        Task<Training> UpdateTrainingAsync(Guid userId, UpdateTrainingRequest request);
        Task DeleteTrainingAsync(Guid userId, Guid trainingId);
    }
}
