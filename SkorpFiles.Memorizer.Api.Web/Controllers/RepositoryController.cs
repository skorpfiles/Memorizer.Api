using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Web.Controllers.Abstract;
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
        public async Task<IActionResult> QuestionnairesAsync(GetQuestionnairesRequest request)
        {
            var userGuid = await GetCurrentUserGuidAsync();
            var result = await _editingLogic.GetQuestionnairesAsync(userGuid, _mapper.Map<SkorpFiles.Memorizer.Api.Models.RequestModels.GetQuestionnairesRequest>(request));
            return Ok(_mapper.Map<GetQuestionnairesResponse>(result));
        }

        [Route("Questionnaire/{idOrCode}")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> QuestionnaireAsync(string idOrCode)
        {
            object idOrCodeObj;
            IdOrCode isIdOrCode;

            try
            {
                isIdOrCode = GetIdType(idOrCode, out idOrCodeObj);
            }
            catch(ArgumentException)
            {
                return BadRequest("Unsupported ID parameter: only GUID or integer are supported.");
            }

            SkorpFiles.Memorizer.Api.Models.Questionnaire resultQuestionnaire;

            try
            {
                if (isIdOrCode == IdOrCode.Id)
                {
                    resultQuestionnaire = await _editingLogic.GetQuestionnaireAsync(
                        await GetCurrentUserGuidAsync(), (Guid)idOrCodeObj);
                }
                else if (isIdOrCode == IdOrCode.Code)
                {
                    resultQuestionnaire = await _editingLogic.GetQuestionnaireAsync(
                        await GetCurrentUserGuidAsync(), (int)idOrCodeObj);
                }
                else
                    throw new NotImplementedException("Enum method is not implemented.");

                return Ok(_mapper.Map<Questionnaire>(resultQuestionnaire));
            }
            catch(AccessDeniedForUserException e)
            {
                return Unauthorized(e.Message);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(ObjectNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [Route("Questions")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> QuestionsAsync(GetQuestionsRequest request)
        {
            var userGuid = await GetCurrentUserGuidAsync();
            try
            {
                var result = await _editingLogic.GetQuestionsAsync(userGuid, _mapper.Map<Api.Models.RequestModels.GetQuestionsRequest>(request));
                return Ok(_mapper.Map<GetQuestionsResponse>(result));
            }
            catch(AccessDeniedForUserException e)
            {
                return Unauthorized(e.Message);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(ObjectNotFoundException e)
            {
                return NotFound(e.Message);
            }
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
    }
}
