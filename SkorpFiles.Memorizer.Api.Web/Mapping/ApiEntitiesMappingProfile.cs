using AutoMapper;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public class ApiEntitiesMappingProfile:Profile
    {
        public ApiEntitiesMappingProfile()
        {
            CreateMap<SkorpFiles.Memorizer.Api.Models.Questionnaire, Questionnaire>()
                .ForMember(dest => dest.Availability, opts => opts.MapFrom(src => src.Availability.ToString()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.Label, Label>()
                .ForMember(dest => dest.Number, opts => opts.MapFrom(src => src.StatusInQuestionnaire!.Number))
                .ForMember(dest => dest.ParentLabelId, opts => opts.MapFrom(src => src.StatusInQuestionnaire!.ParentLabelId));
        }
    }
}
