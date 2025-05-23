﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.Design;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Mapping
{
    public class DataAccessMappingProfile : Profile
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
                .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.LabelsForQuestionnaire))
                .ForMember(dest => dest.OwnerName, opts =>
                {
                    opts.Condition(src => src.Owner != null);
                    opts.MapFrom(src => src.Owner!.UserName);
                });
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
                .ForMember(dest => dest.Code, opts => opts.MapFrom(src => src.LabelCode))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Label, Label>()
                .ForMember(dest => dest.LabelId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.LabelName, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.LabelCode, opts => opts.MapFrom(src => src.Code))
                .ForMember(dest => dest.ObjectCreationTimeUtc, opts => opts.MapFrom(src => src.CreationTimeUtc))
                .ForMember(dest => dest.ObjectIsRemoved, opts => opts.MapFrom(src => src.IsRemoved))
                .ForMember(dest => dest.ObjectRemovalTimeUtc, opts => opts.MapFrom(src => src.RemovalTimeUtc));
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.Question>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.CodeInQuestionnaire, opts => opts.MapFrom(src => src.QuestionQuestionnaireCode))
                .ForMember(dest => dest.Reference, opts => opts.MapFrom(src => src.QuestionReference))
                .ForMember(dest => dest.IsEnabled, opts => opts.MapFrom(src => src.QuestionIsEnabled))
                .ForMember(dest => dest.IsFixed, opts => opts.MapFrom(src => src.QuestionIsFixed))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc))
                .ForMember(dest => dest.Type, opts => opts.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.Questionnaire, opts => opts.MapFrom(src => src.Questionnaire))
                .ForMember(dest => dest.EstimatedTrainingTimeSeconds, opts => opts.MapFrom(src => src.QuestionEstimatedTrainingTimeSeconds))
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.UntypedAnswer, opts => opts.MapFrom(src => src.QuestionUntypedAnswer))
                .IncludeAllDerived();
            CreateMap<Question, SkorpFiles.Memorizer.Api.Models.ExistingQuestion>()
                .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.LabelsForQuestion));
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
                .ForMember(dest => dest.QuestionIsFixed, opts => opts.MapFrom(src => src.IsFixed))
                .IncludeAllDerived();
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
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.TypedAnswerText))
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Training, Training>()
                .ForMember(dest => dest.TrainingId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.TrainingName, opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.TrainingLastTimeUtc, opts => opts.MapFrom(src => src.LastTimeUtc))
                .ForMember(dest => dest.TrainingLengthType, opts => opts.MapFrom(src => src.LengthType))
                .ForMember(dest => dest.TrainingQuestionsCount, opts => opts.MapFrom(src => src.QuestionsCount))
                .ForMember(dest => dest.TrainingTimeMinutes, opts => opts.MapFrom(src => src.TimeMinutes))
                .ForMember(dest => dest.TrainingNewQuestionsFraction, opts => opts.MapFrom(src => src.NewQuestionsFraction))
                .ForMember(dest => dest.TrainingPenaltyQuestionsFraction, opts => opts.MapFrom(src => src.PenaltyQuestionsFraction))
                .ForMember(dest => dest.ObjectCreationTimeUtc, opts => opts.MapFrom(src => src.CreationTimeUtc))
                .ForMember(dest => dest.ObjectIsRemoved, opts => opts.MapFrom(src => src.IsRemoved))
                .ForMember(dest => dest.ObjectRemovalTimeUtc, opts => opts.MapFrom(src => src.RemovalTimeUtc));
            CreateMap<Training, SkorpFiles.Memorizer.Api.Models.Training>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.TrainingId))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.TrainingName))
                .ForMember(dest => dest.LastTimeUtc, opts => opts.MapFrom(src => src.TrainingLastTimeUtc))
                .ForMember(dest => dest.LengthType, opts => opts.MapFrom(src => src.TrainingLengthType))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.TrainingQuestionsCount))
                .ForMember(dest => dest.TimeMinutes, opts => opts.MapFrom(src => src.TrainingTimeMinutes))
                .ForMember(dest => dest.NewQuestionsFraction, opts => opts.MapFrom(src => src.TrainingNewQuestionsFraction))
                .ForMember(dest => dest.PenaltyQuestionsFraction, opts => opts.MapFrom(src => src.TrainingPenaltyQuestionsFraction))
                .ForMember(dest => dest.Questionnaires, opts =>
                {
                    opts.Condition(src => src.QuestionnairesForTraining != null);
                    opts.MapFrom(src => src.QuestionnairesForTraining!.Select(q => q.Questionnaire));
                })
                .ForMember(dest => dest.CreationTimeUtc, opts => opts.MapFrom(src => src.ObjectCreationTimeUtc))
                .ForMember(dest => dest.IsRemoved, opts => opts.MapFrom(src => src.ObjectIsRemoved))
                .ForMember(dest => dest.RemovalTimeUtc, opts => opts.MapFrom(src => src.ObjectRemovalTimeUtc));
            CreateMap<TrainingResultTypedAnswer, SkorpFiles.Memorizer.Api.Models.GivenTypedAnswer>()
                .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.TrtaAnswer))
                .ForMember(dest => dest.IsCorrect, opts => opts.MapFrom(src => src.TrtaIsCorrect));
            CreateMap<SkorpFiles.Memorizer.Api.Models.GivenTypedAnswer, TrainingResultTypedAnswer>()
                .ForMember(dest => dest.TrtaAnswer, opts => opts.MapFrom(src => src.Text))
                .ForMember(dest => dest.TrtaIsCorrect, opts => opts.MapFrom(src => src.IsCorrect));
            CreateMap<SkorpFiles.Memorizer.Api.Models.TrainingResult, TrainingResult>()
                .ForMember(dest => dest.TrainingResultRating, opts =>
                {
                    opts.Condition(src => src.ResultQuestionStatus != null);
                    opts.MapFrom(src => src.ResultQuestionStatus!.Rating);
                })
                .ForMember(dest => dest.TrainingResultIsNew, opts =>
                {
                    opts.Condition(src => src.ResultQuestionStatus != null);
                    opts.MapFrom(src => src.ResultQuestionStatus!.IsNew);
                })
                .ForMember(dest => dest.TrainingResultPenaltyPoints, opts =>
                {
                    opts.Condition(src => src.ResultQuestionStatus != null);
                    opts.MapFrom(src => src.ResultQuestionStatus!.PenaltyPoints);
                })
                .ForMember(dest => dest.TrainingResultInitialRating, opts =>
                {
                    opts.Condition(src => src.InitialQuestionStatus != null);
                    opts.MapFrom(src => src.InitialQuestionStatus!.Rating);
                })
                .ForMember(dest => dest.TrainingResultInitialNewStatus, opts =>
                {
                    opts.Condition(src => src.InitialQuestionStatus != null);
                    opts.MapFrom(src => src.InitialQuestionStatus!.IsNew);
                })
                .ForMember(dest => dest.TrainingResultInitialPenaltyPoints, opts =>
                {
                    opts.Condition(src => src.InitialQuestionStatus != null);
                    opts.MapFrom(src => src.InitialQuestionStatus!.PenaltyPoints);
                })
                .ForMember(dest => dest.TrainingResultQuestionId, opts => opts.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.TrainingResultAnswerIsCorrect, opts => opts.MapFrom(src => src.IsAnswerCorrect))
                .ForMember(dest => dest.TrainingResultRecordingTime, opts => opts.MapFrom(src => src.RecordingTime))
                .ForMember(dest => dest.TrainingResultTimeSeconds, opts => opts.MapFrom(src => (int)Math.Round((double)(src.AnswerTimeMilliseconds / 1000))))
                .ForMember(dest => dest.TrainingResultUserId, opts => opts.MapFrom(src => src.UserId))
                .ForMember(dest => dest.TypedAnswers, opts => opts.MapFrom(src => src.GivenTypedAnswers));
        }
    }
}