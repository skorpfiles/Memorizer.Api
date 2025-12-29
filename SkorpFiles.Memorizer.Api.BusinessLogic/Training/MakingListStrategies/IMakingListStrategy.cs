using AutoMapper;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.RequestModels;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies
{
    internal interface IMakingListStrategy
    {
        List<ExistingQuestion> MakeQuestionsListForTraining(TrainingOptions options, IMapper mapper);
    }
}
