using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class EditingLogic : IEditingLogic
    {
        private readonly IEditingRepository _editingRepository;

        public EditingLogic(IEditingRepository editingRepository)
        {
            _editingRepository = editingRepository;
        }

        public async Task<Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId)
        {
            return await _editingRepository.GetQuestionnaireAsync(userId, questionnaireId);
        }

        public async Task<Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode)
        {
            return await _editingRepository.GetQuestionnaireAsync(userId, questionnaireCode);
        }

        public async Task<PaginatedCollection<Questionnaire>> GetQuestionnairesAsync(Guid userId, GetQuestionnairesRequest request)
        {
            return await _editingRepository.GetQuestionnairesAsync(userId, request);
        }

        public async Task<PaginatedCollection<Question>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
        {
            return await _editingRepository.GetQuestionsAsync(userId, request);
        }
    }
}
