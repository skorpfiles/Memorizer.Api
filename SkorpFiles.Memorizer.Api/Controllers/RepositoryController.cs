using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Requests.Repository;

namespace SkorpFiles.Memorizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepositoryController:Controller
    {
        [Route("Questionnaires")]
        [HttpGet]
        public async Task<IActionResult> QuestionnairesAsync(GetQuestionnariesRequest request)
        {
            return Ok();
        }
    }
}
