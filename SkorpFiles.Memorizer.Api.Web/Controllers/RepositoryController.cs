using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Mapping;
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository.Abstract;
using SkorpFiles.Memorizer.Api.Web.Models.Responses;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Repository;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepositoryController:ControllerWithUserInfo
    {
        private readonly IEditingLogic _editingLogic;

        public RepositoryController(IEditingLogic editingLogic, UserManager<DataAccess.Models.ApplicationUser> userManager, IUserStore<DataAccess.Models.ApplicationUser> userStore):base(userManager,userStore)
        {
            _editingLogic = editingLogic;
        }

        [Route("Questionnaires")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionnairesAsync([FromQuery]Web.Models.Requests.Repository.GetQuestionnairesRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                RestoreDefaultPageValues(request);
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.GetQuestionnairesAsync(userGuid, request.MapTo<SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>());
                return Ok(result.MapTo<GetQuestionnairesResponse>());
            });
        }

        [Route("Questionnaire/{idOrCode}", Name = "GetQuestionnaire")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionnaireAsync(string idOrCode, [FromQuery]bool calculateTime)
        {
            return await SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync(idOrCode,
                async (id) => await _editingLogic.GetQuestionnaireAsync(await GetCurrentUserGuidAsync(), id, calculateTime),
                async (code) => await _editingLogic.GetQuestionnaireAsync(await GetCurrentUserGuidAsync(), code, calculateTime),
                (businessLogicResult) => Ok(businessLogicResult?.MapTo<Questionnaire>()));
        }

        [Route("Questionnaire")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutQuestionnaireAsync(PutQuestionnaireRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var creatingResult = await _editingLogic.CreateQuestionnaireAsync(await GetCurrentUserGuidAsync(), request.MapTo<Api.Models.RequestModels.UpdateQuestionnaireRequest>());
                if (creatingResult != null)
                    return CreatedAtRoute("GetQuestionnaire", new { idOrCode = creatingResult.Code.ToString() },
                    new IdentifiersGroupResponse
                    {
                        Code = creatingResult.Code!.Value,
                        Id = creatingResult.Id!.Value
                    });
                else
                    throw new InternalErrorException("The database hasn't returned a result.");
            });
        }

        [Route("Questionnaire")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostQuestionnaireAsync(PostQuestionnaireRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                await _editingLogic.UpdateQuestionnaireAsync(await GetCurrentUserGuidAsync(), request.MapTo<Api.Models.RequestModels.UpdateQuestionnaireRequest>());
                return Ok();
            });
        }

        [Route("Questionnaire/{idOrCode}")]
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteQuestionnaireAsync(string idOrCode)
        {
            return await SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync(idOrCode,
                async (id) =>
                {
                    await _editingLogic.DeleteQuestionnaireAsync(await GetCurrentUserGuidAsync(), id);
                    return true;
                },
                async (code) =>
                {
                    await _editingLogic.DeleteQuestionnaireAsync(await GetCurrentUserGuidAsync(), code);
                    return true;
                },
                (_) => Ok());
        }

        [Route("Questions")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionsAsync([FromQuery]Web.Models.Requests.Repository.GetQuestionsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                RestoreDefaultPageValues(request);
                var result = await _editingLogic.GetQuestionsAsync(await GetCurrentUserGuidAsync(), request.MapTo<Api.Models.RequestModels.GetQuestionsRequest>());
                return Ok(result.MapTo<GetQuestionsResponse>());
            });
        }

        [Route("Questions")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateQuestionsAsync(PostQuestionsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                await _editingLogic.UpdateQuestionsAsync(await GetCurrentUserGuidAsync(), request.MapTo<Api.Models.RequestModels.UpdateQuestionsRequest>());
                return Ok();
            });
        }

        [Route("Questions/MyStatus")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MyStatus(PostMyStatusRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                await _editingLogic.UpdateUserQuestionStatusAsync(await GetCurrentUserGuidAsync(),
                    request.MapTo<Api.Models.RequestModels.UpdateUserQuestionStatusesRequest>());
                return Ok();
            });
        }

        [Route("Labels")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetLabelsAsync(Web.Models.Requests.Repository.GetLabelsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var result = await _editingLogic.GetLabelsAsync(await GetCurrentUserGuidAsync(),
                    request.MapTo<Api.Models.RequestModels.GetLabelsRequest>());
                return Ok(result.MapTo<GetLabelsResponse>());
            });
        }

        [Route("Label/{idOrCode}", Name = "GetLabel")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetLabelAsync(string idOrCode)
        {
            return await SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync(idOrCode,
                async (id) => await _editingLogic.GetLabelAsync(await GetCurrentUserGuidAsync(), id),
                async (code) => await _editingLogic.GetLabelAsync(await GetCurrentUserGuidAsync(), code),
                (businessLogicResult) => Ok(businessLogicResult?.MapTo<Label>()));
        }

        [Route("Label")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutLabelAsync([FromQuery]string name)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var result = await _editingLogic.CreateLabelAsync(await GetCurrentUserGuidAsync(), name);
                if (result != null)
                    return CreatedAtRoute("GetLabel", new { idOrCode = result.Code.ToString() },
                    new IdentifiersGroupResponse
                    {
                        Code = result.Code!.Value,
                        Id = result.Id!.Value
                    });
                else
                    throw new InternalErrorException("The database hasn't returned a result.");
            });
        }

        [Route("Label/{idOrCode}")]
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteLabelAsync(string idOrCode)
        {
            return await SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync(idOrCode,
                async (id) =>
                {
                    await _editingLogic.DeleteLabelAsync(await GetCurrentUserGuidAsync(), id);
                    return true;
                },
                async (code) =>
                {
                    await _editingLogic.DeleteLabelAsync(await GetCurrentUserGuidAsync(), code);
                    return true;
                },
                (_) => Ok());
        }

        [Route("Trainings")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTrainingsForUserAsync([FromQuery]CollectionRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                RestoreDefaultPageValues(request);
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.GetTrainingsForUserAsync(userGuid, request.MapTo<SkorpFiles.Memorizer.Api.Models.RequestModels.GetCollectionRequest>());
                return Ok(result.MapTo<GetTrainingsResponse>());
            });
        }

        [Route("Training/{id}", Name = "GetTraining")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTrainingAsync(Guid id, [FromQuery]bool calculateTime)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.GetTrainingAsync(userGuid, id, calculateTime);
                return Ok(result.MapTo<Training>());
            });
        }

        [Route("Training")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutTrainingAsync(PostTrainingRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.CreateTrainingAsync(userGuid, request.MapTo<UpdateTrainingRequest>());
                if (result != null)
                    return CreatedAtRoute("GetTraining", new { id = result.Id.ToString() }, new { result.Id });
                else
                    throw new InternalErrorException("The database hasn't returned a result.");
            });
        }

        [Route("Training")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostTrainingAsync(PostTrainingRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.UpdateTrainingAsync(userGuid, request.MapTo<UpdateTrainingRequest>());
                if (result != null)
                    return Ok();
                else
                    throw new InternalErrorException("The database hasn't returned a result.");
            });
        }

        [Route("Training/{id}")]
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteTrainingAsync(Guid id)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var userGuid = await GetCurrentUserGuidAsync();
                await _editingLogic.DeleteTrainingAsync(userGuid, id);
                return Ok();
            });
        }

        private static void RestoreDefaultPageValues(CollectionRequest request, int? defaultPageSize=null)
        {
            if (request.PageNumber == 0)
                request.PageNumber = 1;
            if (request.PageSize == 0)
                request.PageSize = defaultPageSize ?? 50;
        }
    }
}
