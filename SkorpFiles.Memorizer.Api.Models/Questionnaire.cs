﻿using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models
{
    public class Questionnaire:Entity
    {
        public string? Name { get; set; }
        public Availability Availability { get; set; }
        public Guid OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public QuestionsCounts? CountsOfQuestions { get; set; }
        public List<Label>? Labels { get; set; }
        public long? TotalTrainingTimeSeconds { get; set; }
    }
}
