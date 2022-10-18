using SkorpFiles.Memorizer.Api.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface IEditingRepository
    {
        Task<IEnumerable<Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnaireRequest request);
    }
}
