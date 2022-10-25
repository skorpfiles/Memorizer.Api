using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface IEditingRepository
    {
        Task<IEnumerable<Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnairesRequest request);
        Task<Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId);
        Task<Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode);
    }
}
