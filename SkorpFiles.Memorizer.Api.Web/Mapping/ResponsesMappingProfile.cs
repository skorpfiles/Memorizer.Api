using AutoMapper;
using SkorpFiles.Memorizer.Api.Web.Models.Responses;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
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
