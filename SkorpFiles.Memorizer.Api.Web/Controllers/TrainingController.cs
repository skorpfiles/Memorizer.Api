using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrainingController(ITrainingLogic trainingLogic, 
        IEditingLogic editingLogic, UserManager<DataAccess.Models.ApplicationUser> userManager, 
        IUserStore<DataAccess.Models.ApplicationUser> userStore, IMapper mapper) : ControllerWithUserInfo(userManager, userStore)
    {
        [Route("Start")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> StartAsync(Guid id)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var training = await editingLogic.GetTrainingAsync(userGuid, id);
                if (training?.Questionnaires != null)
                {
                    var result = (await trainingLogic.SelectQuestionsForTrainingAsync(userGuid, training.Questionnaires.Select(q => q.Id!.Value).ToList(), mapper.Map<TrainingOptions>(training))).ToList();
                    if (result != null)
                        return Ok(mapper.Map<StartTrainingResponse>(result));
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
                var result = await trainingLogic.UpdateQuestionStatusAsync(userGuid, mapper.Map<Api.Models.TrainingResult>(request));
                return Ok(mapper.Map<UserQuestionStatus>(result));
            });
        }
    }
}
