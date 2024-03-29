﻿using AutoMapper;
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
            CreateMap<UserQuestionStatus, SkorpFiles.Memorizer.Api.Models.UserQuestionStatus>();
            CreateMap<UserQuestionStatusForUpdate, SkorpFiles.Memorizer.Api.Models.UserQuestionStatus>();
            CreateMap<Training, SkorpFiles.Memorizer.Api.Models.Training>()
                .ForMember(dest => dest.LengthType, opts =>
                {
                    opts.Condition(src => Enum.TryParse<TrainingLengthType>(src.LengthType, out _));
                    opts.MapFrom(src => Enum.Parse<TrainingLengthType>(src.LengthType!));
                })
                .ForMember(dest => dest.LastTimeUtc, opts => opts.MapFrom(src => src.LastTime));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Training, Training>()
                .ForMember(dest => dest.LengthType, opts => opts.MapFrom(src => src.LengthType.ToString().PascalCaseToLowerCamelCase()))
                .ForMember(dest => dest.LastTime, opts => opts.MapFrom(src => src.LastTimeUtc));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Questionnaire, QuestionnaireForTraining>();
            CreateMap<SkorpFiles.Memorizer.Api.Models.QuestionsCounts, QuestionsCounts>();
            CreateMap<Api.Models.ExistingQuestion, QuestionForTraining>();
        }
    }
}
