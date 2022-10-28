using AutoMapper;
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
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc))
                .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.LabelsForQuestionnaire));
            CreateMap<EntityLabel, SkorpFiles.Memorizer.Api.Models.Label>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.LabelId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Label!.LabelName))
                .ForMember(dest => dest.StatusInQuestionnaire, opts => opts.MapFrom(src => new SkorpFiles.Memorizer.Api.Models.LabelInQuestionnaire
                {
                     Number = src.LabelNumber,
                     ParentLabelId = src.ParentLabelId
                }))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.Label!.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.Label!.ObjectRemovalTimeUtc));
            CreateMap<Label, SkorpFiles.Memorizer.Api.Models.Label>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.LabelId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.LabelName))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.Question>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.CodeInQuestionnaire, opts => opts.MapFrom(src => src.QuestionQuestionnaireCode))
                .ForMember(dest => dest.Reference, opts => opts.MapFrom(src => src.QuestionReference))
                .ForMember(dest => dest.IsFixed, opts => opts.MapFrom(src => src.QuestionIsFixed))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc))
                .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.LabelsForQuestion));
            CreateMap<QuestionUser, SkorpFiles.Memorizer.Api.Models.UserQuestionStatus>()
                .ForMember(dest => dest.IsNew, opts => opts.MapFrom(src => src.QuestionUserIsNew))
                .ForMember(dest => dest.Rating, opts => opts.MapFrom(src => src.QuestionUserRating))
                .ForMember(dest => dest.PenaltyPoints, opts => opts.MapFrom(src => src.QuestionUserPenaltyPoints));
            CreateMap<TypedAnswer, SkorpFiles.Memorizer.Api.Models.TypedAnswer>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.TypedAnswerId))
                .ForMember(dest => dest.Text, opts=>opts.MapFrom(src=>src.TypedAnswerText))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
        }
    }
}
