using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;
using System.Diagnostics.CodeAnalysis;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public static class RequestsMapper
    {
        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this GetQuestionnairesRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.GetQuestionnairesRequest))
            {
                var result = new Api.Models.RequestModels.GetQuestionnairesRequest
                {
                    PageNumber = source.PageNumber,
                    PageSize = source.PageSize,
                    OwnerId = source.OwnerId,
                    PartOfName = source.PartOfName,
                    LabelsNames = source.LabelsNames
                };

                if (Enum.TryParse<Origin>(source.Origin, true, out var origin))
                    result.Origin = origin;
                if (Enum.TryParse<Availability>(source.Availability, true, out var availability))
                    result.Availability = availability;
                if (Enum.TryParse<QuestionnaireSortField>(source.SortField, true, out var sortField))
                    result.SortField = sortField;
                if (Enum.TryParse<SortDirection>(source.SortDirection, true, out var sortDirection))
                    result.SortDirection = sortDirection;

                return (TDestination)(object)result;
            }

            throw MappingNotSupported<GetQuestionnairesRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this GetQuestionsRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.GetQuestionsRequest))
            {
                var result = new Api.Models.RequestModels.GetQuestionsRequest
                {
                    PageNumber = source.PageNumber,
                    PageSize = source.PageSize,
                    QuestionnaireId = source.QuestionnaireId,
                    QuestionnaireCode = source.QuestionnaireCode,
                    LabelsNames = source.LabelsNames
                };

                if (Enum.TryParse<QuestionSortField>(source.SortField, true, out var sortField))
                    result.SortField = sortField;
                if (Enum.TryParse<SortDirection>(source.SortDirection, true, out var sortDirection))
                    result.SortDirection = sortDirection;

                return (TDestination)(object)result;
            }

            throw MappingNotSupported<GetQuestionsRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this PutQuestionnaireRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.UpdateQuestionnaireRequest))
            {
                var result = new Api.Models.RequestModels.UpdateQuestionnaireRequest
                {
                    Name = source.Name,
                    Labels = source.Labels?.Select(l => l.MapTo<Api.Models.LabelInQuestionnaire>()).ToList()
                };

                if (Enum.TryParse<Availability>(source.Availability, true, out var availability))
                    result.Availability = availability;

                return (TDestination)(object)result;
            }

            throw MappingNotSupported<PutQuestionnaireRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this PostQuestionnaireRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.UpdateQuestionnaireRequest))
            {
                var result = new Api.Models.RequestModels.UpdateQuestionnaireRequest
                {
                    Id = source.Id,
                    Code = source.Code,
                    Name = source.Name
                };

                if (Enum.TryParse<Availability>(source.Availability, true, out var availability))
                    result.Availability = availability;

                if (source.LabelsIds != null)
                    result.Labels = source.LabelsIds.Select(l => new Api.Models.LabelInQuestionnaire { Id = l }).ToList();

                return (TDestination)(object)result;
            }

            throw MappingNotSupported<PostQuestionnaireRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this PostQuestionsRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.UpdateQuestionsRequest))
                return (TDestination)(object)new Api.Models.RequestModels.UpdateQuestionsRequest
                {
                    QuestionnaireId = source.QuestionnaireId,
                    QuestionnaireCode = source.QuestionnaireCode,
                    CreatedQuestions = source.CreatedQuestions?.Select(q => q.MapTo<Api.Models.QuestionToUpdate>()).ToList(),
                    UpdatedQuestions = source.UpdatedQuestions?.Select(q => q.MapTo<Api.Models.QuestionToUpdate>()).ToList(),
                    DeletedQuestions = source.DeletedQuestions?.Select(q => q.MapTo<Api.Models.QuestionIdentifier>()).ToList()
                };

            throw MappingNotSupported<PostQuestionsRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this PostMyStatusRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.UpdateUserQuestionStatusesRequest))
                return (TDestination)(object)new Api.Models.RequestModels.UpdateUserQuestionStatusesRequest
                {
                    Items = source.Items?.Select(i => i.MapTo<Api.Models.UserQuestionStatus>()).ToList()
                };

            throw MappingNotSupported<PostMyStatusRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this GetLabelsRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.GetLabelsRequest))
                return (TDestination)(object)new Api.Models.RequestModels.GetLabelsRequest
                {
                    PageNumber = source.PageNumber,
                    PageSize = source.PageSize,
                    Origin = string.IsNullOrEmpty(source.Origin) ? null : Enum.Parse<Origin>(source.Origin, true),
                    OwnerId = source.OwnerId,
                    PartOfName = source.PartOfName
                };

            throw MappingNotSupported<GetLabelsRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this PostTrainingRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.UpdateTrainingRequest))
                return (TDestination)(object)new Api.Models.RequestModels.UpdateTrainingRequest
                {
                    Id = source.Id,
                    Name = source.Name,
                    RefreshLastTime = source.RefreshLastTime ?? false,
                    LengthType = string.IsNullOrEmpty(source.LengthType) ? null : Enum.Parse<TrainingLengthType>(source.LengthType, true),
                    QuestionsCount = source.QuestionsCount,
                    TimeMinutes = source.TimeMinutes,
                    NewQuestionsFraction = source.NewQuestionsFraction,
                    PenaltyQuestionsFraction = source.PenaltyQuestionsFraction,
                    QuestionnairesIds = source.QuestionnairesIds
                };

            throw MappingNotSupported<PostTrainingRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this CollectionRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.GetCollectionRequest))
                return (TDestination)(object)new Api.Models.RequestModels.GetCollectionRequest
                {
                    PageNumber = source.PageNumber,
                    PageSize = source.PageSize
                };

            throw MappingNotSupported<CollectionRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Web.Models.Requests.Training.TrainingResultRequest? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.TrainingResult))
                return (TDestination)(object)new Api.Models.TrainingResult
                {
                    QuestionId = source.QuestionId,
                    IsAnswerCorrect = source.IsAnswerCorrect,
                    AnswerTimeMilliseconds = source.AnswerTimeMilliseconds,
                    GivenTypedAnswers = source.GivenTypedAnswers?.Select(a => a.MapTo<Api.Models.GivenTypedAnswer>()).ToList()
                };

            throw MappingNotSupported<Web.Models.Requests.Training.TrainingResultRequest, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Web.Models.ApiEntities.GivenTypedAnswer? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.GivenTypedAnswer))
                return (TDestination)(object)new Api.Models.GivenTypedAnswer
                {
                    Text = source.Text,
                    IsCorrect = source.IsCorrect
                };

            throw MappingNotSupported<Web.Models.ApiEntities.GivenTypedAnswer, TDestination>();
        }

        private static NotSupportedException MappingNotSupported<TSource, TDestination>() =>
            new($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported.");
    }
}
