using AutoMapper;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;

namespace SkorpFiles.Memorizer.Api.Web.Mapping
{
    public class ResponsesMappingProfile:Profile
    {
        public ResponsesMappingProfile()
        {
            CreateMap<SkorpFiles.Memorizer.Api.Models.PaginatedCollection<SkorpFiles.Memorizer.Api.Models.Questionnaire>, GetQuestionnairesResponse>()
                .ForMember(dest => dest.Questionnaires, opts => opts.MapFrom(src => src.Items.ToList()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.PaginatedCollection<SkorpFiles.Memorizer.Api.Models.Question>, GetQuestionsResponse>()
                .ForMember(dest => dest.Questions, opts => opts.MapFrom(src => src.Items.ToList()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.PaginatedCollection<SkorpFiles.Memorizer.Api.Models.ExistingQuestion>, GetQuestionsResponse>()
                .ForMember(dest => dest.Questions, opts => opts.MapFrom(src => src.Items.ToList()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.PaginatedCollection<SkorpFiles.Memorizer.Api.Models.Label>, GetLabelsResponse>()
                .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.Items.ToList()));
            CreateMap<SkorpFiles.Memorizer.Api.Models.PaginatedCollection<SkorpFiles.Memorizer.Api.Models.Training>, GetTrainingsResponse>()
                .ForMember(dest => dest.Trainings, opts => opts.MapFrom(src => src.Items.ToList()));
            CreateMap<IEnumerable<Api.Models.ExistingQuestion>, StartTrainingResponse>()
                .ForMember(dest => dest.Questions, opts => opts.MapFrom(src => src.ToList()));
        }
    }
}
