using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.RequestModels
{
    public class GetLabelsRequest:GetCollectionRequest
    {
        public Origin? Origin { get; set; }
        public Guid? OwnerId { get; set; }
        public string? PartOfName { get; set; }
    }
}
