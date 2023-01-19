using AutoMapper;
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
using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Repository;
using SkorpFiles.Memorizer.Api.Web.Models.Responses;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepositoryController:ControllerWithUserInfo
    {
        private readonly IEditingLogic _editingLogic;
        private readonly IMapper _mapper;

        public RepositoryController(IEditingLogic editingLogic, UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, IMapper mapper):base(userManager,userStore)
        {
            _editingLogic = editingLogic;
            _mapper = mapper;
        }

        [Route("Questionnaires")]
        [HttpGet]
        [EnableCors]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionnairesAsync([FromQuery]Web.Models.Requests.Repository.GetQuestionnairesRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                RestoreDefaultPageValues(request);
                var userGuid = await GetCurrentUserGuidAsync();
                var result = await _editingLogic.GetQuestionnairesAsync(userGuid, _mapper.Map<SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>(request));
                return Ok(_mapper.Map<GetQuestionnairesResponse>(result));
            });
        }

        [Route("Questionnaire/{idOrCode}", Name = "GetQuestionnaire")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionnaireAsync(string idOrCode)
        {
            return await SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync(idOrCode,
                async (id) => await _editingLogic.GetQuestionnaireAsync(await GetCurrentUserGuidAsync(), id),
                async (code) => await _editingLogic.GetQuestionnaireAsync(await GetCurrentUserGuidAsync(), code),
                (businessLogicResult) => Ok(_mapper.Map<Questionnaire>(businessLogicResult)));
        }

        [Route("Questionnaire")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutQuestionnaireAsync(PutQuestionnaireRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var creatingResult = await _editingLogic.CreateQuestionnaireAsync(await GetCurrentUserGuidAsync(), _mapper.Map<Api.Models.RequestModels.UpdateQuestionnaireRequest>(request));
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
                await _editingLogic.UpdateQuestionnaireAsync(await GetCurrentUserGuidAsync(), _mapper.Map<Api.Models.RequestModels.UpdateQuestionnaireRequest>(request));
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
        [EnableCors]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionsAsync([FromQuery]Web.Models.Requests.Repository.GetQuestionsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                RestoreDefaultPageValues(request);
                var result = await _editingLogic.GetQuestionsAsync(await GetCurrentUserGuidAsync(), _mapper.Map<Api.Models.RequestModels.GetQuestionsRequest>(request));
                return Ok(_mapper.Map<GetQuestionsResponse>(result));
            });
        }

        [Route("Questions")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateQuestionsAsync(PostQuestionsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                await _editingLogic.UpdateQuestionsAsync(await GetCurrentUserGuidAsync(), _mapper.Map<Api.Models.RequestModels.UpdateQuestionsRequest>(request));
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
                    _mapper.Map<Api.Models.RequestModels.UpdateUserQuestionStatusRequest>(request));
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
                    _mapper.Map<Api.Models.RequestModels.GetLabelsRequest>(request));
                return Ok(_mapper.Map<GetLabelsResponse>(result));
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
                (businessLogicResult) => Ok(_mapper.Map<Label>(businessLogicResult)));
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

        private enum IdOrCode
        {
            Id,
            Code
        }

        private static IdOrCode GetIdType(string idOrCode, out object result)
        {
            bool isId = Guid.TryParse(idOrCode, out Guid guidResult);
            bool isCode = int.TryParse(idOrCode, out int intResult);
            if (isId)
            {
                result = guidResult;
                return IdOrCode.Id;
            }
            else if (isCode)
            {
                result = intResult;
                return IdOrCode.Code;
            }
            else
                throw new ArgumentException("The value is not a GUID ID or an int code.");
        }

        private async Task<IActionResult> ExecuteActionToBusinessLogicAsync(Func<Task<IActionResult>> actionAsync)
        {
            IActionResult result;
            try
            {
                result = await actionAsync();
            }
            catch (AccessDeniedForUserException e)
            {
                result = Unauthorized(e.Message);
            }
            catch (BadRequestException e)
            {
                result = BadRequest(e.Message);
            }
            catch (ObjectNotFoundException e)
            {
                result = NotFound(e.Message);
            }
            return result;
        }

        private async Task<IActionResult> SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync<TBusinessLogicReturningType>(
            string idOrCode, Func<Guid, Task<TBusinessLogicReturningType>> actionIfIdAsync, Func<int, Task<TBusinessLogicReturningType>> actionIfCodeAsync,
            Func<TBusinessLogicReturningType, IActionResult> processResultAction)
        {
            object idOrCodeObj;
            IdOrCode isIdOrCode;
            IActionResult result;

            try
            {
                isIdOrCode = GetIdType(idOrCode, out idOrCodeObj);
            }
            catch (ArgumentException)
            {
                return BadRequest("Unsupported ID parameter: only GUID or integer are supported.");
            }

            result = await ExecuteActionToBusinessLogicAsync(async () =>
            {
                TBusinessLogicReturningType businessLogicResult;

                if (isIdOrCode == IdOrCode.Id)
                    businessLogicResult = await actionIfIdAsync((Guid)idOrCodeObj);
                else if (isIdOrCode == IdOrCode.Code)
                    businessLogicResult = await actionIfCodeAsync((int)idOrCodeObj);
                else
                    throw new NotImplementedException("Enum method is not implemented.");

                return processResultAction(businessLogicResult);
            });

            return result;
        }

        private void RestoreDefaultPageValues(Models.Requests.Repository.Abstract.CollectionRequest request, int? defaultPageSize=null)
        {
            if (request.PageNumber == 0)
                request.PageNumber = 1;
            if (request.PageSize == 0)
                request.PageSize = defaultPageSize ?? 50;
        }
    }
}
