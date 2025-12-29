using AutoMapper;
using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies.Strategy2018;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Abstract;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training.MakingListStrategies
{
    internal abstract class CapacityRestrictedMakingListStrategy<TNewQuestionsList, TPenaltyQuestionsList> : IMakingListStrategy 
        where TNewQuestionsList : IPickableTrainingList<GetQuestionsForTrainingResult>, IEditableListWithId<GetQuestionsForTrainingResult, Guid>, IEnumerable<GetQuestionsForTrainingResult>, new()
        where TPenaltyQuestionsList : IPickableTrainingList<GetQuestionsForTrainingResult>, IEditableListWithId<GetQuestionsForTrainingResult, Guid>, new()
    {
        protected internal readonly Random Random = new();
        public List<GetQuestionsForTrainingResult> BasicQuestionsList { get; set; } = [];
        public TNewQuestionsList NewQuestionsList { get; set; } = new TNewQuestionsList();
        public TPenaltyQuestionsList PrioritizedPenaltyQuestionsList { get; set; } = new TPenaltyQuestionsList();

        abstract internal IPickableTrainingList<GetQuestionsForTrainingResult> GetPickerForBasicList(IEnumerable<Entity> entitiesHaveBeenAlreadyChosen);

        public CapacityRestrictedMakingListStrategy(IEnumerable<GetQuestionsForTrainingResult> initialQuestionsList)
        {
            FillQuestionsListsInitially(initialQuestionsList);
        }

        private void FillQuestionsListsInitially(IEnumerable<GetQuestionsForTrainingResult> initialQuestionsList)
        {
            //todo optimize so that the questions are not copied to the new lists
            IEnumerator<GetQuestionsForTrainingResult> questionsEnumerator = initialQuestionsList.GetEnumerator();
            while (questionsEnumerator.MoveNext())
            {
                var currentQuestion = questionsEnumerator.Current;

                if (currentQuestion != null && currentQuestion.QuestionIsEnabled)
                {
                    if (currentQuestion.QuestionUserIsNew == null || currentQuestion.QuestionUserIsNew.Value)
                        NewQuestionsList.Add(currentQuestion);
                    else
                    {
                        BasicQuestionsList.Add(currentQuestion);
                        if (currentQuestion.QuestionUserPenaltyPoints > 0)
                            PrioritizedPenaltyQuestionsList.Add(currentQuestion);
                    }
                }
            }
        }

        public List<ExistingQuestion> MakeQuestionsListForTraining(TrainingOptions options, IMapper mapper)
        {
            if (options.NewQuestionsFraction < 0 || options.PrioritizedPenaltyQuestionsFraction < 0)
                throw new IncorrectTrainingOptionsException(Constants.NegativeFractionsMessage);

            if (options.NewQuestionsFraction + options.PrioritizedPenaltyQuestionsFraction > 1)
                throw new IncorrectTrainingOptionsException(Constants.SumOfFractionsCannotBeMoreThan1Message);

            if (options.LengthValue <= 0)
                throw new IncorrectTrainingOptionsException(Constants.NonPositiveLengthValueMessage);

            List<ExistingQuestion> result = [];

            double expectedLengthForNewQuestionList = options.LengthValue * options.NewQuestionsFraction;
            double expectedLengthForPrioritizedPenaltyQuestionsList = options.LengthValue * options.PrioritizedPenaltyQuestionsFraction;

            //create new questions list
            result.AddRange(mapper.Map<List<ExistingQuestion>>(GetSelectedQuestionsFromGeneralList(NewQuestionsList, options.LengthType, expectedLengthForNewQuestionList, Random, out int resultNewLength).Values));
            //create penalty questions list
            var selectedPenaltyQuestions = GetSelectedQuestionsFromGeneralList(PrioritizedPenaltyQuestionsList, options.LengthType, expectedLengthForPrioritizedPenaltyQuestionsList, Random, out int resultPenaltyLength);
            BasicQuestionsList.RemoveAll(q => selectedPenaltyQuestions.ContainsKey(q.Id!.Value));
            result.AddRange(mapper.Map<List<ExistingQuestion>>(selectedPenaltyQuestions.Values));

            //basic list
            int expectedLengthForBasicQuestionList = options.LengthValue - resultNewLength - resultPenaltyLength;
            if (expectedLengthForBasicQuestionList > 0)
            {
                var pickerForBasicList = GetPickerForBasicList(result);

                Dictionary<Guid, GetQuestionsForTrainingResult> questionsSelectedFromRatingTape = GetSelectedQuestionsFromGeneralList(pickerForBasicList, options.LengthType, expectedLengthForBasicQuestionList, Random, out int resultBasicLength);
                BasicQuestionsList.RemoveAll(q => questionsSelectedFromRatingTape.ContainsKey(q.Id!.Value));
                result.AddRange(mapper.Map<List<ExistingQuestion>>(questionsSelectedFromRatingTape.Values));

                //if there is lack of questions, add from new ones
                int expectedLengthForAdditionalNewQuestionList = expectedLengthForBasicQuestionList - resultBasicLength;
                if (expectedLengthForAdditionalNewQuestionList > expectedLengthForAdditionalNewQuestionList * Constants.AllowableErrorFraction)
                {
                    result.AddRange(mapper.Map<List<ExistingQuestion>>(GetSelectedQuestionsFromGeneralList(NewQuestionsList, options.LengthType, expectedLengthForAdditionalNewQuestionList, Random, out int resultAdditionalNewLength).Values));

                    //if there is lack of questions after all selections, search for questions to fill 
                    if (options.LengthType == Models.Enums.TrainingLengthType.Time && options.LengthValue - options.LengthValue * Constants.AllowableErrorFraction > resultNewLength + resultPenaltyLength + resultBasicLength + resultAdditionalNewLength)
                    {
                        List<GetQuestionsForTrainingResult> remainingQuestions = [.. NewQuestionsList.ToList(), .. BasicQuestionsList.ToList()];

                        result.AddRange(mapper.Map<List<ExistingQuestion>>(Utils.FindBestQuestionsTimesCombination(remainingQuestions, options.LengthValue)));

                        //if the list is empty, add one the cheapest question
                        if (result.Count == 0 && remainingQuestions.Count != 0)
                        {
                            result.Add(mapper.Map<ExistingQuestion>(GetQuestionWithMinimalCost(remainingQuestions))!);
                        }
                    }
                }
            }

            return result;
        }

        private static GetQuestionsForTrainingResult? GetQuestionWithMinimalCost(IEnumerable<GetQuestionsForTrainingResult> questions)
        {
            GetQuestionsForTrainingResult? result = null;
            int currentMinimum = -1;
            foreach (GetQuestionsForTrainingResult question in questions)
            {
                if (Math.Round((double)question.FullActualTrainingTimeSeconds()) < currentMinimum || currentMinimum == -1)
                {
                    currentMinimum = question.FullActualTrainingTimeSeconds();
                    result = question;
                }
            }
            return result;
        }

        private static Dictionary<Guid, GetQuestionsForTrainingResult> GetSelectedQuestionsFromGeneralList(IPickableTrainingList<GetQuestionsForTrainingResult> sourceList, Models.Enums.TrainingLengthType lengthType, double expectedLength, Random random, out int resultLength)
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
