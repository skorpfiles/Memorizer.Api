﻿using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses
{
    public class GetQuestionnairesResponse:PaginatedCollectionResponse
    {
        public List<Questionnaire>? Questionnaires { get; set; }
    }
}
