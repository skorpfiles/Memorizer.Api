using AutoMapper;
using SkorpFiles.Memorizer.Api.Models;
using System.Text.Json;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Mapping
{
    public class BusinessLogicMappingProfile: Profile
    {
        public BusinessLogicMappingProfile()
        {
            CreateMap<GetQuestionsForTrainingResult, ExistingQuestion>()
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.UntypedAnswer, opts => opts.MapFrom(src => src.QuestionUntypedAnswer))
                .ForMember(dest => dest.IsEnabled, opts => opts.MapFrom(src => src.QuestionIsEnabled))
                .ForMember(dest => dest.Reference, opts => opts.MapFrom(src => src.QuestionReference))
                .ForMember(dest => dest.EstimatedTrainingTimeSeconds, opts => opts.MapFrom(src => src.QuestionEstimatedTrainingTimeSeconds))
                .ForMember(dest => dest.TypedAnswers, opts => opts.MapFrom((src, dest, destMember, context) =>
                {
                    if (src?.TypedAnswersJson == null)
                        return null;
                    return context.Mapper.Map<List<TypedAnswer>>(JsonSerializer.Deserialize<List<TypedAnswer>>(src.TypedAnswersJson));
                }))
                .ForMember(dest => dest.Questionnaire, opts => opts.MapFrom(src => new Questionnaire
                {
                    Id = src.QuestionnaireId,
                    Name = src.QuestionnaireName
                }))
                .ForMember(dest => dest.MyStatus, opts => opts.MapFrom(src =>
                    src.QuestionUserIsNew == null || src.QuestionUserRating == null ||
                    src.QuestionUserPenaltyPoints == null ? null :
                    new UserQuestionStatus
                    {
                        IsNew = src.QuestionUserIsNew.Value,
                        Rating = src.QuestionUserRating.Value,
                        PenaltyPoints = src.QuestionUserPenaltyPoints.Value,
                        AverageTrainingTimeSeconds = src.QuestionActualTrainingTimeSeconds
                    })
                );
        }
    }
}
