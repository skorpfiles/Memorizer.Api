using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.ApiModels.Requests.Repository;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;

namespace SkorpFiles.Memorizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepositoryController:Controller
    {
        private readonly IEditingLogic _editingLogic;

        public RepositoryController(IEditingLogic editingLogic)
        {
            _editingLogic = editingLogic;
        }

        [Route("Questionnaires")]
        [HttpGet]
        public async Task<IActionResult> QuestionnairesAsync(GetQuestionnairesRequest request)
        {
            await _editingLogic.GetQuestionnairesAsync(Guid.NewGuid(), new Models.RequestModels.GetQuestionnairesRequest());
            return Ok();
        }
    }
}
