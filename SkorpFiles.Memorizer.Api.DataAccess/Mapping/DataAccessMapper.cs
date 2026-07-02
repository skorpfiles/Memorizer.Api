using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System.Diagnostics.CodeAnalysis;

namespace SkorpFiles.Memorizer.Api.DataAccess.Mapping
{
    public static class DataAccessMapper
    {
        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Questionnaire? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.Questionnaire))
                return (TDestination)(object)new Api.Models.Questionnaire
                {
                    Id = source.QuestionnaireId,
                    Code = source.QuestionnaireCode,
                    Name = source.QuestionnaireName,
                    Availability = source.QuestionnaireAvailability,
                    OwnerId = Guid.Parse(source.OwnerId),
                    OwnerName = source.Owner?.UserName,
                    Labels = source.LabelsForQuestionnaire?.Select(el => el.MapTo<Api.Models.Label>()).ToList(),
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.ObjectIsRemoved,
                    RemovalTimeUtc = source.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<Questionnaire, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this EntityLabel? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.Label))
                return (TDestination)(object)new Api.Models.Label
                {
                    Id = source.LabelId,
                    Name = source.Label?.LabelName,
                    StatusInQuestionnaire = new Api.Models.LabelInQuestionnaire
                    {
                        Id = source.LabelId,
                        Number = source.LabelNumber,
                        ParentLabelId = source.ParentLabelId
                    },
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.Label?.ObjectIsRemoved ?? false,
                    RemovalTimeUtc = source.Label?.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<EntityLabel, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Label? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.Label))
                return (TDestination)(object)new Api.Models.Label
                {
                    Id = source.LabelId,
                    Code = source.LabelCode,
                    Name = source.LabelName,
                    OwnerId = Guid.Parse(source.OwnerId),
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.ObjectIsRemoved,
                    RemovalTimeUtc = source.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<Label, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Question? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.ExistingQuestion))
                return (TDestination)(object)new Api.Models.ExistingQuestion
                {
                    Id = source.QuestionId,
                    CodeInQuestionnaire = source.QuestionQuestionnaireCode,
                    Type = source.QuestionType,
                    Text = source.QuestionText,
                    UntypedAnswer = source.QuestionUntypedAnswer,
                    IsEnabled = source.QuestionIsEnabled,
                    Reference = source.QuestionReference,
                    IsFixed = source.QuestionIsFixed,
                    EstimatedTrainingTimeSeconds = source.QuestionEstimatedTrainingTimeSeconds,
                    QuestionnaireId = source.QuestionnaireId,
                    Questionnaire = source.Questionnaire?.MapTo<Api.Models.Questionnaire>(),
                    Labels = source.LabelsForQuestion?.Select(el => el.MapTo<Api.Models.Label>()).ToList(),
                    TypedAnswers = source.TypedAnswers?.Select(a => a.MapTo<Api.Models.TypedAnswer>()).ToList(),
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.ObjectIsRemoved,
                    RemovalTimeUtc = source.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<Question, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this TypedAnswer? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.TypedAnswer))
                return (TDestination)(object)new Api.Models.TypedAnswer
                {
                    Id = source.TypedAnswerId,
                    QuestionId = source.QuestionId ?? default,
                    Text = source.TypedAnswerText,
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.ObjectIsRemoved,
                    RemovalTimeUtc = source.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<TypedAnswer, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this QuestionUser? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.UserQuestionStatus))
                return (TDestination)(object)new Api.Models.UserQuestionStatus
                {
                    QuestionId = source.QuestionId,
                    UserId = Guid.Parse(source.UserId),
                    IsNew = source.QuestionUserIsNew,
                    Rating = source.QuestionUserRating,
                    PenaltyPoints = source.QuestionUserPenaltyPoints
                };

            throw MappingNotSupported<QuestionUser, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.UserQuestionStatus? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(QuestionUser))
                return (TDestination)(object)new QuestionUser
                {
                    QuestionId = source.QuestionId,
                    UserId = source.UserId.ToString(),
                    QuestionUserIsNew = source.IsNew,
                    QuestionUserRating = source.Rating,
                    QuestionUserPenaltyPoints = source.PenaltyPoints
                };

            throw MappingNotSupported<Api.Models.UserQuestionStatus, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.Question? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Question))
                return (TDestination)(object)new Question
                {
                    QuestionId = source.Id ?? default,
                    QuestionType = source.Type,
                    QuestionText = source.Text!,
                    QuestionUntypedAnswer = source.UntypedAnswer,
                    QuestionIsEnabled = source.IsEnabled,
                    QuestionReference = source.Reference,
                    QuestionIsFixed = source.IsFixed,
                    QuestionEstimatedTrainingTimeSeconds = source.EstimatedTrainingTimeSeconds,
                    QuestionnaireId = source.QuestionnaireId
                };

            throw MappingNotSupported<Api.Models.Question, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Training? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.Training))
                return (TDestination)(object)new Api.Models.Training
                {
                    Id = source.TrainingId,
                    Name = source.TrainingName,
                    LastTimeUtc = source.TrainingLastTimeUtc,
                    LengthType = source.TrainingLengthType,
                    QuestionsCount = source.TrainingQuestionsCount,
                    TimeMinutes = source.TrainingTimeMinutes,
                    NewQuestionsFraction = source.TrainingNewQuestionsFraction,
                    PenaltyQuestionsFraction = source.TrainingPenaltyQuestionsFraction,
                    Questionnaires = source.QuestionnairesForTraining?.Select(tq => tq.Questionnaire?.MapTo<Api.Models.Questionnaire>()!).ToList(),
                    CreationTimeUtc = source.ObjectCreationTimeUtc,
                    IsRemoved = source.ObjectIsRemoved,
                    RemovalTimeUtc = source.ObjectRemovalTimeUtc
                };

            throw MappingNotSupported<Training, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.TrainingResult? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(TrainingResult))
                return (TDestination)(object)new TrainingResult
                {
                    TrainingResultQuestionId = source.QuestionId,
                    TrainingResultUserId = source.UserId.ToString(),
                    TrainingResultRecordingTime = source.RecordingTime,
                    TrainingResultAnswerIsCorrect = source.IsAnswerCorrect,
                    TrainingResultTimeSeconds = (int)Math.Round((double)(source.AnswerTimeMilliseconds / 1000)),
                    TrainingResultIsNew = source.ResultQuestionStatus?.IsNew ?? default,
                    TrainingResultRating = source.ResultQuestionStatus?.Rating ?? default,
                    TrainingResultPenaltyPoints = source.ResultQuestionStatus?.PenaltyPoints ?? default,
                    TrainingResultInitialNewStatus = source.InitialQuestionStatus?.IsNew ?? default,
                    TrainingResultInitialRating = source.InitialQuestionStatus?.Rating ?? default,
                    TrainingResultInitialPenaltyPoints = source.InitialQuestionStatus?.PenaltyPoints ?? default,
                    TypedAnswers = source.GivenTypedAnswers?.Select(a => a.MapTo<TrainingResultTypedAnswer>()).ToList()
                };

            throw MappingNotSupported<Api.Models.TrainingResult, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.GivenTypedAnswer? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(TrainingResultTypedAnswer))
                return (TDestination)(object)new TrainingResultTypedAnswer
                {
                    TrtaAnswer = source.Text!,
                    TrtaIsCorrect = source.IsCorrect
                };

            throw MappingNotSupported<Api.Models.GivenTypedAnswer, TDestination>();
        }

        private static NotSupportedException MappingNotSupported<TSource, TDestination>() =>
            new($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported.");
    }
}
