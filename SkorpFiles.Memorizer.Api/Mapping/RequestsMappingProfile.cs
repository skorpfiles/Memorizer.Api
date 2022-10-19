using AutoMapper;
using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Mapping
{
    public class RequestsMappingProfile : Profile
    {
        public RequestsMappingProfile()
        {
            CreateMap<SkorpFiles.Memorizer.Api.ApiModels.Requests.Repository.GetQuestionnairesRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>()
                .ForMember(dest => dest.Origin, opts =>
                {
                    opts.Condition(src => src.Origin != null && Enum.TryParse<QuestionnaireOrigin>(src.Origin, true, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireOrigin>(src.Origin, true));
                })
                .ForMember(dest => dest.Availability, opts =>
                {
                    opts.Condition(src => src.Availability != null && Enum.TryParse<QuestionnaireAvailability>(src.Availability, true, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireAvailability>(src.Availability, true));
                })
                .ForMember(dest => dest.SortField, opts =>
                {
                    opts.Condition(src => src.SortField != null && Enum.TryParse<QuestionnaireSortField>(src.SortField, true, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireSortField>(src.SortField, true));
                })
                .ForMember(dest => dest.SortDirection, opts =>
                {
                    opts.Condition(src => src.SortDirection != null && Enum.TryParse<SortDirection>(src.SortDirection, true, out _));
                    opts.MapFrom(src => Enum.Parse<SortDirection>(src.SortDirection, true));
                });
        }
    }
}
