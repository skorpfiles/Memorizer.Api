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

        public async Task<Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            return await _editingRepository.CreateQuestionnaireAsync(userId, request);
        }

        public Task<Questionnaire> UpdateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId)
        {
            await _editingRepository.DeleteQuestionnaireAsync(userId, questionnaireId);
        }

        public async Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode)
        {
            await _editingRepository.DeleteQuestionnaireAsync(userId, questionnaireCode);
        }

        public async Task<PaginatedCollection<Question>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
        {
            return await _editingRepository.GetQuestionsAsync(userId, request);
        }

        public Task<Question> UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedCollection<Label>> GetLabelsAsync(Guid userId, GetLabelsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Label> GetLabelAsync(Guid userId, Guid labelId)
        {
            throw new NotImplementedException();
        }

        public Task<Label> GetLabelAsync(Guid userId, int labelCode)
        {
            throw new NotImplementedException();
        }

        public Task<Label> CreateLabelAsync(Guid userId, string labelName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLabelAsync(Guid userId, Guid labelId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLabelAsync(Guid userId, int labelCode)
        {
            throw new NotImplementedException();
        }
    }
}
