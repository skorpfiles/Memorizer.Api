﻿using AutoMapper;
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
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.Type.ToString().PascalCaseToLowerCamelCase()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.UserQuestionStatus, UserQuestionStatus>();
            CreateMap<SkorpFiles.Memorizer.Api.Models.TypedAnswer, TypedAnswer>();
        }
    }
}