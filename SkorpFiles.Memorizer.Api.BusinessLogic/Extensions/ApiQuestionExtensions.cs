using SkorpFiles.Memorizer.Api.Models;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Extensions
{
    public static class ApiQuestionExtensions
    {
        public static int FullActualTrainingTimeSeconds(this Question question)
        {
            ArgumentNullException.ThrowIfNull(question);

            int result;
            int actualTimeTrainingTimeSeconds = (int)(question.MyStatus?.AverageTrainingTimeSeconds ?? question.EstimatedTrainingTimeSeconds);
            if (!question.MyStatus?.IsNew ?? true)
            {
                result = actualTimeTrainingTimeSeconds;
            }
            else
            {
                result = (int)Math.Round(actualTimeTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);
            }
            return result;
        }
    }
}
