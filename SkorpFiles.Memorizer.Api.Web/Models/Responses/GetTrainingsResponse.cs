﻿using SkorpFiles.Memorizer.Api.Web.Models.ApiEntities;
using SkorpFiles.Memorizer.Api.Web.Models.Responses.Abstract;

namespace SkorpFiles.Memorizer.Api.Web.Models.Responses
{
    public class GetTrainingsResponse : PaginatedCollectionResponse
    {
        public List<Training>? Trainings { get; set; }
    }
}