using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;
using System.Diagnostics.CodeAnalysis;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public static class ResponsesMapper
    {
        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.PaginatedCollection<Api.Models.Questionnaire>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(GetQuestionnairesResponse))
                return (TDestination)(object)new GetQuestionnairesResponse
                {
                    Questionnaires = source.Items.Select(q => q.MapTo<Questionnaire>()).ToList(),
                    TotalCount = source.TotalCount,
                    TotalPages = source.TotalPages
                };

            throw MappingNotSupported<Api.Models.PaginatedCollection<Api.Models.Questionnaire>, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(GetQuestionsResponse))
                return (TDestination)(object)new GetQuestionsResponse
                {
                    Questions = source.Items.Select(q => q.MapTo<ExistingQuestion>()).ToList(),
                    TotalCount = source.TotalCount,
                    TotalPages = source.TotalPages
                };

            throw MappingNotSupported<Api.Models.PaginatedCollection<Api.Models.ExistingQuestion>, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.PaginatedCollection<Api.Models.Label>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(GetLabelsResponse))
                return (TDestination)(object)new GetLabelsResponse
                {
                    Labels = source.Items.Select(l => l.MapTo<Label>()).ToList(),
                    TotalCount = source.TotalCount,
                    TotalPages = source.TotalPages
                };

            throw MappingNotSupported<Api.Models.PaginatedCollection<Api.Models.Label>, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.PaginatedCollection<Api.Models.Training>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(GetTrainingsResponse))
                return (TDestination)(object)new GetTrainingsResponse
                {
                    Trainings = source.Items.Select(t => t.MapTo<Training>()).ToList(),
                    TotalCount = source.TotalCount,
                    TotalPages = source.TotalPages
                };

            throw MappingNotSupported<Api.Models.PaginatedCollection<Api.Models.Training>, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this IEnumerable<Api.Models.ExistingQuestion>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(StartTrainingResponse))
                return (TDestination)(object)new StartTrainingResponse
                {
                    Questions = source.Select(q => q.MapTo<QuestionForTraining>()).ToList()
                };

            throw MappingNotSupported<IEnumerable<Api.Models.ExistingQuestion>, TDestination>();
        }

        private static NotSupportedException MappingNotSupported<TSource, TDestination>() =>
            new($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported.");
    }
}
