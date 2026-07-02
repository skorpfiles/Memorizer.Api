using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Mapping;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Training;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrainingController(ITrainingLogic trainingLogic,
        IEditingLogic editingLogic, UserManager<DataAccess.Models.ApplicationUser> userManager,
        IUserStore<DataAccess.Models.ApplicationUser> userStore) : ControllerWithUserInfo(userManager, userStore)
    {
        [Route("Start")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> StartAsync(Guid id)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var training = await editingLogic.GetTrainingAsync(userGuid, id, false);
                if (training?.Questionnaires != null)
                {
                    var result = (await trainingLogic.SelectQuestionsForTrainingAsync(userGuid, training.Questionnaires.Select(q => q.Id!.Value).ToList(), training.MapTo<TrainingOptions>())).ToList();
                    if (result != null)
                    {
                        StartTrainingResponse response = result.MapTo<StartTrainingResponse>();
                        response.Name = training.Name;

                        await editingLogic.UpdateTrainingAsync(userGuid, new UpdateTrainingRequest { Id = training.Id, RefreshLastTime = true });

                        return Ok(response);
                    }
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
                if (request.QuestionId == Guid.Empty)
                    throw new BadRequestException("QuestionId mustn't be empty.");
                if (request.AnswerTimeMilliseconds <= 0)
                    throw new BadRequestException("Answer Time must be positive.");

                var userGuid = await GetCurrentUserGuidAsync();
                var result = await trainingLogic.UpdateQuestionStatusAsync(userGuid, request.MapTo<Api.Models.TrainingResult>());
                return Ok(result.MapTo<UserQuestionStatus>());
            });
        }
    }
}
