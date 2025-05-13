using SkorpFiles.Memorizer.Api.Models;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Extensions
{
    public static class GetQuestionsForTrainingResultExtensions
    {
        public static int FullActualTrainingTimeSeconds(this GetQuestionsForTrainingResult question)
        {
            ArgumentNullException.ThrowIfNull(question);

            int result;
            if (!question.QuestionUserIsNew ?? true)
            {
                result = question.QuestionActualTrainingTimeSeconds;
            }
            else
            {
                result = (int)Math.Round(question.QuestionActualTrainingTimeSeconds * Constants.NewQuestionsLearningTimeMultiplicator);
            }
            return result;
        }
    }
}
