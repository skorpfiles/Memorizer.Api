using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies.WeightedRandomSamplingStrategy
{
    internal class WeightedRandomSamplingStrategy:CapacityRestrictedMakingListStrategy<EntitiesListForRandomChoice<GetQuestionsForTrainingResult>, EntitiesListForRandomChoice<GetQuestionsForTrainingResult>>
    {
        public WeightedRandomSamplingStrategy(IEnumerable<GetQuestionsForTrainingResult> initialQuestionsList) : base(initialQuestionsList) { }

        internal override IPickableTrainingList<GetQuestionsForTrainingResult> GetPickerForBasicList(IEnumerable<SkorpFiles.Memorizer.Api.Models.Abstract.Entity> entitiesHaveBeenAlreadyChosen)
        {
            const double alpha = 10;
            return new WeightedRandomSamplingPicker<GetQuestionsForTrainingResult>(BasicQuestionsList, x => x.QuestionUserRating ?? Restrictions.InitialQuestionRating, Random, alpha);
        }
    }
}
