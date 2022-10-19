using AutoMapper;
using SkorpFiles.Memorizer.Api.ApiModels.Responses;

namespace SkorpFiles.Memorizer.Api.Mapping
{
    public class ResponsesMappingProfile:Profile
    {
        public ResponsesMappingProfile()
        {
            CreateMap<IEnumerable<SkorpFiles.Memorizer.Api.Models.Questionnaire>, GetQuestionnairesResponse>()
                .ForMember(dest => dest.Questionnaires, opts => opts.MapFrom(src => src.ToList()));
        }
    }
}
