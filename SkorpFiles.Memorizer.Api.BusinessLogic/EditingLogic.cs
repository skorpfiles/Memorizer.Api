﻿using SkorpFiles.Memorizer.Api.Models;
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

        public async Task<Questionnaire?> GetQuestionnaireAsync(Guid userId, Guid questionnaireId, bool calculateTime)
        {
            return await _editingRepository.GetQuestionnaireAsync(userId, questionnaireId, calculateTime);
        }

        public async Task<Questionnaire?> GetQuestionnaireAsync(Guid userId, int questionnaireCode, bool calculateTime)
        {
            return await _editingRepository.GetQuestionnaireAsync(userId, questionnaireCode, calculateTime);
        }

        public async Task<PaginatedCollection<Questionnaire>> GetQuestionnairesAsync(Guid userId, GetQuestionnairesRequest request)
        {
            return await _editingRepository.GetQuestionnairesAsync(userId, request);
        }

        public async Task<Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            return await _editingRepository.CreateQuestionnaireAsync(userId, request);
        }

        public async Task<Questionnaire> UpdateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            return await _editingRepository.UpdateQuestionnaireAsync(userId,request);
        }

        public async Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId)
        {
            await _editingRepository.DeleteQuestionnaireAsync(userId, questionnaireId);
        }

        public async Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode)
        {
            await _editingRepository.DeleteQuestionnaireAsync(userId, questionnaireCode);
        }

        public async Task<PaginatedCollection<ExistingQuestion>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
        {
            return await _editingRepository.GetQuestionsAsync(userId, request);
        }

        public async Task UpdateQuestionsAsync(Guid userId, UpdateQuestionsRequest request)
        {
            await _editingRepository.UpdateQuestionsAsync(userId, request);
        }

        public async Task UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusesRequest request)
        {
            await _editingRepository.UpdateUserQuestionStatusAsync(userId, request);
        }

        public async Task<PaginatedCollection<Label>> GetLabelsAsync(Guid userId, GetLabelsRequest request)
        {
            return await _editingRepository.GetLabelsAsync(userId, request);
        }

        public async Task<Label> GetLabelAsync(Guid userId, Guid labelId)
        {
            return await _editingRepository.GetLabelAsync(userId, labelId);
        }

        public async Task<Label> GetLabelAsync(Guid userId, int labelCode)
        {
            return await _editingRepository.GetLabelAsync(userId, labelCode);
        }

        public async Task<Label> CreateLabelAsync(Guid userId, string labelName)
        {
            return await _editingRepository.CreateLabelAsync(userId, labelName);
        }

        public async Task DeleteLabelAsync(Guid userId, Guid labelId)
        {
            await _editingRepository.DeleteLabelAsync(userId, labelId);
        }

        public async Task DeleteLabelAsync(Guid userId, int labelCode)
        {
            await _editingRepository.DeleteLabelAsync(userId, labelCode);
        }

        public async Task<PaginatedCollection<Models.Training>> GetTrainingsForUserAsync(Guid userId, GetCollectionRequest request)
        {
            return await _editingRepository.GetTrainingsForUserAsync(userId, request);
        }

        public async Task<Models.Training> GetTrainingAsync(Guid userId, Guid trainingId, bool calculateTime)
        {
            return await _editingRepository.GetTrainingAsync(userId, trainingId, calculateTime);
        }

        public async Task<Models.Training> CreateTrainingAsync(Guid userId, UpdateTrainingRequest request)
        {
            return await _editingRepository.CreateTrainingAsync(userId, request);
        }

        public async Task<Models.Training> UpdateTrainingAsync(Guid userId, UpdateTrainingRequest request)
        {
            return await _editingRepository.UpdateTrainingAsync(userId, request);
        }

        public async Task DeleteTrainingAsync(Guid userId, Guid trainingId)
        {
            await _editingRepository.DeleteTrainingAsync(userId, trainingId);
        }
    }
}
