using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal static class TrainingUtilities
    {
        public static Dictionary<Guid, GetQuestionsForTrainingResult> GetSelectedQuestionsFromGeneralList(IPickableTrainingList<GetQuestionsForTrainingResult> sourceList, Models.Enums.TrainingLengthType lengthType, double expectedLength, Random random, out int resultLength)
        {
            Dictionary<Guid, GetQuestionsForTrainingResult> selectedQuestions = [];

            int consumedValue = 0;
            int tryingAttemptInARowWithoutResult = 0;
            const int MaxCountOfTryingAttemptInARowWithoutResult = 100;
            bool stoppedByAlgorithm = false;

            if (Math.Round(expectedLength) > 0 && !sourceList.Consumed)
            {
                GetQuestionsForTrainingResult? selectedQuestion;
                do
                {
                    selectedQuestion = sourceList.PickAndDelete(random);
                    if (selectedQuestion?.Id == null)
                    {
                        throw new InvalidOperationException("Question cannot have null ID while creating training list.");
                    }

#pragma warning disable CA1864 // Prefer the 'IDictionary.TryAdd(TKey, TValue)' method
                    if (!selectedQuestions.ContainsKey(selectedQuestion.Id.Value))
                    {
                        switch (lengthType)
                        {
                            case Models.Enums.TrainingLengthType.Time:
                                int lengthValue = selectedQuestion.FullActualTrainingTimeSeconds();

                                if (consumedValue <= Math.Round(expectedLength))
                                {
                                    if (consumedValue + lengthValue <= Math.Round(expectedLength + expectedLength * Constants.AllowableErrorFraction))
                                    {
                                        selectedQuestions.Add(selectedQuestion.Id.Value, selectedQuestion);
                                        consumedValue += lengthValue;
                                        tryingAttemptInARowWithoutResult = 0;
                                    }
                                    else
                                    {
                                        sourceList.Return(selectedQuestion);
                                        tryingAttemptInARowWithoutResult++;
                                    }
                                }
                                else if (consumedValue > Math.Round(expectedLength))
                                {
                                    sourceList.Return(selectedQuestion);
                                    stoppedByAlgorithm = true;
                                }
                                else
                                {
                                    throw new InvalidOperationException("Impossible condition. This is an internal error.");
                                }
                                break;
                            case Models.Enums.TrainingLengthType.QuestionsCount:
                                selectedQuestions.Add(selectedQuestion.Id.Value, selectedQuestion);
                                consumedValue++;
                                break;
                            default:
                                throw new InvalidOperationException("Unknown length type.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Question with the same ID found the second time.");
                    }
#pragma warning restore CA1864 // Prefer the 'IDictionary.TryAdd(TKey, TValue)' method
                }
                while (!sourceList.Consumed &&
                    ((lengthType == Models.Enums.TrainingLengthType.QuestionsCount && consumedValue < Math.Round(expectedLength)) ||
                    (lengthType == Models.Enums.TrainingLengthType.Time && consumedValue != Math.Round(expectedLength) && tryingAttemptInARowWithoutResult < MaxCountOfTryingAttemptInARowWithoutResult && !stoppedByAlgorithm)));
            }
            resultLength = consumedValue;
            return selectedQuestions;
        }
    }
}
