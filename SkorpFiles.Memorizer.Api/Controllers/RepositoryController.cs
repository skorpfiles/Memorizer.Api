using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.ApiModels.Requests.Repository;
using SkorpFiles.Memorizer.Api.ApiModels.Responses;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;

namespace SkorpFiles.Memorizer.Api.Controllers
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
            var result = await _editingLogic.GetQuestionnairesAsync(userGuid, _mapper.Map<Models.RequestModels.GetQuestionnairesRequest>(request));
            return Ok(_mapper.Map<GetQuestionnairesResponse>(result));
        }
    }
}
