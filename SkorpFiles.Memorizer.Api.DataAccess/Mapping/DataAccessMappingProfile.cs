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
                    Id = src.LabelId,
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
            CreateMap<SkorpFiles.Memorizer.Api.Models.Label, Label>()
                .ForMember(dest => dest.LabelId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.LabelName, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.ObjectCreationTimeUtc, opts => opts.MapFrom(src => src.CreationTimeUtc))
                .ForMember(dest => dest.ObjectIsRemoved, opts => opts.MapFrom(src => src.IsRemoved))
                .ForMember(dest => dest.ObjectRemovalTimeUtc, opts => opts.MapFrom(src => src.RemovalTimeUtc));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.Question>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.CodeInQuestionnaire, opts => opts.MapFrom(src => src.QuestionQuestionnaireCode))
                .ForMember(dest => dest.Reference, opts => opts.MapFrom(src => src.QuestionReference))
                .ForMember(dest => dest.IsFixed, opts => opts.MapFrom(src => src.QuestionIsFixed))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.QuestionType));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.ExistingQuestion>()
                .ForMember(dest=>dest.Labels, opts=>opts.MapFrom(src=>src.LabelsForQuestion));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.QuestionToUpdate>()
                .ForMember(dest => dest.LabelsIds, opts =>
                {
                    opts.Condition(src => src.LabelsForQuestion != null);
                    opts.MapFrom(src => src.LabelsForQuestion!.Select(l => l.LabelId));
                });
            CreateMap<SkorpFiles.Memorizer.Api.Models.Question, Question>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionText, opts =>
                {
                    opts.Condition(src => src.Text != null);
                    opts.MapFrom(src => src.Text);
                })
                .ForMember(dest => dest.QuestionUntypedAnswer, opts => opts.MapFrom(src => src.UntypedAnswer))
                .ForMember(dest => dest.QuestionEstimatedTrainingTimeSeconds, opts => opts.MapFrom(src => src.EstimatedTrainingTimeSeconds))
                .ForMember(dest => dest.QuestionIsEnabled, opts => opts.MapFrom(src => src.IsEnabled))
                .ForMember(dest => dest.QuestionReference, opts => opts.MapFrom(src => src.Reference))
                .ForMember(dest => dest.QuestionType, opts => opts.MapFrom(src => src.Type))
                .ForMember(dest => dest.QuestionIsFixed, opts => opts.MapFrom(src => src.IsFixed));
            CreateMap<QuestionUser, SkorpFiles.Memorizer.Api.Models.UserQuestionStatus>()
                .ForMember(dest => dest.IsNew, opts => opts.MapFrom(src => src.QuestionUserIsNew))
                .ForMember(dest => dest.Rating, opts => opts.MapFrom(src => src.QuestionUserRating))
                .ForMember(dest => dest.PenaltyPoints, opts => opts.MapFrom(src => src.QuestionUserPenaltyPoints));
            CreateMap<SkorpFiles.Memorizer.Api.Models.UserQuestionStatus, QuestionUser>()
                .ForMember(dest => dest.QuestionUserIsNew, opts => opts.MapFrom(src => src.IsNew))
                .ForMember(dest => dest.QuestionUserRating, opts => opts.MapFrom(src => src.Rating))
                .ForMember(dest => dest.QuestionUserPenaltyPoints, opts => opts.MapFrom(src => src.PenaltyPoints));
            CreateMap<TypedAnswer, SkorpFiles.Memorizer.Api.Models.TypedAnswer>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.TypedAnswerId))
                .ForMember(dest => dest.Text, opts=>opts.MapFrom(src=>src.TypedAnswerText))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
        }
    }
}
