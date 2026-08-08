using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Models.Utils;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class EditingRepository(
        ApplicationDbContext dbContext, 
        IMapper mapper,
        LabelsService labelsService) : RepositoryBase(dbContext), IEditingRepository
    {
        private readonly IMapper _mapper = mapper;
        private readonly LabelsService _labelsService = labelsService;

        public async Task<Api.Models.PaginatedCollection<Api.Models.Questionnaire>> GetQuestionnairesAsync(Guid userId,
            GetQuestionnairesRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var userIdString = userId.ToAspNetUserIdString();
            var ownerIdString = request.OwnerId?.ToAspNetUserIdString();

            IQueryable<Models.Questionnaire> foundQuestionnaires =
                from questionnaire in DbContext.Questionnaires
                where !questionnaire.ObjectIsRemoved
                select questionnaire;

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

                    (request.PartOfName == null || questionnaire.QuestionnaireName.ToLower().Contains(request.PartOfName.ToLower(), StringComparison.InvariantCulture)) &&

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
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.Owner!.UserName); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.Owner!.UserName); break;
                    }
                    break;
                case QuestionnaireSortField.EditingTime:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionnaires = foundQuestionnaires.OrderBy(p => p.QuestionnaireLastEditingTimeUtc); break;
                        case SortDirection.Descending: foundQuestionnaires = foundQuestionnaires.OrderByDescending(p => p.QuestionnaireLastEditingTimeUtc); break;
                    }
                    break;
            }

            var totalCount = await foundQuestionnaires.CountAsync();

            foundQuestionnaires = foundQuestionnaires.Page(request.PageNumber, request.PageSize);

            var foundGroups = GetQuestionnairesAndCountsOfQuestionsQuery(foundQuestionnaires, userIdString);

            switch (request.SortField)
            {
                case QuestionnaireSortField.Name:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundGroups = foundGroups.OrderBy(p => p.Questionnaire.QuestionnaireName); break;
                        case SortDirection.Descending: foundGroups = foundGroups.OrderByDescending(p => p.Questionnaire.QuestionnaireName); break;
                    }
                    break;
                case QuestionnaireSortField.OwnerName:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundGroups = foundGroups.OrderBy(p => p.Questionnaire.Owner!.UserName); break;
                        case SortDirection.Descending: foundGroups = foundGroups.OrderByDescending(p => p.Questionnaire.Owner!.UserName); break;
                    }
                    break;
                case QuestionnaireSortField.EditingTime:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundGroups = foundGroups.OrderBy(p => p.Questionnaire.QuestionnaireLastEditingTimeUtc); break;
                        case SortDirection.Descending: foundGroups = foundGroups.OrderByDescending(p => p.Questionnaire.QuestionnaireLastEditingTimeUtc); break;
                    }
                    break;
            }

            var foundGroupsResult = await foundGroups.ToListAsync();

            var questionnairesIds = foundGroupsResult.Select(g => g.Questionnaire.QuestionnaireId).ToList();

            var questionnairesWithRelations = await GetQuestionnairesByIdsAsync(questionnairesIds);

            List<Api.Models.Questionnaire> resultList = [];

            foreach (var group in foundGroupsResult)
            {
                var questionnaire = MapQuestionnaireWithCounts(group, questionnairesWithRelations ?? []);
                resultList.Add(questionnaire);
            }

            return new Api.Models.PaginatedCollection<Api.Models.Questionnaire>(resultList, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<Api.Models.Questionnaire?> GetQuestionnaireAsync(Guid userId, Guid questionnaireId, bool calculateTime)
        {
            var result = await GetFullQuestionnaireInfoAsync(userId, questionnaireId, null);
            return result;
        }

        public async Task<Api.Models.Questionnaire?> GetQuestionnaireAsync(Guid userId, int questionnaireCode, bool calculateTime)
        {
            var result = await GetFullQuestionnaireInfoAsync(userId, null, questionnaireCode);
            return result;
        }

        public async Task<Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>> GetQuestionsAsync(Guid userId, GetQuestionsRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

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
                Utils.CheckQuestionnaireAvailabilityForUser(userId, Guid.Parse(questionnaireResult.OwnerId), questionnaireResult.QuestionnaireAvailability);
            else
                throw new ObjectNotFoundException("No questionnaire with such ID or code.");

            //Checking labels restrictions
            if (request.Labels != null)
            {
                if (request.Labels.Count() > Restrictions.MaxCountOfLabelsForQuestionsFilter)
                    throw new ArgumentException($"The count of labels for filtering questions should not exceed {Restrictions.MaxCountOfLabelsForQuestionsFilter}.");

                foreach (var label in request.Labels)
                {
                    if (string.IsNullOrEmpty(label))
                        throw new ArgumentException("Label name cannot be null or empty.");
                    if (label.Length > Restrictions.LabelNameMaxLength)
                        throw new ArgumentException($"Label name cannot exceed {Restrictions.LabelNameMaxLength} characters.");
                }
            }

            //Getting data
            var userIdString = userId.ToAspNetUserIdString();

            var normalizedLabels = request.Labels?.Select(l => Normalize(l)).ToList() ?? [];

            var foundQuestionsAndStatuses =
                from question in DbContext.Questions
                    .Include(q => q.TypedAnswers)
                    .Include(q => q.LabelsForQuestion)
                    .ThenInclude(lq => lq.NormalizedLabel)
                join questionUser in DbContext.QuestionsUsers.Where(qu => qu.UserId == userIdString) on question equals questionUser.Question into questionsUsersGrouped
                from questionUserResult in questionsUsersGrouped.DefaultIfEmpty()
                where !question.ObjectIsRemoved && (request.Labels == null || !request.Labels.Any() || 
                    question.LabelsForQuestion.Any(lq => normalizedLabels.Contains(lq.NormalizedLabel!.NormalizedLabelName)))
                select new
                {
                    Question = question,
                    QuestionUser = questionUserResult
                };

            foundQuestionsAndStatuses =
                from questionAndStatus in foundQuestionsAndStatuses
                where
                    questionAndStatus.Question.QuestionnaireId == questionnaireResult.QuestionnaireId
                select questionAndStatus;

            switch (request.SortField)
            {
                case QuestionSortField.AddedTime:
                default:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderBy(p => p.Question.ObjectCreationTimeUtc); break;
                        case SortDirection.Descending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderByDescending(p => p.Question.ObjectCreationTimeUtc); break;
                    }
                    break;
                case QuestionSortField.Text:
                    switch (request.SortDirection)
                    {
                        case SortDirection.Ascending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderBy(p => p.Question.QuestionText); break;
                        case SortDirection.Descending: foundQuestionsAndStatuses = foundQuestionsAndStatuses.OrderByDescending(p => p.Question.QuestionText); break;
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
            }

            var foundQuestions = foundQuestionsAndStatusesResult.Select(questionAndStatus =>
            {
                var question = _mapper.Map<Api.Models.ExistingQuestion>(questionAndStatus.Question);
                question.MyStatus = _mapper.Map<Api.Models.UserQuestionStatus>(questionAndStatus.QuestionUser);
                return question;
            });

            return new Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>(foundQuestions, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task UpdateQuestionsAsync(Guid userId, UpdateQuestionsRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            CheckIdAndCodeDefinitionRule(request.QuestionnaireId, request.QuestionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            if (request.CreatedQuestions != null)
            {
                foreach (var question in request.CreatedQuestions)
                {
                    CheckQuestionRequest(question);
                }
            }

            if (request.UpdatedQuestions != null)
            {
                foreach (var question in request.UpdatedQuestions)
                {
                    CheckIdAndCodeDefinitionRule(question.Id, question.CodeInQuestionnaire,
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                        new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

                    CheckQuestionRequest(question);
                }
            }

            if (request.DeletedQuestions != null)
            {
                foreach (var question in request.DeletedQuestions)
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

            if (questionnaireResult != null)
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

                        if (question.TypedAnswers != null)
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

                        if (question.Labels != null)
                        {
                            var labelsAndIds = await _labelsService.EnsureLabelsAsync(question.Labels);
                            IEnumerable<Models.QuestionLabel>? questionsLabelsToAdd = null;
                            questionsLabelsToAdd = labelsAndIds.Select(l => new Models.QuestionLabel
                            {
                                QuestionId = addedQuestion.Entity.QuestionId,
                                NormalizedLabelId = l.Value,
                                QuestionLabelName = l.Key,
                                ObjectCreationTimeUtc = DateTime.UtcNow
                            });
                            DbContext.QuestionsLabels.AddRange(questionsLabelsToAdd);
                        }
                    }
                }

                if (request.UpdatedQuestions != null)
                {
                    foreach (var question in request.UpdatedQuestions)
                    {

                        var questionFromDb = await (from questionQuery in DbContext.Questions
                                                        .Include(q => q.TypedAnswers)
                                                    where !questionQuery.ObjectIsRemoved &&
                                                    questionQuery.QuestionnaireId == questionnaireResult.QuestionnaireId &&
                                                    (question.Id == null || questionQuery.QuestionId == question.Id) &&
                                                    (question.CodeInQuestionnaire == null || questionQuery.QuestionQuestionnaireCode == question.CodeInQuestionnaire)
                                                    select questionQuery).SingleOrDefaultAsync() ?? throw new ObjectNotFoundException("One of the updated questions doesn't exist.");

                        if (question.TypedAnswers != null)
                        {
                            var currentTypedAnswersTexts = questionFromDb.TypedAnswers!.Select(a => a.TypedAnswerText).ToList();
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

                        if (question.Labels != null)
                        {
                            var labelsAndIds = await _labelsService.EnsureLabelsAsync(question.Labels);

                            var dbQuestionLabelsToDelete =
                                from questionLabel in DbContext.QuestionsLabels
                                where questionLabel.QuestionId == questionFromDb.QuestionId &&
                                    !labelsAndIds.Any(l => l.Value == questionLabel.NormalizedLabelId)
                                select questionLabel;

                            await dbQuestionLabelsToDelete.ForEachAsync(l =>
                            {
                                DbContext.QuestionsLabels.Remove(l);
                            });

                            var dbQuestionLabelsToAdd = labelsAndIds.Where(l => !questionFromDb.LabelsForQuestion!.Any(q => q.NormalizedLabelId == l.Value)).Select(l => new Models.QuestionLabel
                            {
                                QuestionId = questionFromDb.QuestionId,
                                NormalizedLabelId = l.Value,
                                QuestionLabelName = l.Key,
                                ObjectCreationTimeUtc = DateTime.UtcNow
                            });

                            DbContext.QuestionsLabels.AddRange(dbQuestionLabelsToAdd);
                        }

                        questionFromDb.QuestionEstimatedTrainingTimeSeconds = question.EstimatedTrainingTimeSeconds;
                        questionFromDb.QuestionIsEnabled = question.IsEnabled;
                        questionFromDb.QuestionReference = question.Reference;
                        questionFromDb.QuestionText = question.Text!;
                        questionFromDb.QuestionType = question.Type;
                        questionFromDb.QuestionUntypedAnswer = question.UntypedAnswer;
                    }
                }

                if (request.DeletedQuestions != null)
                {
                    foreach (var question in request.DeletedQuestions)
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

                if (request.CreatedQuestions != null || request.UpdatedQuestions != null || request.DeletedQuestions != null)
                    questionnaireResult.QuestionnaireLastEditingTimeUtc = DateTime.UtcNow;

                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<Api.Models.Questionnaire> CreateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException($"{request.Name} cannot be null.");

            if (userId == Guid.Empty)
                throw new ArgumentException($"{userId} cannot be empty.");

            Models.Questionnaire newQuestionnaire = new()
            {
                QuestionnaireName = request.Name,
                OwnerId = userId.ToAspNetUserIdString()!,
                QuestionnaireAvailability = request.Availability!.Value,
                QuestionnaireLastEditingTimeUtc = DateTime.UtcNow,
                ObjectCreationTimeUtc = DateTime.UtcNow,
                ObjectIsRemoved = false
            };

            var questionnaireEntry = DbContext.Questionnaires.Add(newQuestionnaire);

            await DbContext.SaveChangesAsync();

            var result = questionnaireEntry.Entity;
            return _mapper.Map<Api.Models.Questionnaire>(result);
        }

        public async Task<Api.Models.Questionnaire> UpdateQuestionnaireAsync(Guid userId, UpdateQuestionnaireRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var questionnaireResult = await GetSpecialQuestionnaireInfoAsync(userId, request.Id, request.Code);

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

            if (changed)
            {
                questionnaireResult.QuestionnaireLastEditingTimeUtc = DateTime.UtcNow;
                await DbContext.SaveChangesAsync();
            }

            return _mapper.Map<Api.Models.Questionnaire>(questionnaireResult);
        }

        public async Task DeleteQuestionnaireAsync(Guid userId, Guid questionnaireId) =>
            await DeleteQuestionnaireAsync(userId, questionnaireId, null);

        public async Task DeleteQuestionnaireAsync(Guid userId, int questionnaireCode) =>
            await DeleteQuestionnaireAsync(userId, null, questionnaireCode);

        public async Task UpdateUserQuestionStatusAsync(Guid userId, UpdateUserQuestionStatusesRequest request)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException($"{userId} cannot be empty.");

            ArgumentNullException.ThrowIfNull(request);
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Items should not be null or empty.");

            var requestItems = request.Items.ToList();
            var requestItemsIds = request.Items.Select(i => i.QuestionId).ToList();

            var questionsUsersToUpdate = await (from questionUser in DbContext.QuestionsUsers
                                                .Include(q => q.Question)
                                                .ThenInclude(q => q!.Questionnaire)
                                                where questionUser.UserId == userId.ToAspNetUserIdString() &&
                                                !questionUser.Question!.ObjectIsRemoved &&
                                                requestItemsIds.Contains(questionUser.QuestionId)
                                                select questionUser).ToListAsync();

            foreach (var requestItem in requestItems)
            {
                var questionUserToUpdate = questionsUsersToUpdate.SingleOrDefault(q => q.QuestionId == requestItem.QuestionId);
                if (questionUserToUpdate == null)
                {
                    questionUserToUpdate = _mapper.Map<QuestionUser>(requestItem);
                    questionUserToUpdate.UserId = userId.ToAspNetUserIdString()!;
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

        public async Task<PaginatedCollection<Api.Models.Training>> GetTrainingsForUserAsync(Guid userId, GetCollectionRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var userIdString = userId.ToAspNetUserIdString();

            IQueryable<Models.Training> foundTrainings = from training in DbContext.Trainings
                                                         where !training.ObjectIsRemoved &&
                                                         training.OwnerId == userIdString
                                                         orderby training.TrainingLastTimeUtc descending
                                                         select training;

            var totalCount = await foundTrainings.CountAsync();

            foundTrainings = foundTrainings.Page(request.PageNumber, request.PageSize);

            var foundTrainingsResult = await foundTrainings.ToListAsync();

            return new PaginatedCollection<Api.Models.Training>(_mapper.Map<IEnumerable<Api.Models.Training>>(foundTrainingsResult), totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<Api.Models.Training> GetTrainingAsync(Guid userId, Guid trainingId, bool calculateTime)
        {
            Models.Training? trainingResult = null;
            List<QuestionnaireAndCountsOfQuestions> foundGroupsResult = [];
            IEnumerable<Models.Questionnaire>? questionnairesWithRelations = null;

            using (var transaction = await DbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    trainingResult = await (from training in DbContext.Trainings
                                            where !training.ObjectIsRemoved &&
                                            training.TrainingId == trainingId
                                            select training).SingleOrDefaultAsync();

                    if (trainingResult != null)
                    {
                        var userIdString = userId.ToAspNetUserIdString();

                        var questionnairesQuery = from questionnaire in DbContext.Questionnaires
                                                  from questionnaireForTraining in DbContext.TrainingsQuestionnaires
                                                  where questionnaireForTraining.TrainingId == trainingResult.TrainingId &&
                                                  questionnaireForTraining.QuestionnaireId == questionnaire.QuestionnaireId &&
                                                  !questionnaire.ObjectIsRemoved
                                                  select questionnaire;

                        var questionnairesAndCountsOfQuestions = GetQuestionnairesAndCountsOfQuestionsQuery(questionnairesQuery, userIdString);

                        foundGroupsResult = await questionnairesAndCountsOfQuestions.ToListAsync();

                        var questionnairesIds = foundGroupsResult.Select(g => g.Questionnaire.QuestionnaireId).ToList();

                        questionnairesWithRelations = await GetQuestionnairesByIdsAsync(questionnairesIds);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            if (trainingResult != null)
            {
                Utils.CheckAvailabilityForUser(userId, Guid.Parse(trainingResult.OwnerId), "The user doesn't own the training.");

                List<Api.Models.Questionnaire> resultQuestionnairesList = [];

                foreach (var group in foundGroupsResult)
                {
                    var mappedQuestionnaire = MapQuestionnaireWithCounts(group, questionnairesWithRelations ?? []);
                    resultQuestionnairesList.Add(mappedQuestionnaire);
                }
                Api.Models.Training resultTraining = _mapper.Map<Api.Models.Training>(trainingResult);
                resultTraining.Questionnaires = resultQuestionnairesList;
                return resultTraining;
            }
            else
                throw new ObjectNotFoundException("Training with such ID is not found.");
        }

        public async Task<Api.Models.Training> CreateTrainingAsync(Guid userId, UpdateTrainingRequest request)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException($"{userId} cannot be empty.");

            ArgumentNullException.ThrowIfNull(request);

            const string errorMessageTemplate = "{0} cannot be null.";

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.Name)));
            if (request.LengthType == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.LengthType)));
            if (request.QuestionsCount == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.QuestionsCount)));
            if (request.TimeMinutes == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.TimeMinutes)));
            if (request.NewQuestionsFraction == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.NewQuestionsFraction)));
            if (request.PenaltyQuestionsFraction == null)
                throw new ArgumentException(string.Format(errorMessageTemplate, nameof(request.PenaltyQuestionsFraction)));

            CheckFractions(request.NewQuestionsFraction.Value, request.PenaltyQuestionsFraction.Value);

            var questionnairesIdsList = request?.QuestionnairesIds?.ToList();

            if (questionnairesIdsList != null)
                await CheckQuestionnairesAvailabilityForManagingTrainingsAsync(userId, questionnairesIdsList);

            Models.Training newTraining = new()
            {
                TrainingName = request!.Name,
                TrainingLastTimeUtc = DateTime.UtcNow,
                TrainingLengthType = request.LengthType.Value,
                TrainingQuestionsCount = request.QuestionsCount.Value,
                TrainingTimeMinutes = request.TimeMinutes.Value,
                TrainingNewQuestionsFraction = request.NewQuestionsFraction.Value,
                TrainingPenaltyQuestionsFraction = request.PenaltyQuestionsFraction.Value,
                OwnerId = userId.ToAspNetUserIdString()!,
                ObjectCreationTimeUtc = DateTime.UtcNow,
            };

            var trainingEntry = await DbContext.Trainings.AddAsync(newTraining);

            if (questionnairesIdsList != null)
                foreach (var questionnaireId in questionnairesIdsList)
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
            ArgumentNullException.ThrowIfNull(request);

            var trainingResult = await (from training in DbContext.Trainings
                                        .Include(t => t.QuestionnairesForTraining)
                                        where !training.ObjectIsRemoved && training.TrainingId == request.Id
                                        select training).SingleOrDefaultAsync() ?? throw new ObjectNotFoundException("Training with such ID is not found.");
            if (trainingResult.OwnerId != userId.ToAspNetUserIdString())
                throw new AccessDeniedForUserException(Constants.ExceptionMessages.UserCannotChangeQuestionnaire);

            if (request.NewQuestionsFraction != null || request.PenaltyQuestionsFraction != null)
            {
                CheckFractions(request.NewQuestionsFraction ?? trainingResult.TrainingNewQuestionsFraction,
                    request.PenaltyQuestionsFraction ?? trainingResult.TrainingPenaltyQuestionsFraction);
            }

            bool changed = false;

            if (!string.IsNullOrEmpty(request.Name))
            {
                trainingResult.TrainingName = request.Name;
                changed = true;
            }

            if (request.RefreshLastTime)
            {
                trainingResult.TrainingLastTimeUtc = DateTime.UtcNow;
                changed = true;
            }

            if (request.LengthType != null)
            {
                trainingResult.TrainingLengthType = request.LengthType.Value;
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

            if (request.NewQuestionsFraction != null)
            {
                trainingResult.TrainingNewQuestionsFraction = request.NewQuestionsFraction.Value;
                changed = true;
            }

            if (request.PenaltyQuestionsFraction != null)
            {
                trainingResult.TrainingPenaltyQuestionsFraction = request.PenaltyQuestionsFraction.Value;
                changed = true;
            }

            if (request.QuestionnairesIds != null)
            {
                var currentQuestionnairesIds = trainingResult.QuestionnairesForTraining!.Select(qt => qt.QuestionnaireId).ToList();
                var questionnairesIdsToAdd = request.QuestionnairesIds.Where(q => !currentQuestionnairesIds.Contains(q)).ToList();

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
                Utils.CheckAvailabilityForUser(userId, Guid.Parse(trainingDetails.OwnerId), "The user doesn't have rights to delete the training.");
                trainingDetails.ObjectIsRemoved = true;
                trainingDetails.ObjectRemovalTimeUtc = DateTime.UtcNow;
                await DbContext.SaveChangesAsync();
            }
            else
                throw new ObjectNotFoundException("Training with such ID doesn't exist.");
        }

        private IQueryable<Models.Questionnaire> GetBasicQuestionnaireQuery(Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            return from questionnaire in DbContext.Questionnaires
                   where
                       !questionnaire.ObjectIsRemoved &&
                       (questionnaire.QuestionnaireId == questionnaireId || questionnaire.QuestionnaireCode == questionnaireCode)
                   select questionnaire;
        }

        private async Task<Models.Questionnaire> GetSpecialQuestionnaireInfoAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            CheckIdAndCodeDefinitionRule(questionnaireId, questionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            var questionnaireResult = await GetBasicQuestionnaireQuery(questionnaireId, questionnaireCode).SingleOrDefaultAsync();

            if (questionnaireResult != null)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(userId, Guid.Parse(questionnaireResult.OwnerId), questionnaireResult.QuestionnaireAvailability);
                return questionnaireResult;
            }
            else
                throw new ObjectNotFoundException("Questionnaire with such ID or code is not found.");
        }

        private async Task<Api.Models.Questionnaire> GetFullQuestionnaireInfoAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            CheckIdAndCodeDefinitionRule(questionnaireId, questionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            var groupResult =
                await (from questionnaire in GetBasicQuestionnaireQuery(questionnaireId, questionnaireCode)
                       from question in DbContext.Questions.Where(q => q.QuestionnaireId == questionnaire.QuestionnaireId).DefaultIfEmpty()
                       from questionUser in DbContext.QuestionsUsers.Where(qu => qu.QuestionId == question.QuestionId && qu.UserId == userId.ToAspNetUserIdString()).DefaultIfEmpty()
                       group new { question, questionUser } by questionnaire into questionnaireGroup
                       select new
                       {
                           Questionnaire = questionnaireGroup.Key,
                           QuestionsTotalCount = questionnaireGroup.Count(q => q.question != null),
                           QuestionsNewCount = questionnaireGroup.Count(q => q.question != null && ((q.questionUser != null && q.questionUser.QuestionUserIsNew) || q.questionUser == null)),
                           QuestionsRecheckCount = questionnaireGroup.Count(q => q.questionUser != null && q.questionUser.QuestionUserPenaltyPoints > 0),
                           TotalTrainingTimeSeconds = questionnaireGroup.Where(q => q.question != null).Sum(q => q.question.QuestionEstimatedTrainingTimeSeconds)
                       }).SingleOrDefaultAsync();

            if (groupResult != null)
            {
                Utils.CheckQuestionnaireAvailabilityForUser(userId, Guid.Parse(groupResult.Questionnaire.OwnerId), groupResult.Questionnaire.QuestionnaireAvailability);

                Api.Models.Questionnaire result = _mapper.Map<Api.Models.Questionnaire>(groupResult.Questionnaire);
                result.CountsOfQuestions = new QuestionsCounts
                {
                    Total = groupResult.QuestionsTotalCount,
                    New = groupResult.QuestionsNewCount,
                    Rechecked = groupResult.QuestionsRecheckCount
                };
                result.TotalTrainingTimeSeconds = groupResult.TotalTrainingTimeSeconds;

                return result;
            }
            else
                throw new ObjectNotFoundException("Questionnaire with such ID or code is not found.");
        }

        private async Task DeleteQuestionnaireAsync(Guid userId, Guid? questionnaireId = null, int? questionnaireCode = null)
        {
            CheckIdAndCodeDefinitionRule(questionnaireId, questionnaireCode,
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldNotBeNull),
                new ArgumentException(Constants.ExceptionMessages.IdOrCodeShouldBeNull));

            var questionnaireDetails =
                await (from questionnaire in DbContext.Questionnaires.Include(q => q.Questions)
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

        private static void CheckIdAndCodeDefinitionRule(Guid? id, int? code, Exception exceptionWhenBothNull, Exception exceptionWhenBothNotNull)
        {
            if (id == null && code == null)
                throw exceptionWhenBothNull;
            else if (id != null && code != null)
                throw exceptionWhenBothNotNull;
        }

        private async Task CheckQuestionnairesAvailabilityForManagingTrainingsAsync(Guid userId, List<Guid> questionnairesIds)
        {
            if (questionnairesIds != null)
            {
                var questionnairesFromDb = await (
                    from questionnaire in DbContext.Questionnaires
                    where questionnairesIds.Contains(questionnaire.QuestionnaireId)
                    select questionnaire).ToListAsync();

                foreach (var questionnaireIdFromRequest in questionnairesIds)
                {
                    var questionnaireFromDb = questionnairesFromDb.SingleOrDefault(q => q.QuestionnaireId == questionnaireIdFromRequest);
                    if (questionnaireFromDb != null)
                    {
                        Utils.CheckAvailabilityForUser(userId,
                            Guid.Parse(questionnaireFromDb.OwnerId),
                            $"The user '{userId}' doesn't have a managing access to the questionnaire '{questionnaireIdFromRequest}'.",
                            questionnaireFromDb.QuestionnaireAvailability);
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

            if (question.Labels != null)
            {
                if (question.Labels.Count() > Restrictions.MaxCountOfLabelsPerQuestion)
                    throw new ArgumentException($"The number of labels for a question cannot exceed {Restrictions.MaxCountOfLabelsPerQuestion}.");

                foreach (var label in question.Labels)
                {
                    if (string.IsNullOrEmpty(label))
                        throw new ArgumentException("All labels should have text.");

                    if (label.Length > Restrictions.LabelNameMaxLength)
                        throw new ArgumentException($"The length of a label cannot exceed {Restrictions.LabelNameMaxLength} characters.");
                }
            }
        }

        private static void CheckFractions(decimal newQuestionsFraction, decimal penaltyQuestionsFraction)
        {
            if (newQuestionsFraction + penaltyQuestionsFraction < 0 || newQuestionsFraction + penaltyQuestionsFraction > 1)
                throw new ArgumentException("Sum of new questions fraction and penalty questions fraction cannot be less than 0 or more than 1.");
        }
        private struct QuestionnaireAndCountsOfQuestions
        {
            public Models.Questionnaire Questionnaire { get; set; }
            public int QuestionsTotalCount { get; set; }
            public int QuestionsNewCount { get; set; }
            public int QuestionsRecheckCount { get; set; }
        }

        private IQueryable<QuestionnaireAndCountsOfQuestions> GetQuestionnairesAndCountsOfQuestionsQuery(IQueryable<Models.Questionnaire> questionnairesQuery, string? userIdString)
        {
            return from questionnaire in questionnairesQuery.Include(q => q.Owner)
                   from question in DbContext.Questions.Where(q => q.QuestionnaireId == questionnaire.QuestionnaireId && !q.ObjectIsRemoved).DefaultIfEmpty()
                   from questionUser in DbContext.QuestionsUsers.Where(qu => qu.QuestionId == question.QuestionId && qu.UserId == userIdString).DefaultIfEmpty()
                   from user in DbContext.Users.Where(u => u.Id == questionnaire.OwnerId).DefaultIfEmpty()
                   group new { question, questionUser } by questionnaire into questionnaireGroup
                   select new QuestionnaireAndCountsOfQuestions
                   {
                       Questionnaire = questionnaireGroup.Key,
                       QuestionsTotalCount = questionnaireGroup.Count(q => q.question != null),
                       QuestionsNewCount = questionnaireGroup.Count(q => q.question != null && ((q.questionUser != null && q.questionUser.QuestionUserIsNew) || q.questionUser == null)),
                       QuestionsRecheckCount = questionnaireGroup.Count(q => q.questionUser != null && q.questionUser.QuestionUserPenaltyPoints > 0)
                   };
        }

        private async Task<IEnumerable<Models.Questionnaire>?> GetQuestionnairesByIdsAsync(List<Guid> questionnairesIds)
        {
            return questionnairesIds.Count > 0 ? (await (from questionnaire in DbContext.Questionnaires.Include(q => q.Owner)
                                                         where questionnairesIds.Contains(questionnaire.QuestionnaireId)
                                                         select questionnaire).ToListAsync()) : null;
        }

        private Api.Models.Questionnaire MapQuestionnaireWithCounts(QuestionnaireAndCountsOfQuestions questionnaireAndCountsOfQuestions, IEnumerable<Models.Questionnaire> sourceQuestionnaires)
        {
            Api.Models.Questionnaire questionnaire = _mapper.Map<Api.Models.Questionnaire>(questionnaireAndCountsOfQuestions.Questionnaire);
            questionnaire.CountsOfQuestions = new QuestionsCounts
            {
                Total = questionnaireAndCountsOfQuestions.QuestionsTotalCount,
                New = questionnaireAndCountsOfQuestions.QuestionsNewCount,
                Rechecked = questionnaireAndCountsOfQuestions.QuestionsRecheckCount
            };

            var questionnaireWithRelations = sourceQuestionnaires?.FirstOrDefault(q => questionnaireAndCountsOfQuestions.Questionnaire.QuestionnaireId == q.QuestionnaireId);
            if (questionnaireWithRelations != null)
            {
                questionnaire.OwnerName = questionnaireWithRelations.Owner?.UserName;
            }
            return questionnaire;
        }

        private static string Normalize(string input)
        {
            return input.Trim().ToUpperInvariant();
        }
    }
}
