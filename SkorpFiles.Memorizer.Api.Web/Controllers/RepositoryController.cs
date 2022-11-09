using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQuestionnairesAsync(GetQuestionnairesRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
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
                        Code = creatingResult.Code,
                        Id = creatingResult.Id
                    });
                else
                    throw new InternalErrorException("The database hasn't returned a result.");
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
        public async Task<IActionResult> GetQuestionsAsync(GetQuestionsRequest request)
        {
            return await ExecuteActionToBusinessLogicAsync(async () =>
            {
                var result = await _editingLogic.GetQuestionsAsync(await GetCurrentUserGuidAsync(), _mapper.Map<Api.Models.RequestModels.GetQuestionsRequest>(request));
                return Ok(_mapper.Map<GetQuestionsResponse>(result));
            });
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
            catch (ArgumentException e)
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
    }
}
