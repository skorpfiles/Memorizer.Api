using AutoMapper;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public class RequestsMappingProfile : Profile
    {
        public RequestsMappingProfile()
        {
            CreateMap<GetQuestionnairesRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>()
                .ForMember(dest => dest.Origin, opts =>
                {
                    opts.Condition(src => src.Origin != null && Enum.TryParse<Origin>(src.Origin, true, out _));
                    opts.MapFrom(src => Enum.Parse<Origin>(src.Origin!, true));
                })
                .ForMember(dest => dest.Availability, opts =>
                {
                    opts.Condition(src => src.Availability != null && Enum.TryParse<Availability>(src.Availability, true, out _));
                    opts.MapFrom(src => Enum.Parse<Availability>(src.Availability!, true));
                })
                .ForMember(dest => dest.SortField, opts =>
                {
                    opts.Condition(src => src.SortField != null && Enum.TryParse<QuestionnaireSortField>(src.SortField, true, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireSortField>(src.SortField!, true));
                })
                .ForMember(dest => dest.SortDirection, opts =>
                {
                    opts.Condition(src => src.SortDirection != null && Enum.TryParse<SortDirection>(src.SortDirection, true, out _));
                    opts.MapFrom(src => Enum.Parse<SortDirection>(src.SortDirection!, true));
                });
            CreateMap<GetQuestionsRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionsRequest>()
                .ForMember(dest => dest.SortField, opts =>
                {
                    opts.Condition(src => src.SortField != null && Enum.TryParse<QuestionSortField>(src.SortField, true, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionSortField>(src.SortField!, true));
                })
                .ForMember(dest => dest.SortDirection, opts =>
                {
                    opts.Condition(src => src.SortDirection != null && Enum.TryParse<SortDirection>(src.SortDirection, true, out _));
                    opts.MapFrom(src => Enum.Parse<SortDirection>(src.SortDirection!, true));
                });
            CreateMap<PutQuestionnaireRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.UpdateQuestionnaireRequest>()
                .ForMember(dest => dest.Availability, opts =>
                {
                    opts.Condition(src => src.Availability != null && Enum.TryParse<Availability>(src.Availability, true, out _));
                    opts.MapFrom(src => Enum.Parse<Availability>(src.Availability!, true));
                });
            CreateMap<PostQuestionnaireRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.UpdateQuestionnaireRequest>()
                .ForMember(dest => dest.Availability, opts =>
                {
                    opts.Condition(src => src.Availability != null && Enum.TryParse<Availability>(src.Availability, true, out _));
                    opts.MapFrom(src => Enum.Parse<Availability>(src.Availability!, true));
                })
                .ForMember(dest => dest.Labels, opts =>
                {
                    opts.Condition(src => src.LabelsIds != null);
                    opts.MapFrom(src => src.LabelsIds!.Select(l => new SkorpFiles.Memorizer.Api.Models.LabelInQuestionnaire { Id = l }).ToList());
                });
            CreateMap<PostQuestionsRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.UpdateQuestionsRequest>();
            CreateMap<PostMyStatusRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.UpdateUserQuestionStatusesRequest>();
            CreateMap<GetLabelsRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetLabelsRequest>();
            CreateMap<PostTrainingRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.UpdateTrainingRequest>()
                .ForMember(dest => dest.RefreshLastTime, opts => opts.MapFrom(src => src.RefreshLastTime ?? false));
            CreateMap<CollectionRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetCollectionRequest>();
            CreateMap<Api.Models.Training, Api.Models.RequestModels.TrainingOptions>()
                .ForMember(dest => dest.LengthValue, opts =>
                {
                    opts.Condition(src => src.LengthType == TrainingLengthType.QuestionsCount || src.LengthType == TrainingLengthType.Time);
                    opts.MapFrom(src => src.LengthType == TrainingLengthType.QuestionsCount ? src.QuestionsCount : src.TimeMinutes * Constants.SecondsInMinute);
                })
                .ForMember(dest => dest.PrioritizedPenaltyQuestionsFraction, opts => opts.MapFrom(src => src.PenaltyQuestionsFraction));
            CreateMap<Web.Models.Requests.Training.TrainingResultRequest, Api.Models.TrainingResult>();
            CreateMap<Web.Models.ApiEntities.GivenTypedAnswer, Api.Models.GivenTypedAnswer>();
        }
    }
}
