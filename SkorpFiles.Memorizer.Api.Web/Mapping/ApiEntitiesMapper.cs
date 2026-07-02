using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Web.Extensions;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using System.Diagnostics.CodeAnalysis;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public static class ApiEntitiesMapper
    {
        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.Questionnaire? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Questionnaire))
                return (TDestination)(object)new Questionnaire
                {
                    Id = source.Id ?? default,
                    Code = source.Code ?? default,
                    Name = source.Name,
                    Availability = source.Availability.ToString().PascalCaseToLowerCamelCase(),
                    OwnerId = source.OwnerId,
                    OwnerName = source.OwnerName,
                    CountsOfQuestions = source.CountsOfQuestions?.MapTo<QuestionsCounts>(),
                    TotalTrainingTimeSeconds = source.TotalTrainingTimeSeconds ?? default,
                    Labels = source.Labels?.Select(l => l.MapTo<Label>()).ToList()
                };

            if (typeof(TDestination) == typeof(QuestionnaireForTraining))
                return (TDestination)(object)new QuestionnaireForTraining
                {
                    Id = source.Id ?? default,
                    Code = source.Code ?? default,
                    Name = source.Name,
                    OwnerId = source.OwnerId,
                    OwnerName = source.OwnerName
                };

            throw MappingNotSupported<Api.Models.Questionnaire, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.QuestionsCounts? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(QuestionsCounts))
                return (TDestination)(object)new QuestionsCounts
                {
                    Total = source.Total,
                    New = source.New,
                    Rechecked = source.Rechecked
                };

            throw MappingNotSupported<Api.Models.QuestionsCounts, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.Label? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Label))
                return (TDestination)(object)new Label
                {
                    Id = source.Id ?? default,
                    Name = source.Name,
                    Number = source.StatusInQuestionnaire?.Number ?? default,
                    ParentLabelId = source.StatusInQuestionnaire?.ParentLabelId
                };

            throw MappingNotSupported<Api.Models.Label, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Label? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.LabelInQuestionnaire))
                return (TDestination)(object)new Api.Models.LabelInQuestionnaire
                {
                    Id = source.Id,
                    Number = source.Number,
                    ParentLabelId = source.ParentLabelId
                };

            throw MappingNotSupported<Label, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.ExistingQuestion? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(ExistingQuestion))
                return (TDestination)(object)new ExistingQuestion
                {
                    Id = source.Id,
                    CodeInQuestionnaire = source.CodeInQuestionnaire,
                    Type = source.Type.ToString().PascalCaseToLowerCamelCase(),
                    Text = source.Text,
                    UntypedAnswer = source.UntypedAnswer,
                    Enabled = source.IsEnabled,
                    Reference = source.Reference,
                    IsFixed = source.IsFixed,
                    MyStatus = source.MyStatus?.MapTo<UserQuestionStatus>(),
                    EstimatedTrainingTimeSeconds = source.EstimatedTrainingTimeSeconds,
                    Labels = source.Labels?.Select(l => l.MapTo<Label>()).ToList(),
                    TypedAnswers = source.TypedAnswers?.Select(a => a.MapTo<TypedAnswer>()).ToList()
                };

            if (typeof(TDestination) == typeof(QuestionForTraining))
                return (TDestination)(object)new QuestionForTraining
                {
                    Id = source.Id,
                    CodeInQuestionnaire = source.CodeInQuestionnaire,
                    Type = source.Type.ToString().PascalCaseToLowerCamelCase(),
                    Text = source.Text,
                    UntypedAnswer = source.UntypedAnswer,
                    Enabled = source.IsEnabled,
                    Reference = source.Reference,
                    IsFixed = source.IsFixed,
                    MyStatus = source.MyStatus?.MapTo<UserQuestionStatus>(),
                    EstimatedTrainingTimeSeconds = source.EstimatedTrainingTimeSeconds,
                    Labels = source.Labels?.Select(l => l.MapTo<Label>()).ToList(),
                    TypedAnswers = source.TypedAnswers?.Select(a => a.MapTo<TypedAnswer>()).ToList(),
                    Questionnaire = source.Questionnaire?.MapTo<QuestionnaireForTraining>()
                };

            throw MappingNotSupported<Api.Models.ExistingQuestion, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.TypedAnswer? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(TypedAnswer))
                return (TDestination)(object)new TypedAnswer
                {
                    Id = source.Id ?? default,
                    Text = source.Text
                };

            throw MappingNotSupported<Api.Models.TypedAnswer, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.UserQuestionStatus? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(UserQuestionStatus))
                return (TDestination)(object)new UserQuestionStatus
                {
                    IsNew = source.IsNew,
                    Rating = source.Rating,
                    PenaltyPoints = source.PenaltyPoints
                };

            throw MappingNotSupported<Api.Models.UserQuestionStatus, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this UserQuestionStatus? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.UserQuestionStatus))
                return (TDestination)(object)new Api.Models.UserQuestionStatus
                {
                    IsNew = source.IsNew,
                    Rating = source.Rating,
                    PenaltyPoints = source.PenaltyPoints
                };

            throw MappingNotSupported<UserQuestionStatus, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this QuestionToUpdate? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.QuestionToUpdate))
                return (TDestination)(object)new Api.Models.QuestionToUpdate
                {
                    Id = source.Id,
                    CodeInQuestionnaire = source.CodeInQuestionnaire,
                    Type = string.IsNullOrEmpty(source.Type) ? default : Enum.Parse<QuestionType>(source.Type, true),
                    Text = source.Text,
                    UntypedAnswer = source.UntypedAnswer,
                    IsEnabled = source.Enabled,
                    Reference = source.Reference,
                    IsFixed = source.IsFixed,
                    EstimatedTrainingTimeSeconds = source.EstimatedTrainingTimeSeconds,
                    MyStatus = source.MyStatus?.MapTo<Api.Models.UserQuestionStatus>(),
                    LabelsIds = source.LabelsIds,
                    TypedAnswers = source.TypedAnswers
                };

            throw MappingNotSupported<QuestionToUpdate, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this QuestionIdentifier? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Api.Models.QuestionIdentifier))
                return (TDestination)(object)new Api.Models.QuestionIdentifier
                {
                    Id = source.Id,
                    Code = source.Code
                };

            throw MappingNotSupported<QuestionIdentifier, TDestination>();
        }

        [return: NotNullIfNotNull(nameof(source))]
        public static TDestination? MapTo<TDestination>(this Api.Models.Training? source) where TDestination : class
        {
            if (source == null)
                return null;

            if (typeof(TDestination) == typeof(Training))
                return (TDestination)(object)new Training
                {
                    Id = source.Id ?? default,
                    Name = source.Name,
                    LastTime = source.LastTimeUtc,
                    LengthType = source.LengthType.ToString().PascalCaseToLowerCamelCase(),
                    QuestionsCount = source.QuestionsCount,
                    TimeMinutes = source.TimeMinutes,
                    NewQuestionsFraction = source.NewQuestionsFraction,
                    PenaltyQuestionsFraction = source.PenaltyQuestionsFraction,
                    Questionnaires = source.Questionnaires?.Select(q => q.MapTo<Questionnaire>()).ToList()
                };

            if (typeof(TDestination) == typeof(Api.Models.RequestModels.TrainingOptions))
            {
                var trainingOptions = new Api.Models.RequestModels.TrainingOptions
                {
                    LengthType = source.LengthType,
                    NewQuestionsFraction = (double)source.NewQuestionsFraction,
                    PrioritizedPenaltyQuestionsFraction = (double)source.PenaltyQuestionsFraction
                };

                if (source.LengthType == TrainingLengthType.QuestionsCount || source.LengthType == TrainingLengthType.Time)
                    trainingOptions.LengthValue = source.LengthType == TrainingLengthType.QuestionsCount ? source.QuestionsCount : source.TimeMinutes * Constants.SecondsInMinute;

                return (TDestination)(object)trainingOptions;
            }

            throw MappingNotSupported<Api.Models.Training, TDestination>();
        }

        private static NotSupportedException MappingNotSupported<TSource, TDestination>() =>
            new($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported.");
    }
}
