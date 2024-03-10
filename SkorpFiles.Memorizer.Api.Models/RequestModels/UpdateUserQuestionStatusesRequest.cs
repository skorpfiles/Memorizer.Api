using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class UpdateUserQuestionStatusesRequest
    {
        public IEnumerable<UserQuestionStatus>? Items { get; set; }
    }
}
