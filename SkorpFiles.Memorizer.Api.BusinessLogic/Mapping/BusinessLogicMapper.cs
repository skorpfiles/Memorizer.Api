using SkorpFiles.Memorizer.Api.DataAccess.Mapping;
using SkorpFiles.Memorizer.Api.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Mapping
{
    public static class BusinessLogicMapper
    {
        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this GetQuestionsForTrainingResult? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(ExistingQuestion))
                return (TDestination)(object)new ExistingQuestion
                {
                    Id = source.Id,
                    Code = source.Code,
                    Type = source.QuestionType,
                    Text = source.QuestionText,
                    UntypedAnswer = source.QuestionUntypedAnswer,
                    IsEnabled = source.QuestionIsEnabled,
                    Reference = source.QuestionReference,
                    EstimatedTrainingTimeSeconds = source.QuestionEstimatedTrainingTimeSeconds,
                    QuestionnaireId = source.QuestionnaireId ?? default,
                    TypedAnswers = source.TypedAnswersJson == null ? null :
                        JsonSerializer.Deserialize<List<DataAccess.Models.TypedAnswer>>(source.TypedAnswersJson)?
                            .Select(a => a.MapTo<TypedAnswer>()).ToList(),
                    Questionnaire = new Questionnaire
                    {
                        Id = source.QuestionnaireId,
                        Name = source.QuestionnaireName
                    },
                    MyStatus = source.QuestionUserIsNew == null || source.QuestionUserRating == null ||
                        source.QuestionUserPenaltyPoints == null ? null :
                        new UserQuestionStatus
                        {
                            IsNew = source.QuestionUserIsNew.Value,
                            Rating = source.QuestionUserRating.Value,
                            PenaltyPoints = source.QuestionUserPenaltyPoints.Value,
                            AverageTrainingTimeSeconds = source.QuestionActualTrainingTimeSeconds
                        },
                    CreationTimeUtc = source.CreationTimeUtc,
                    IsRemoved = source.IsRemoved,
                    RemovalTimeUtc = source.RemovalTimeUtc
                };

            throw new NotSupportedException($"Mapping from {nameof(GetQuestionsForTrainingResult)} to {typeof(TDestination).Name} is not supported.");
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this IEnumerable<GetQuestionsForTrainingResult>? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(List<ExistingQuestion>))
                return (TDestination)(object)source.Select(q => q.MapTo<ExistingQuestion>()).ToList();

            throw new NotSupportedException($"Mapping from {nameof(IEnumerable<GetQuestionsForTrainingResult>)} to {typeof(TDestination).Name} is not supported.");
        }
    }
}
