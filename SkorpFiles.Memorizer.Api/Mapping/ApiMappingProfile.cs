using AutoMapper;
using SkorpFiles.Memorizer.Api.Models.Enums;

namespace SkorpFiles.Memorizer.Api.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<SkorpFiles.Memorizer.Api.ApiModels.Requests.Repository.GetQuestionnairesRequest, SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>()
                .ForMember(dest => dest.Origin, opts =>
                {
                    opts.Condition(src => src.Origin != null && Enum.TryParse<QuestionnaireOrigin>(src.Origin, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireOrigin>(src.Origin));
                })
                .ForMember(dest => dest.Availability, opts =>
                {
                    opts.Condition(src => src.Availability != null && Enum.TryParse<QuestionnaireAvailability>(src.Availability, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireAvailability>(src.Availability));
                })
                .ForMember(dest => dest.SortField, opts =>
                {
                    opts.Condition(src => src.SortField != null && Enum.TryParse<QuestionnaireSortField>(src.SortField, out _));
                    opts.MapFrom(src => Enum.Parse<QuestionnaireSortField>(src.SortField));
                })
                .ForMember(dest => dest.SortDirection, opts =>
                {
                    opts.Condition(src => src.SortDirection != null && Enum.TryParse<SortDirection>(src.SortDirection, out _));
                    opts.MapFrom(src => Enum.Parse<SortDirection>(src.SortDirection));
                });
        }
    }
}
