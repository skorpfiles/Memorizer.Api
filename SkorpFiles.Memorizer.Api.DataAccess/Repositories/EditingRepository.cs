using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
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
                    ((request.Origin == null) ||
                    (request.Origin == Origin.Own && questionnaire.OwnerId == userIdString) ||
                    (request.Origin == Origin.Foreign && questionnaire.OwnerId != userIdString)) &&

                    ((ownerIdString == null) ||
                    (request.OwnerId!.Value == default) ||
                    (questionnaire.OwnerId == ownerIdString)) &&

                    ((request.Availability == null) ||
                    (request.Availability == questionnaire.QuestionnaireAvailability)) &&

                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLower().Contains(request.PartOfName.ToLower())) &&

                    //If the questionnaire is private, return only own questionnaires! Edit carefully!
                    ((questionnaire.QuestionnaireAvailability == Availability.Private && questionnaire.OwnerId == userIdString) ||
                    (questionnaire.QuestionnaireAvailability == Availability.Public))
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

        public async Task<Api.Models.Questionnaire> GetQuestionnaireAsync(Guid userId, Guid questionnaireId)
        {
            var result = _mapper.Map<Api.Models.Questionnaire>(await GetQuestionnaireAsync(userId, questionnaireId, null));
            result.QuestionsCount = await GetQuestionsCountInQuestionnaireAsync(result.Id ?? default);
            return result;
        }

        public async Task<Api.Models.Questionnaire> GetQuestionnaireAsync(Guid userId, int questionnaireCode)
        {
            var result = _mapper.Map<Api.Models.Questionnaire>(await GetQuestionnaireAsync(userId, null, questionnaireCode));
            result.QuestionsCount = await GetQuestionsCountInQuestionnaireAsync(result.Id ?? default);
            return result;
        }

        public async Task<Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
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
                default:
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
            {
                if (questionnaire?.Question.TypedAnswers != null)
                    questionnaire.Question.TypedAnswers = questionnaire.Question.TypedAnswers.Where(a => !a.ObjectIsRemoved).ToList();

                if (questionnaire?.Question.LabelsForQuestion != null)
                    questionnaire.Question.LabelsForQuestion = questionnaire.Question.LabelsForQuestion.OrderBy(l => l.LabelNumber).ToList();
            }

            var foundQuestions = foundQuestionsAndStatusesResult.Select(questionAndStatus =>
            {
                var question = _mapper.Map<Api.Models.ExistingQuestion>(questionAndStatus.Question);
                question.MyStatus = _mapper.Map<Api.Models.UserQuestionStatus>(questionAndStatus.QuestionUser);
                return question;
            });

            return new Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>(foundQuestions, totalCount, request.PageNumber);
        }

        public async Task UpdateQuestionsAsync(Guid userId, UpdateQuestionsRequest request)
        {
            if (request==null)
                throw new ArgumentNullException(nameof(request));

            CheckIdAndCodeDefinitionRule(request.QuestionnaireId, request.QuestionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            if (request.CreatedQuestions != null)
            {
                foreach (var question in request.CreatedQuestions)
                {
                    CheckQuestionRequest(question);

                    if (question.LabelsIds != null)
                        await CheckLabelsAvailabilityForManagingEntitiesAsync(userId, question.LabelsIds.ToList());
                }
            }

            if (request.UpdatedQuestions!=null)
            {
                foreach(var question in request.UpdatedQuestions)
                {
                    CheckIdAndCodeDefinitionRule(question.Id, question.CodeInQuestionnaire,
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

                    CheckQuestionRequest(question);
                }
            }

            if (request.DeletedQuestions != null)
            {
                foreach(var question in request.DeletedQuestions)
                {
                    CheckIdAndCodeDefinitionRule(question.Id, question.CodeInQuestionnaire,
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull), 
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));
                }
            }

            var questionnaireResult =
                await (from questionnaire in DbContext.Questionnaires
                        where
                            !questionnaire.ObjectIsRemoved &&
                            (request.QuestionnaireId == null || questionnaire.QuestionnaireId == request.QuestionnaireId) &&
                            (request.QuestionnaireCode == null || questionnaire.QuestionnaireCode == request.QuestionnaireCode)
                        select questionnaire).SingleOrDefaultAsync();
            if (questionnaireResult!=null)
            {
                if (questionnaireResult.OwnerId != userId.ToAspNetUserIdString())
                    throw new AccessDeniedForUserException(Constants.ExceptionMessages.UserCannotChangeQuestionnaire);

                if (request.CreatedQuestions != null)
                {
                    foreach (var question in request.CreatedQuestions)
                    {
                        var questionForDb = _mapper.Map<DataAccess.Models.Question>(question);
                        questionForDb.QuestionId = Guid.Empty;
                        questionForDb.QuestionnaireId = questionnaireResult.QuestionnaireId;
                        questionForDb.ObjectCreationTimeUtc = DateTime.UtcNow;

                        var addedQuestion = DbContext.Questions.Add(questionForDb);

                        if (question.LabelsIds!=null)
                        {
                            IEnumerable<EntityLabel>? entitiesLabelsToAdd = null;
                            entitiesLabelsToAdd = question.LabelsIds.Select(id => new EntityLabel
                            {
                                QuestionId = addedQuestion.Entity.QuestionId,
                                LabelId = id,
                                EntityType = Enums.EntityType.Question,
                                ObjectCreationTimeUtc = DateTime.UtcNow
                            });
                            DbContext.EntitiesLabels.AddRange(entitiesLabelsToAdd);
                        }

                        if (question.TypedAnswers!=null)
                        {
                            IEnumerable<Models.TypedAnswer>? typedAnswersToAdd = null;
                            typedAnswersToAdd = question.TypedAnswers.Select(a => new Models.TypedAnswer(a)
                            {
                                QuestionId = addedQuestion.Entity.QuestionId,
                                ObjectCreationTimeUtc = DateTime.UtcNow,
                                ObjectIsRemoved = false
                            });
                            DbContext.TypedAnswers.AddRange(typedAnswersToAdd);
                        }
                    }
                }

                if (request.UpdatedQuestions !=null)
                {
                    foreach(var question in request.UpdatedQuestions)
                    {

                        var questionFromDb = await (from questionQuery in DbContext.Questions
                                                        .Include(q=>q.LabelsForQuestion)
                                                        .Include(q=>q.TypedAnswers)
                                                    where !questionQuery.ObjectIsRemoved &&
                                                    questionQuery.QuestionnaireId == questionnaireResult.QuestionnaireId &&
                                                    (question.Id == null || questionQuery.QuestionId == question.Id) &&
                                                    (question.CodeInQuestionnaire == null || questionQuery.QuestionQuestionnaireCode == question.CodeInQuestionnaire)
                                                    select questionQuery).SingleOrDefaultAsync();

                        if (questionFromDb == null)
                            throw new ObjectNotFoundException("One of the updated questions doesn't exist.");

                        if (question.LabelsIds != null)
                        {
                            var currentLabelsIds = questionFromDb.LabelsForQuestion!.Select(l => l.LabelId).ToList();
                            var newLabelsIds = question.LabelsIds.ToList();
                            var labelsToAdd = newLabelsIds.Where(l => !currentLabelsIds.Contains(l)).ToList();

                            await CheckLabelsAvailabilityForManagingEntitiesAsync(userId, labelsToAdd);

                            var labelsToDelete = currentLabelsIds.Where(l => !newLabelsIds.Contains(l)).ToList();

                            var entitiesLabelsToDelete =
                                from entityLabel in DbContext.EntitiesLabels
                                where labelsToDelete.Contains(entityLabel.LabelId) &&
                                    entityLabel.QuestionId == questionFromDb.QuestionId
                                select entityLabel;

                            DbContext.EntitiesLabels.RemoveRange(entitiesLabelsToDelete);

                            DbContext.EntitiesLabels.AddRange(labelsToAdd.Select(l => new Models.EntityLabel
                            {
                                EntityType = Enums.EntityType.Question,
                                LabelId = l,
                                QuestionId = questionFromDb.QuestionId,
                                ObjectCreationTimeUtc = DateTime.UtcNow
                            }));
                        }

                        if (question.TypedAnswers!=null)
                        {
                            var currentTypedAnswersTexts = questionFromDb.TypedAnswers!.Select(a=>a.TypedAnswerText).ToList();
                            var newTypedAnswersTexts = question.TypedAnswers.ToList();
                            var typedAnswersToAdd = newTypedAnswersTexts.Where(a => !currentTypedAnswersTexts.Contains(a)).ToList();
                            var typedAnswersToDelete = currentTypedAnswersTexts.Where(a => !newTypedAnswersTexts.Contains(a)).ToList();
                            
                            var dbTypedAnswersToDelete =
                                from typedAnswer in DbContext.TypedAnswers
                                where !typedAnswer.ObjectIsRemoved &&
                                    typedAnswersToDelete.Contains(typedAnswer.TypedAnswerText) &&
                                    typedAnswer.QuestionId == questionFromDb.QuestionId
                                select typedAnswer;

                            await dbTypedAnswersToDelete.ForEachAsync(a =>
                            {
                                a.ObjectIsRemoved = true;
                                a.ObjectRemovalTimeUtc = DateTime.UtcNow;
                            });

                            DbContext.TypedAnswers.AddRange(typedAnswersToAdd.Select(a => new Models.TypedAnswer(a)
                            {
                                QuestionId = questionFromDb.QuestionId,
                                ObjectCreationTimeUtc = DateTime.UtcNow,
                                ObjectIsRemoved = false
                            }));
                        }

                        questionFromDb.QuestionEstimatedTrainingTimeSeconds = question.EstimatedTrainingTimeSeconds;
                        questionFromDb.QuestionIsEnabled = question.IsEnabled;
                        questionFromDb.QuestionReference = question.Reference;
                        questionFromDb.QuestionText = question.Text!;
                        questionFromDb.QuestionType = question.Type;
                        questionFromDb.QuestionUntypedAnswer = question.UntypedAnswer;
                    }
                }

                if (request.DeletedQuestions!=null)
                {
                    foreach(var question in request.DeletedQuestions)
                    {
                        var questionFromDb = await (from questionQuery in DbContext.Questions
                                where !questionQuery.ObjectIsRemoved &&
                                questionQuery.QuestionnaireId == questionnaireResult.QuestionnaireId &&
                                (question.Id == null || questionQuery.QuestionId == question.Id) &&
                                (question.CodeInQuestionnaire == null || questionQuery.QuestionQuestionnaireCode == question.CodeInQuestionnaire)
                                select questionQuery).SingleOrDefaultAsync();

                        if (questionFromDb != null)
                        {
                            questionFromDb.ObjectIsRemoved = true;
                            questionFromDb.ObjectRemovalTimeUtc = DateTime.UtcNow;
                        }
                        else
                            throw new ObjectNotFoundException("One of the deleted questions doesn't exist.");
                    }
                }
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<Api.Models.Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException($"{request.Name} cannot be null.");

            var labelsList = request.Labels?.ToList();

            if (labelsList!=null)
                await CheckLabelsAvailabilityForManagingEntitiesAsync(userId, labelsList.Select(l=>l.Id).ToList());

            Models.Questionnaire newQuestionnaire = new Models.Questionnaire
            {
                QuestionnaireName = request.Name,
                OwnerId = userId.ToAspNetUserIdString(),
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
                throw new AccessDeniedForUserException(Constants.ExceptionMessages.UserCannotChangeQuestionnaire);

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

                await CheckLabelsAvailabilityForManagingEntitiesAsync(userId, labelsToAdd);

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

        public async Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId) =>
            await DeleteQuestionnaireAsync(userId, questionnaireId, null);

        public async Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode)=>
            await DeleteQuestionnaireAsync(userId,null,questionnaireCode);

        public async Task UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Items should not be null or empty.");
            
            var requestItems = request.Items.ToList();
            var requestItemsIds = request.Items.Select(i=>i.QuestionId).ToList();

            var questionsUsersToUpdate = await (from questionUser in DbContext.QuestionsUsers
                                                .Include(q=>q.Question)
                                                .ThenInclude(q=>q.Questionnaire)
                                         where questionUser.UserId == userId.ToAspNetUserIdString() &&
                                         !questionUser.Question!.ObjectIsRemoved &&
                                         requestItemsIds.Contains(questionUser.QuestionId)
                                         select questionUser).ToListAsync();

            foreach(var requestItem in requestItems)
            {
                var questionUserToUpdate = questionsUsersToUpdate.SingleOrDefault(q=>q.QuestionId == requestItem.QuestionId);
                if (questionUserToUpdate==null)
                {
                    questionUserToUpdate = _mapper.Map<QuestionUser>(requestItem);
                    questionUserToUpdate.UserId = userId.ToAspNetUserIdString();
                    questionUserToUpdate.ObjectCreationTimeUtc = DateTime.UtcNow;
                    DbContext.Add(questionUserToUpdate);
                }
                else
                {
                    questionUserToUpdate.QuestionUserRating = requestItem.Rating;
                    questionUserToUpdate.QuestionUserIsNew = requestItem.IsNew;
                    questionUserToUpdate.QuestionUserPenaltyPoints = requestItem.PenaltyPoints;
                }
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task<Api.Models.PaginatedCollection<Api.Models.Label>> GetLabelsAsync(Guid userId, GetLabelsRequest request)
        {
            if (request==null)
                throw new ArgumentNullException(nameof(request));

            var userIdString = userId.ToAspNetUserIdString();

            var foundLabels = from label in DbContext.Labels
                              where !label.ObjectIsRemoved &&
                              (request.Origin == null ||
                              (request.Origin == Origin.Own && label.OwnerId == userIdString) ||
                              (request.Origin == Origin.Foreign && label.OwnerId != userIdString)) &&
                              (request.PartOfName == null || label.LabelName.ToLower().Contains(request.PartOfName.ToLower()))
                              select label;

            var totalCount = await foundLabels.CountAsync();

            foundLabels = foundLabels.Page(request.PageNumber, request.PageSize);

            var foundLabelsResult = await foundLabels.ToListAsync();

            return new Api.Models.PaginatedCollection<Api.Models.Label>(_mapper.Map<IEnumerable<Api.Models.Label>>(foundLabelsResult), totalCount, request.PageNumber);
        }

        public async Task<Api.Models.Label> GetLabelAsync(Guid userId, Guid labelId) =>
            _mapper.Map<Api.Models.Label>(await GetLabelAsync(userId, labelId, null));

        public async Task<Api.Models.Label> GetLabelAsync(Guid userId, int labelCode) =>
            _mapper.Map<Api.Models.Label>(await GetLabelAsync(userId, null, labelCode));

        public async Task<Api.Models.Label> CreateLabelAsync(Guid userId, string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
                throw new ArgumentNullException(nameof(labelName));

            Models.Label newLabel = new Models.Label()
            {
                LabelName = labelName,
                OwnerId = userId.ToAspNetUserIdString(),
                ObjectCreationTimeUtc = DateTime.UtcNow
            };

            var labelEntry = DbContext.Labels.Add(newLabel);

            await DbContext.SaveChangesAsync();

            var result = labelEntry.Entity;
            return _mapper.Map<Api.Models.Label>(result);
        }

        public async Task DeleteLabelAsync(Guid userId, Guid labelId) =>
            await DeleteLabelAsync(userId, labelId, null);

        public async Task DeleteLabelAsync(Guid userId, int labelCode) =>
            await DeleteLabelAsync(userId, null, labelCode);

        public async Task<PaginatedCollection<Api.Models.Training>> GetTrainingsForUserAsync(Guid userId, GetCollectionRequest request)
        {
            if (request==null)
                throw new ArgumentNullException(nameof(request));

            var userIdString = userId.ToAspNetUserIdString();

            IQueryable<Models.Training> foundTrainings = from training in DbContext.Trainings
                                                         where !training.ObjectIsRemoved &&
                                                         training.OwnerId == userIdString
                                                         orderby training.TrainingLastTimeUtc descending
                                                         select training;

            var totalCount = await foundTrainings.CountAsync();

            foundTrainings = foundTrainings.Page(request.PageNumber, request.PageSize);

            var foundTrainingsResult = await foundTrainings.ToListAsync();

            return new PaginatedCollection<Api.Models.Training>(_mapper.Map<IEnumerable<Api.Models.Training>>(foundTrainingsResult), totalCount, request.PageNumber);
        }

        public async Task<Api.Models.Training> GetTrainingAsync(Guid userId, Guid trainingId)
        {
            Models.Training? trainingResult = await (from training in DbContext.Trainings
                                                     .Include(t=>t.QuestionnairesForTraining!)
                                                     .ThenInclude(qt=>qt.Questionnaire)
                                                     where !training.ObjectIsRemoved &&
                                                     training.TrainingId == trainingId
                                                     select training
                                                     ).SingleOrDefaultAsync();
            if (trainingResult!=null)
            {
                CheckAvailabilityForUser(userId, Guid.Parse(trainingResult.OwnerId), "The user doesn't own the training.");
                return _mapper.Map<Api.Models.Training>(trainingResult);
            }
            else
                throw new ObjectNotFoundException("Training with such ID is not found.");
        }

        public async Task<Api.Models.Training> CreateTrainingAsync(Guid userId, UpdateTrainingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            const string errorMessageTemplate = "{0} cannot be null.";

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.Name)));
            if (request.LengthType == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.LengthType)));
            if (request.QuestionsCount == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.QuestionsCount)));
            if (request.TimeMinutes == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.TimeMinutes)));

            var questionnairesIdsList = request?.QuestionnairesIds?.ToList();

            if (questionnairesIdsList != null)
                await CheckQuestionnairesAvailabilityForManagingTrainingsAsync(userId, questionnairesIdsList);

            Models.Training newTraining = new Models.Training
            {
                TrainingName = request!.Name,
                TrainingLastTimeUtc = DateTime.UtcNow,
                TrainingLengthType = request.LengthType.Value,
                TrainingQuestionsCount = request.QuestionsCount.Value,
                TrainingTimeMinutes = request.TimeMinutes.Value,
                OwnerId = userId.ToAspNetUserIdString(),
                ObjectCreationTimeUtc = DateTime.UtcNow,
            };

            var trainingEntry = await DbContext.Trainings.AddAsync(newTraining);

            if (questionnairesIdsList != null)
                foreach(var questionnaireId in questionnairesIdsList)
                {
                    await DbContext.TrainingsQuestionnaires.AddAsync(new Models.TrainingQuestionnaire
                    {
                        QuestionnaireId = questionnaireId,
                        TrainingId = trainingEntry.Entity.TrainingId,
                        ObjectCreationTimeUtc = DateTime.UtcNow
                    });
                }

            await DbContext.SaveChangesAsync();

            var result = trainingEntry.Entity;
            return _mapper.Map<Api.Models.Training>(result);
        }

        public async Task<Api.Models.Training> UpdateTrainingAsync(Guid userId, UpdateTrainingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var trainingResult = await (from training in DbContext.Trainings
                                        .Include(t => t.QuestionnairesForTraining)
                                        where !training.ObjectIsRemoved && training.TrainingId == request.Id
                                        select training).SingleOrDefaultAsync();

            if (trainingResult == null)
                throw new ObjectNotFoundException("Training with such ID is not found.");

            if (trainingResult.OwnerId != userId.ToAspNetUserIdString())
                    throw new AccessDeniedForUserException(Constants.ExceptionMessages.UserCannotChangeQuestionnaire);

            bool changed = false;

            if (!string.IsNullOrEmpty(request.Name))
            {
                trainingResult.TrainingName = request.Name;
                changed = true;
            }

            if (request.LastTimeUtc != null)
            {
                trainingResult.TrainingLastTimeUtc = request.LastTimeUtc.Value;
                changed = true;
            }

            if (request.QuestionsCount != null)
            {
                trainingResult.TrainingQuestionsCount = request.QuestionsCount.Value;
                changed = true;
            }

            if (request.TimeMinutes != null)
            {
                trainingResult.TrainingTimeMinutes = request.TimeMinutes.Value;
                changed = true;
            }

            if (request.QuestionnairesIds != null)
            {
                var currentQuestionnairesIds = trainingResult.QuestionnairesForTraining!.Select(qt=>qt.QuestionnaireId).ToList();
                var questionnairesIdsToAdd = currentQuestionnairesIds.Where(q => !request.QuestionnairesIds.Contains(q)).ToList();

                await CheckQuestionnairesAvailabilityForManagingTrainingsAsync(userId, questionnairesIdsToAdd);

                var questionnairesToDelete = currentQuestionnairesIds.Where(q => !request.QuestionnairesIds.Contains(q)).ToList();

                var trainingsQuestionnairesToDelete = 
                    from trainingQuestionnaire in DbContext.TrainingsQuestionnaires
                    where questionnairesToDelete.Contains(trainingQuestionnaire.QuestionnaireId) &&
                    trainingQuestionnaire.TrainingId == trainingResult.TrainingId
                    select trainingQuestionnaire;

                DbContext.TrainingsQuestionnaires.RemoveRange(trainingsQuestionnairesToDelete);

                DbContext.TrainingsQuestionnaires.AddRange(questionnairesIdsToAdd.Select(q => new Models.TrainingQuestionnaire
                {
                    QuestionnaireId = q,
                    TrainingId = trainingResult.TrainingId,
                    ObjectCreationTimeUtc = DateTime.UtcNow
                }));

                changed = true;
            }

            if (changed)
                await DbContext.SaveChangesAsync();

            return _mapper.Map<Api.Models.Training>(trainingResult);
        }

        public async Task DeleteTrainingAsync(Guid userId, Guid trainingId)
        {
            var trainingDetails =
                await (from training in DbContext.Trainings.Include(t => t.QuestionnairesForTraining)
                       where !training.ObjectIsRemoved &&
                       training.TrainingId == trainingId
                       select training).SingleOrDefaultAsync();
            if (trainingDetails != null)
            {
                CheckAvailabilityForUser(userId, Guid.Parse(trainingDetails.OwnerId), "The user doesn't have rights to delete the training.");
                trainingDetails.ObjectIsRemoved = true;
                trainingDetails.ObjectRemovalTimeUtc = DateTime.UtcNow;
            }
            else
                throw new ObjectNotFoundException("Training with such ID doesn't exist.");
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

        private async Task<int> GetQuestionsCountInQuestionnaireAsync(Guid questionnaireId)
        {
            return await (from question in DbContext.Questions
                          where
                              !question.ObjectIsRemoved &&
                              question.QuestionnaireId == questionnaireId
                          select question)
                   .CountAsync();
        }

        private async Task DeleteQuestionnaireAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
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
        }

        private static void CheckQuestionnaireAvailabilityForUser(Guid currentUserId, Guid ownerId, Availability availability)
        {
            CheckAvailabilityForUser(currentUserId, ownerId, "Unable to get details about a private questionnaire to a foreign user.", availability);
        }

        private static void CheckAvailabilityForUser(Guid currentUserId, Guid ownerId, string errorMessage, Availability? availability = null)
        {
            if (availability != null)
            {
                if (availability == Availability.Private && ownerId != currentUserId)
                    throw new AccessDeniedForUserException(errorMessage);
            }
            else
            {
                if (ownerId != currentUserId)
                    throw new AccessDeniedForUserException(errorMessage);
            }
        }

        private static void CheckIdAndCodeDefinitionRule(Guid? id, int? code, Exception exceptionWhenBothNull, Exception exceptionWhenBothNotNull)
        {
            if (id == null && code == null)
                throw exceptionWhenBothNull;
            else if (id != null && code != null)
                throw exceptionWhenBothNotNull;
        }

        private async Task CheckLabelsAvailabilityForManagingEntitiesAsync(Guid userId, List<Guid> labelsIds)
        {
            if (labelsIds!=null)
            {
                var labelsFromDb = await (
                    from label in DbContext.Labels
                    where labelsIds.Contains(label.LabelId)
                    select label).ToListAsync();

                foreach(var labelIdFromRequest in labelsIds)
                {
                    var labelFromDb = labelsFromDb.SingleOrDefault(l => l.LabelId == labelIdFromRequest);
                    if (labelFromDb != null)
                    {
                        CheckAvailabilityForUser(userId, Guid.Parse(labelFromDb.OwnerId), $"The user '{userId}' doesn't have a managing access to the label '{labelIdFromRequest}'.");
                    }
                    else
                    {
                        throw new ObjectNotFoundException($"The label '{labelIdFromRequest}' is not found.");
                    }
                }
            }
        }

        private async Task CheckQuestionnairesAvailabilityForManagingTrainingsAsync(Guid userId, List<Guid> questionnairesIds)
        {
            if (questionnairesIds != null)
            {
                var questionnairesFromDb = await (
                    from questionnaire in DbContext.Questionnaires
                    where questionnairesIds.Contains(questionnaire.QuestionnaireId)
                    select questionnaire).ToListAsync();
                
                foreach(var questionnaireIdFromRequest in questionnairesIds)
                {
                    var questionnaireFromDb = questionnairesFromDb.SingleOrDefault(q => q.QuestionnaireId == questionnaireIdFromRequest);
                    if (questionnaireFromDb != null)
                    {
                        CheckAvailabilityForUser(userId, Guid.Parse(questionnaireFromDb.OwnerId), $"The user '{userId}' doesn't have a managing access to the questionnaire '{questionnaireIdFromRequest}'.");
                    }
                    else
                    {
                        throw new ObjectNotFoundException($"The questionnaire '{questionnaireIdFromRequest}' is not found.");
                    }
                }
            }
        }

        private static void CheckQuestionRequest(Api.Models.QuestionToUpdate question)
        {
            if (question.Text == null)
                throw new ArgumentException("All questions should have text.");
        }

        private async Task<Models.Label> GetLabelAsync(Guid userId, Guid? labelId = null, int? labelCode = null)
        {
            CheckIdAndCodeDefinitionRule(labelId, labelCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            Models.Label? labelResult = await (from label in DbContext.Labels
                                               where !label.ObjectIsRemoved &&
                                               (labelId == null || label.LabelId == labelId) &&
                                               (labelCode == null || label.LabelCode == labelCode)
                                               select label).SingleOrDefaultAsync();

            if (labelResult!=null)
            {
                return labelResult;
            }
            else
                throw new ObjectNotFoundException("Label with such ID or code is not found.");
        }

        private async Task DeleteLabelAsync(Guid userId, Guid? labelId=null, int? labelCode=null)
        {
            CheckIdAndCodeDefinitionRule(labelId, labelCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            var labelDetails =
                await (from label in DbContext.Labels
                       where
                           !label.ObjectIsRemoved &&
                           (labelId == null || label.LabelId == labelId) &&
                           (labelCode == null || label.LabelCode == labelCode)
                       select label).SingleOrDefaultAsync();

            if (labelDetails != null)
            {
                if (Guid.TryParse(labelDetails.OwnerId, out Guid ownerGuid) && ownerGuid == userId)
                {
                    labelDetails.ObjectIsRemoved = true;
                    labelDetails.ObjectRemovalTimeUtc = DateTime.UtcNow;
                    await DbContext.SaveChangesAsync();
                }
                else
                    throw new AccessDeniedForUserException("The user doesn't have rights to delete the questionnaire.");
            }
            else
                throw new ObjectNotFoundException("Questionnaire with such ID or Code doesn't exist.");
        }
    }
}
