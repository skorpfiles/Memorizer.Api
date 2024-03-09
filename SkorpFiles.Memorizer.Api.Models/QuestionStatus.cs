using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class QuestionStatus
    {
        public bool IsNew {  get; set; }
        public int Rating { get; set; }
        public int PenaltyPoints { get; set; }
    }
}
