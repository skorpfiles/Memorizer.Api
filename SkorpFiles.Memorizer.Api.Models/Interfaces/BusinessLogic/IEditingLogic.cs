using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic
{
    public interface IEditingLogic
    {
        Task<PaginatedCollection<Questionnaire>> GetQuestionnairesAsync(Guid userId, GetQuestionnairesRequest request);
        Task<Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId);
        Task<Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode);
        Task<PaginatedCollection<Question>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request);
    }
}
