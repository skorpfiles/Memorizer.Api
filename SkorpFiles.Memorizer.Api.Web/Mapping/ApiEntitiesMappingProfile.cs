using AutoMapper;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Web.Extensions;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public class ApiEntitiesMappingProfile:Profile
    {
        public ApiEntitiesMappingProfile()
        {
            CreateMap<SkorpFiles.Memorizer.Api.Models.Questionnaire, Questionnaire>()
                .ForMember(dest => dest.Availability, opts => opts.MapFrom(src => src.Availability.ToString().PascalCaseToLowerCamelCase()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Label, Label>()
                .ForMember(dest => dest.Number, opts => opts.MapFrom(src => src.StatusInQuestionnaire!.Number))
                .ForMember(dest => dest.ParentLabelId, opts => opts.MapFrom(src => src.StatusInQuestionnaire!.ParentLabelId));
            CreateMap<Label, SkorpFiles.Memorizer.Api.Models.Label>()
                .ForMember(dest => dest.StatusInQuestionnaire, opts => opts.MapFrom(src => src));
            CreateMap<Label, SkorpFiles.Memorizer.Api.Models.LabelInQuestionnaire>();

            CreateMap<SkorpFiles.Memorizer.Api.Models.Question, Question>()
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString().PascalCaseToLowerCamelCase()))
                .ForMember(dest => dest.Enabled, opts => opts.MapFrom(src => src.IsEnabled))
                .IncludeAllDerived();
            CreateMap<SkorpFiles.Memorizer.Api.Models.Question, ExistingQuestion>()
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString().PascalCaseToLowerCamelCase()))
                .ForMember(dest => dest.Enabled, opts => opts.MapFrom(src => src.IsEnabled));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.Question>()
                .ForMember(dest => dest.Type, opts =>
                {
                    opts.Condition(src => Enum.TryParse<QuestionType>(src.Type, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionType>(src.Type!));
                })
                .ForMember(dest => dest.IsEnabled, opts => opts.MapFrom(src => src.Enabled));

            CreateMap<SkorpFiles.Memorizer.Api.Models.UserQuestionStatus, UserQuestionStatus>();
            CreateMap<SkorpFiles.Memorizer.Api.Models.TypedAnswer, TypedAnswer>();
            CreateMap<TypedAnswer, SkorpFiles.Memorizer.Api.Models.TypedAnswer>();
            CreateMap<SkorpFiles.Memorizer.Api.Models.QuestionIdentifier, QuestionIdentifier>();
            CreateMap<QuestionIdentifier, SkorpFiles.Memorizer.Api.Models.QuestionIdentifier>();
            CreateMap<SkorpFiles.Memorizer.Api.Models.QuestionToUpdate, QuestionToUpdate>()
                .ForMember(dest => dest.Enabled, opts => opts.MapFrom(src => src.IsEnabled));
            CreateMap<QuestionToUpdate, SkorpFiles.Memorizer.Api.Models.QuestionToUpdate>()
                .ForMember(dest=>dest.IsEnabled, opts=>opts.MapFrom(src=>src.Enabled));
            CreateMap<SkorpFiles.Memorizer.Api.Models.ExistingQuestion, ExistingQuestion>();
            CreateMap<ExistingQuestion, SkorpFiles.Memorizer.Api.Models.ExistingQuestion>();
            CreateMap<UserQuestionStatus, SkorpFiles.Memorizer.Api.Models.UserQuestionStatus>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(src => src.Id));
        }
    }
}
