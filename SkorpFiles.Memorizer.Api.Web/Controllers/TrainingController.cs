using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SkorpFiles.Memorizer.Api.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    public class TrainingController : ControllerWithUserInfo
    {
        private readonly ITrainingLogic _trainingLogic;
        private readonly IEditingLogic _editingLogic;
        private readonly IMapper _mapper;

        public TrainingController(ITrainingLogic trainingLogic, IEditingLogic editingLogic, UserManager<DataAccess.Models.ApplicationUser> userManager, IUserStore<DataAccess.Models.ApplicationUser> userStore, IMapper mapper) : base(userManager, userStore)
        {
            _trainingLogic = trainingLogic;
            _editingLogic = editingLogic;
            _mapper = mapper;
        }

        public async Task<IActionResult> StartAsync(Guid trainingId)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var training = await _editingLogic.GetTrainingAsync(userGuid, trainingId);
                if (training?.Questionnaires != null)
                {
                    var result = await _trainingLogic.SelectQuestionsForTrainingAsync(userGuid, training.Questionnaires.Select(q => q.Id).ToList(),
                        new Api.Models.RequestModels.TrainingOptions
                        {
                            LengthType = training.LengthType,
                            LengthValue = training.LengthType == Api.Models.Enums.TrainingLengthType.QuestionsCount? 
                            training.QuestionsCount:
                            (training.LengthType == Api.Models.Enums.TrainingLengthType.Time?training.TimeMinutes*Constants.SecondsInMinute:throw new InternalErrorException("Invalid length type."),
                            NewQuestionsFraction = training.NewQuestionsFraction,
                            PrioritizedPenaltyQuestionsFraction = training.PrioritizedPenaltyQuestionsFraction
                        });
                    if (result != null)
                        return Ok(_mapper.Map<StartTrainingResponse>(result));
                    else
                        throw new InternalErrorException("The database hasn't returned a result.");
                }
                else
                    throw new ObjectNotFoundException("There is no such training.");
            });
        }
    }
}
