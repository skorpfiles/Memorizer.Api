using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class EditingLogic : IEditingLogic
    {
        private readonly IEditingRepository _editingRepository;

        public EditingLogic(IEditingRepository editingRepository)
        {
            _editingRepository = editingRepository;
        }

        public async Task<IEnumerable<Questionnaire>> GetQuestionnairesAsync(Guid userId, GetQuestionnairesRequest request)
        {
            return await _editingRepository.GetQuestionnairesAsync(userId, request);
        }
    }
}
