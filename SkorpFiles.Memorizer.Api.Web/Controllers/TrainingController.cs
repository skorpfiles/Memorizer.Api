using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SkorpFiles.Memorizer.Api.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [Route("Start")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> StartAsync(Guid id)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var training = await _editingLogic.GetTrainingAsync(userGuid, id);
                if (training?.Questionnaires != null)
                {
                    var result = await _trainingLogic.SelectQuestionsForTrainingAsync(userGuid, training.Questionnaires.Select(q => q.Id!.Value).ToList(), _mapper.Map<TrainingOptions>(training));
                    if (result != null)
                        return Ok(_mapper.Map<StartTrainingResponse>(result));
                    else
                        throw new InternalErrorException("The database hasn't returned questions for training.");
                }
                else
                    throw new ObjectNotFoundException("There is no such training.");
            });
        }

        [Route("UpdateQuestionStatus")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateQuestionStatusAsync(Web.Models.Requests.Training.TrainingResultRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _trainingLogic.UpdateQuestionStatusAsync(userGuid, _mapper.Map<Api.Models.RequestModels.TrainingResultRequest>(request));
                return Ok();
            });
        }
    }
}
