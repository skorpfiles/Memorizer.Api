﻿using AutoMapper;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Mapping
{
    public class DataAccessMappingProfile:Profile
    {
        public DataAccessMappingProfile()
        {
            CreateMap<Questionnaire, SkorpFiles.Memorizer.Api.Models.Questionnaire>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.QuestionnaireId))
                .ForMember(dest => dest.Code, opts => opts.MapFrom(src => src.QuestionnaireCode))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.QuestionnaireName))
                .ForMember(dest => dest.Availability, opts => opts.MapFrom(src => src.QuestionnaireAvailability))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
            CreateMap<Label, SkorpFiles.Memorizer.Api.Models.Label>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.LabelId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.LabelName))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
        }
    }
}