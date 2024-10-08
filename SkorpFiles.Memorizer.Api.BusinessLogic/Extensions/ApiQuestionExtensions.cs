﻿using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Extensions
{
    public static class ApiQuestionExtensions
    {
        public static int FullEstimatedTrainingTimeSeconds(this Question question)
        {
            ArgumentNullException.ThrowIfNull(question);

            int result;
            if (!question.MyStatus?.IsNew ?? true)
            {
                result = question.EstimatedTrainingTimeSeconds;
            }
            else
            {
                result = (int)Math.Round(question.EstimatedTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);
            }
            return result;
        }
    }
}
