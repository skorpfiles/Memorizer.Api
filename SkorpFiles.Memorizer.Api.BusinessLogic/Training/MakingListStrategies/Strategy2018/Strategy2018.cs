using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Abstract;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies.Strategy2018
{
    internal class Strategy2018 : CapacityRestrictedMakingListStrategy<EntitiesListForRandomChoice<GetQuestionsForTrainingResult>, EntitiesListForWeighedSoftmaxChoice>
    {
        public Strategy2018(IEnumerable<GetQuestionsForTrainingResult> initialQuestionsList) : base(initialQuestionsList) { }

        internal override IPickableTrainingList<GetQuestionsForTrainingResult> GetPickerForBasicList(IEnumerable<Entity> entitiesHaveBeenAlreadyChosen)
        {
            return InitializeRatingTape(BasicQuestionsList, entitiesHaveBeenAlreadyChosen);
        }

        private static RatingTape InitializeRatingTape(List<GetQuestionsForTrainingResult> basicList, IEnumerable<Entity> questionsToFilter)
        {
            RatingTape result = new(RatingToWeight);
            List<GetQuestionsForTrainingResult> questionsToRemoveFromBasicList = [];

            foreach (GetQuestionsForTrainingResult question in basicList)
            {
                if (!questionsToFilter.Any(q => q.Id == question.Id))
                {
                    result.Add(question);
                    questionsToRemoveFromBasicList.Add(question);
                }
            }

            return result;
        }

        private static int RatingToWeight(int rating)
        {
            return (int)Math.Round(10000 / ((51 - rating) * Math.Exp(-0.1081 * (rating - 1)) * 200));
        }
    }
}
