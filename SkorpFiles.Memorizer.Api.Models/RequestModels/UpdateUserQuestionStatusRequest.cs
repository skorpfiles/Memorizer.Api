using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class UpdateUserQuestionStatusRequest
    {
        public IEnumerable<UserQuestionStatus>? Items { get; set; }
    }
}
