﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class EditingRepository : RepositoryBase, IEditingRepository
    {
        private readonly IMapper _mapper;
        public EditingRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext) 
        {
            _mapper = mapper;
        }

        public async Task<Api.Models.PaginatedCollection<Api.Models.Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnairesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var userIdString = userId.ToAspNetUserIdString();
            var ownerIdString = request.OwnerId?.ToAspNetUserIdString();

            IQueryable<Models.Questionnaire> foundQuestionnaires =
                from questionnaire in DbContext.Questionnaires
                    .Include(q => q.LabelsForQuestionnaire!)
                    .ThenInclude(el => el.Label)
                where !questionnaire.ObjectIsRemoved
                select questionnaire;

            if (request.LabelsNames != null && request.LabelsNames.Any())
            {
                var labelsIds =
                    from label in DbContext.Labels
                    where !label.ObjectIsRemoved && request.LabelsNames.Contains(label.LabelName)
                    select label.LabelId;

                var entityLabels =
                    from entityLabel in DbContext.EntitiesLabels
                    where labelsIds.Contains(entityLabel.LabelId)
                    select entityLabel;

                var questionnaireIds =
                    from entityLabel in entityLabels
                    group entityLabel by entityLabel.QuestionnaireId into grouped
                    where grouped.Count() >= request.LabelsNames.Count()
                    select grouped.Key;

                foundQuestionnaires =
                    from questionnaire in foundQuestionnaires
                    where
                        questionnaireIds.Contains(questionnaire.QuestionnaireId)
                    select questionnaire;
            }

            foundQuestionnaires =
                from questionnaire in foundQuestionnaires
                where
                    (request.Origin == null ||
                    (request.Origin == Origin.Own && questionnaire.OwnerId == userIdString) ||
                    (request.Origin == Origin.Foreign && questionnaire.OwnerId != userIdString)) &&
                    (ownerIdString == null || request.OwnerId!.Value == default || questionnaire.OwnerId == ownerIdString) &&
                    (request.Availability == null || request.Availability == questionnaire.QuestionnaireAvailability) &&
                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLower().Contains(request.PartOfName.ToLower()))
                select questionnaire;

            switch (request.SortField)
            {
                case QuestionnaireSortField.Name:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.QuestionnaireName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.QuestionnaireName); break;
                    }
                    break;
                case QuestionnaireSortField.OwnerName:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.Owner.UserName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.Owner.UserName); break;
                    }
                    break;
            }

            var totalCount = await foundQuestionnaires.CountAsync();

            foundQuestionnaires = foundQuestionnaires.Page(request.PageNumber, request.PageSize);

            var foundQuestionnairesResult = await foundQuestionnaires.ToListAsync();
            foreach (var questionnaire in foundQuestionnairesResult)
                if (questionnaire?.LabelsForQuestionnaire != null)
                    questionnaire.LabelsForQuestionnaire = questionnaire.LabelsForQuestionnaire.OrderBy(l => l.LabelNumber).ToList();

            return new Api.Models.PaginatedCollection<Api.Models.Questionnaire>(_mapper.Map<IEnumerable<Api.Models.Questionnaire>>(foundQuestionnairesResult), totalCount, request.PageNumber);
        }

        public async Task<Api.Models.Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId) =>
            _mapper.Map<Api.Models.Questionnaire>(await GetQuestionnaireAsync(userId, questionnaireId, null));

        public async Task<Api.Models.Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode) =>
            _mapper.Map<Api.Models.Questionnaire>(await GetQuestionnaireAsync(userId, null, questionnaireCode));

        public async Task<Api.Models.PaginatedCollection<Api.Models.Question>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            CheckIdAndCodeDefinitionRule(request.QuestionnaireId, request.QuestionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            //Checking user rights
            var questionnaireResult =
                await (from questionnaire in DbContext.Questionnaires
                where
                    (request.QuestionnaireId == null || questionnaire.QuestionnaireId == request.QuestionnaireId) &&
                    (request.QuestionnaireCode == null || questionnaire.QuestionnaireCode == request.QuestionnaireCode)
                select questionnaire).SingleOrDefaultAsync();

            if (questionnaireResult != null)
                CheckQuestionnaireAvailabilityForUser(userId, Guid.Parse(questionnaireResult.OwnerId), questionnaireResult.QuestionnaireAvailability);
            else
                throw new ObjectNotFoundException("No questionnaire with such ID or code.");

            //Getting data
            var userIdString = userId.ToAspNetUserIdString();

            var foundQuestionsAndStatuses =
                from question in DbContext.Questions
                    .Include(q => q.TypedAnswers)
                    .Include(q => q.LabelsForQuestion!)
                    .ThenInclude(el => el.Label)
                join questionUser in DbContext.QuestionsUsers on question equals questionUser.Question into questionsUsersGrouped
                from questionUserResult in questionsUsersGrouped.DefaultIfEmpty()
                where !question.ObjectIsRemoved &&
                    (questionUserResult==null || questionUserResult.UserId == userIdString)
                select new
                {
                    Question = question,
                    QuestionUser = questionUserResult
                };

            if (request.LabelsNames != null && request.LabelsNames.Any())
            {
                var labelsIds =
                    from label in DbContext.Labels
                    where !label.ObjectIsRemoved && request.LabelsNames.Contains(label.LabelName)
                    select label.LabelId;

                var entityLabels =
                    from entityLabel in DbContext.EntitiesLabels
                    where labelsIds.Contains(entityLabel.LabelId)
                    select entityLabel;

                var questionIds =
                    from entityLabel in entityLabels
                    group entityLabel by entityLabel.QuestionId into grouped
                    where grouped.Count() >= request.LabelsNames.Count()
                    select grouped.Key;

                foundQuestionsAndStatuses =
                    from questionAndStatus in foundQuestionsAndStatuses
                    where
                        questionIds.Contains(questionAndStatus.Question.QuestionId)
                    select questionAndStatus;
            }

            foundQuestionsAndStatuses =
                from questionAndStatus in foundQuestionsAndStatuses
                where
                    questionAndStatus.Question.QuestionnaireId == questionnaireResult.QuestionnaireId
                select questionAndStatus;

            switch(request.SortField)
            {
                case QuestionSortField.AddedTime:
                    switch(request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderBy(p => p.Question.ObjectCreationTimeUtc);break;
                        case SortDirection.Descending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderByDescending(p => p.Question.ObjectCreationTimeUtc);break;
                    }
                    break;
                case QuestionSortField.Text:
                    switch(request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderBy(p => p.Question.QuestionText);break;
                        case SortDirection.Descending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderByDescending(p=>p.Question.QuestionText);break;
                    }
                    break;
            }

            var totalCount = await foundQuestionsAndStatuses.CountAsync();

            foundQuestionsAndStatuses = foundQuestionsAndStatuses.Page(request.PageNumber, request.PageSize);

            var foundQuestionsAndStatusesResult = await foundQuestionsAndStatuses.ToListAsync();
            foreach (var questionnaire in foundQuestionsAndStatusesResult)
                if (questionnaire?.Question.LabelsForQuestion != null)
                    questionnaire.Question.LabelsForQuestion = questionnaire.Question.LabelsForQuestion.OrderBy(l => l.LabelNumber).ToList();

            var foundQuestions = foundQuestionsAndStatusesResult.Select(questionAndStatus =>
            {
                var question = _mapper.Map<Api.Models.Question>(questionAndStatus.Question);
                question.MyStatus = _mapper.Map<Api.Models.UserQuestionStatus>(questionAndStatus.QuestionUser);
                return question;
            });

            return new Api.Models.PaginatedCollection<Api.Models.Question>(foundQuestions, totalCount, request.PageNumber);
        }

        public Task UpdateQuestionsAsync(Guid userId, UpdateQuestionsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Api.Models.Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException($"{request.Name} cannot be null.");

            var labelsList = request.Labels?.ToList();

            if (labelsList!=null)
                await CheckLabelsAvailabilityForManagingQuestionnairesAsync(userId, labelsList.Select(l=>l.Id).ToList());

            Models.Questionnaire newQuestionnaire = new Models.Questionnaire(request.Name, userId.ToAspNetUserIdString())
            {
                QuestionnaireAvailability = request.Availability!.Value,
                ObjectCreationTimeUtc = DateTime.UtcNow,
                ObjectIsRemoved = false
            };

            var questionnaireEntry = DbContext.Questionnaires.Add(newQuestionnaire);

            if (labelsList != null)
                foreach(var label in labelsList)
                {
                    DbContext.EntitiesLabels.Add(new Models.EntityLabel
                    {
                        EntityType = Enums.EntityType.Questionnaire,
                        QuestionnaireId = questionnaireEntry.Entity.QuestionnaireId,
                        LabelId = label.Id,
                        LabelNumber = label.Number,
                        ParentLabelId = label.ParentLabelId,
                        ObjectCreationTimeUtc = DateTime.UtcNow,
                    });
                }

            await DbContext.SaveChangesAsync();

            var result = questionnaireEntry.Entity;
            return _mapper.Map<Api.Models.Questionnaire>(result);
        }

        public async Task<Api.Models.Questionnaire> UpdateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var questionnaireResult = await GetQuestionnaireAsync(userId,request.Id,request.Code);

            if (questionnaireResult.OwnerId != userId.ToAspNetUserIdString())
                throw new AccessDeniedForUserException("Current user cannot change this questionnaire.");

            bool changed = false;

            if (!string.IsNullOrEmpty(request.Name))
            {
                questionnaireResult.QuestionnaireName = request.Name;
                changed = true;
            }

            if (request.Availability != null)
            {
                questionnaireResult.QuestionnaireAvailability = request.Availability.Value;
                changed = true;
            }

            if (request.Labels != null)
            {
                var currentLabelsIds = questionnaireResult.LabelsForQuestionnaire!.Select(l => l.LabelId).ToList();
                var newLabelsIds = request.Labels!.Select(l=>l.Id).ToList();
                var labelsToAdd = newLabelsIds.Where(l => !currentLabelsIds.Contains(l)).ToList();

                await CheckLabelsAvailabilityForManagingQuestionnairesAsync(userId, labelsToAdd);

                var labelsToDelete = currentLabelsIds.Where(l => !newLabelsIds.Contains(l)).ToList();

                var entitiesLabelsToDelete =
                    from entityLabel in DbContext.EntitiesLabels
                    where labelsToDelete.Contains(entityLabel.LabelId) &&
                        entityLabel.QuestionnaireId == questionnaireResult.QuestionnaireId
                    select entityLabel;

                DbContext.EntitiesLabels.RemoveRange(entitiesLabelsToDelete);

                DbContext.EntitiesLabels.AddRange(labelsToAdd.Select(l => new Models.EntityLabel
                {
                    EntityType = Enums.EntityType.Questionnaire,
                    LabelId = l,
                    QuestionnaireId = questionnaireResult.QuestionnaireId,
                    ObjectCreationTimeUtc = DateTime.UtcNow
                }));

                changed = true;
            }

            if (changed)
                await DbContext.SaveChangesAsync();

            return _mapper.Map<Api.Models.Questionnaire>(questionnaireResult);
        }

        public Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId)=>
            DeleteQuestionnaireAsync(userId,questionnaireId);

        public Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode)=>
            DeleteQuestionnaireAsync(userId,null,questionnaireCode);

        public Task<Api.Models.Question> UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Api.Models.PaginatedCollection<Api.Models.Label>> GetLabelsAsync(Guid userId, GetLabelsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Api.Models.Label> GetLabelAsync(Guid userId, Guid labelId)
        {
            throw new NotImplementedException();
        }

        public Task<Api.Models.Label> GetLabelAsync(Guid userId, int labelCode)
        {
            throw new NotImplementedException();
        }

        public Task<Api.Models.Label> CreateLabelAsync(Guid userId, string labelName)
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

        private async Task<Models.Questionnaire> GetQuestionnaireAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            CheckIdAndCodeDefinitionRule(questionnaireId, questionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            Models.Questionnaire? questionnaireResult =
                await (from questionnaire in DbContext.Questionnaires
                       .Include(q=>q.LabelsForQuestionnaire!)
                       .ThenInclude(el=>el.Label)
                 where
                     !questionnaire.ObjectIsRemoved &&
                     (questionnaireId == null || questionnaire.QuestionnaireId == questionnaireId) &&
                     (questionnaireCode == null || questionnaire.QuestionnaireCode == questionnaireCode)
                 select questionnaire).SingleOrDefaultAsync();

            if (questionnaireResult != null)
            {
                CheckQuestionnaireAvailabilityForUser(userId, Guid.Parse(questionnaireResult.OwnerId), questionnaireResult.QuestionnaireAvailability);
                return questionnaireResult;
            }
            else
                throw new ObjectNotFoundException("Questionnaire with such ID or code is not found.");
        }

        private async Task<Api.Models.Questionnaire> DeleteQuestionnaireAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            CheckIdAndCodeDefinitionRule(questionnaireId, questionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull), 
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            var questionnaireDetails =
                await (from questionnaire in DbContext.Questionnaires.Include(q=>q.Questions)
                       where
                           !questionnaire.ObjectIsRemoved &&
                           (questionnaireId == null || questionnaire.QuestionnaireId == questionnaireId) &&
                           (questionnaireCode == null || questionnaire.QuestionnaireCode == questionnaireCode)
                       select questionnaire).SingleOrDefaultAsync();
            if (questionnaireDetails != null)
            {
                if (Guid.TryParse(questionnaireDetails.OwnerId, out Guid ownerGuid) && ownerGuid == userId)
                {
                    questionnaireDetails.ObjectIsRemoved = true;
                    questionnaireDetails.ObjectRemovalTimeUtc = DateTime.UtcNow;
                    await DbContext.SaveChangesAsync();
                }
                else
                    throw new AccessDeniedForUserException("The user doesn't have rights to delete the questionnaire.");
            }
            else
                throw new ObjectNotFoundException("Questionnaire with such ID or Code doesn't exist.");

            return _mapper.Map<Api.Models.Questionnaire>(questionnaireDetails);
        }

        private static void CheckQuestionnaireAvailabilityForUser(Guid currentUserId, Guid questionnaireOwnerId, Availability questionnaireAvailability)
        {
            if (questionnaireAvailability == Availability.Private && questionnaireOwnerId != currentUserId)
                throw new AccessDeniedForUserException("Unable to get details about a private questionnaire to a foreign user.");
        }

        private static void CheckIdAndCodeDefinitionRule(Guid? id, int? code, Exception exceptionWhenBothNull, Exception exceptionWhenBothNotNull)
        {
            if (id == null && code == null)
                throw exceptionWhenBothNull;
            else if (id != null && code != null)
                throw exceptionWhenBothNotNull;
        }

        private async Task CheckLabelsAvailabilityForManagingQuestionnairesAsync(Guid userId, List<Guid> labelsIds)
        {
            if (labelsIds!=null)
                foreach(var labelIdFromParameter in labelsIds)
                {
                    var labelFromDb =
                        await (from label in DbContext.Labels
                         where
                             label.LabelId == labelIdFromParameter
                               select label).SingleOrDefaultAsync();
                    if (labelFromDb != null)
                    {
                        if (!Guid.TryParse(labelFromDb.OwnerId, out Guid ownerGuid) || ownerGuid != userId)
                            throw new AccessDeniedForUserException($"The user '{userId}' doesn't have a managing access to the label '{labelIdFromParameter}'.");
                    }
                    else
                        throw new ObjectNotFoundException($"The label '{labelIdFromParameter}' is not found.");
                }
        }
    }
}
