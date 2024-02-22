using SkorpFiles.Memorizer.Api.Models;
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
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            int result;
            if (!question.MyStatus!.IsNew)
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
