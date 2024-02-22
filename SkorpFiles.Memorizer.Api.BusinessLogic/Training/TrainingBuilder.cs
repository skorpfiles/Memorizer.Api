using Azure.Core;
using Microsoft.Extensions.Options;
using SkorpFiles.Memorizer.Api.BusinessLogic.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class TrainingBuilder
    {
        private Random _random = new();

        public List<Question> BasicQuestionsList { get; set; } = new List<Question>();
        public EntitiesListForRandomChoice<Question> NewQuestionsList { get; set; } = new EntitiesListForRandomChoice<Question>();
        public EntitiesListForRandomChoice<Question> PrioritizedPenaltyQuestionsList { get; set; } = new EntitiesListForRandomChoice<Question>();

        public TrainingBuilder(IEnumerable<Question> initialQuestionsList)
        {
            FillQuestionsListsInitially(initialQuestionsList);
        }

        private void FillQuestionsListsInitially(IEnumerable<Question> initialQuestionsList)
        {
            IEnumerator<Api.Models.Question> questionsEnumerator = initialQuestionsList.GetEnumerator();
            while (questionsEnumerator.MoveNext())
            {
                var currentQuestion = questionsEnumerator.Current;

                if (currentQuestion.IsEnabled)
                {
                    if (currentQuestion.MyStatus == null || currentQuestion.MyStatus.IsNew)
                        NewQuestionsList.Add(currentQuestion);
                    else
                    {
                        BasicQuestionsList.Add(currentQuestion);
                        if (currentQuestion.MyStatus.PenaltyPoints > 0)
                            PrioritizedPenaltyQuestionsList.Add(currentQuestion);
                    }
                }
            }
        }

        public List<Question> MakeQuestionsListForTraining(TrainingOptions options)
        {
            if (options.NewQuestionsFraction < 0 || options.PrioritizedPenaltyQuestionsFraction < 0)
                throw new IncorrectTrainingOptionsException(Constants.NegativeFractionsMessage);

            if (options.NewQuestionsFraction + options.PrioritizedPenaltyQuestionsFraction > 1)
                throw new IncorrectTrainingOptionsException(Constants.SumOfFractionsCannotBeMoreThan1Message);

            if (options.LengthValue <= 0)
                throw new IncorrectTrainingOptionsException(Constants.NonPositiveLengthValueMessage);

            List<Question> result = new();

            double expectedLengthForNewQuestionList = options.LengthValue * options.NewQuestionsFraction;
            double expectedLengthForPrioritizedPenaltyQuestionsList = options.LengthValue * options.PrioritizedPenaltyQuestionsFraction;

            //create new questions list
            result.AddRange(GetSelectedQuestionsFromGeneralList(NewQuestionsList, options.LengthType, expectedLengthForNewQuestionList, _random, out int resultNewLength).Values);
            //create penalty questions list
            var selectedPenaltyQuestions = GetSelectedQuestionsFromGeneralList(PrioritizedPenaltyQuestionsList, options.LengthType, expectedLengthForPrioritizedPenaltyQuestionsList, _random, out int resultPenaltyLength);
            BasicQuestionsList.RemoveAll(q => selectedPenaltyQuestions.ContainsKey(q.Id!.Value));
            result.AddRange(selectedPenaltyQuestions.Values);

            //basic list
            int expectedLengthForBasicQuestionList = options.LengthValue - resultNewLength - resultPenaltyLength;
            if (expectedLengthForBasicQuestionList > 0)
            {
                RatingTape ratingTape = InitializeRatingTape(BasicQuestionsList, result);
                Dictionary<Guid, Question> questionsSelectedFromRatingTape = GetSelectedQuestionsFromGeneralList(ratingTape, options.LengthType, expectedLengthForBasicQuestionList, _random, out int resultBasicLength);
                BasicQuestionsList.RemoveAll(q => questionsSelectedFromRatingTape.ContainsKey(q.Id!.Value));
                result.AddRange(questionsSelectedFromRatingTape.Values);

                //if there is lack of questions, add from new ones
                int expectedLengthForAdditionalNewQuestionList = expectedLengthForBasicQuestionList - resultBasicLength;
                if (expectedLengthForAdditionalNewQuestionList > expectedLengthForAdditionalNewQuestionList * Constants.AllowableErrorFraction)
                {
                    result.AddRange(GetSelectedQuestionsFromGeneralList(NewQuestionsList, options.LengthType, expectedLengthForAdditionalNewQuestionList, _random, out int resultAdditionalNewLength).Values);

                    //if there is lack of questions after all selections, search for questions to fill 
                    if (options.LengthType == Models.Enums.TrainingLengthType.Time && options.LengthValue - options.LengthValue * Constants.AllowableErrorFraction > resultNewLength+resultPenaltyLength+resultBasicLength+resultAdditionalNewLength)
                    {
                        List<Question> remainingQuestions = new();
                        remainingQuestions.AddRange(NewQuestionsList.ToList());
                        remainingQuestions.AddRange(BasicQuestionsList.ToList());

                        result.AddRange(Utils.FindBestQuestionsTimesCombination(remainingQuestions, options.LengthValue));

                        //if the list is empty, add one the cheapest question
                        if (!result.Any() && remainingQuestions.Any())
                        {
                            result.Add(GetQuestionWithMinimalCost(remainingQuestions)!);
                        }
                    }
                }
            }

            

            return result;
        }

        private static Question? GetQuestionWithMinimalCost(IEnumerable<Question> questions)
        {
            Question? result = null;
            int currentMinimum = -1;
            foreach (Question question in questions)
            {
                if (question.FullEstimatedTrainingTimeSeconds() < currentMinimum || currentMinimum == -1)
                {
                    currentMinimum = question.FullEstimatedTrainingTimeSeconds();
                    result = question;
                }
            }
            return result;
        }

        private static Dictionary<Guid, Question> GetSelectedQuestionsFromGeneralList(IPickable<Question> sourceList, Models.Enums.TrainingLengthType lengthType, double expectedLength, Random random, out int resultLength)
        {
            Dictionary<Guid, Question> selectedQuestions = new();

            int consumedValue = 0;
            int tryingAttemptInARowWithoutResult = 0;
            const int MaxCountOfTryingAttemptInARowWithoutResult = 100;
            bool stoppedByAlgorithm = false;

            if (Math.Round(expectedLength) > 0 && !sourceList.Consumed)
            {
                Question? selectedQuestion;
                do
                {
                    selectedQuestion = sourceList.PickAndDelete(random);
                    if (selectedQuestion?.Id == null)
                    {
                        throw new InvalidOperationException("Question cannot have null ID while creating training list.");
                    }

                    if (!selectedQuestions.ContainsKey(selectedQuestion.Id.Value))
                    {
                        switch (lengthType)
                        {
                            case Models.Enums.TrainingLengthType.Time:
                                int lengthValue = selectedQuestion.FullEstimatedTrainingTimeSeconds();

                                if (consumedValue <= Math.Round(expectedLength))
                                {
                                    if (consumedValue+lengthValue <= Math.Round(expectedLength + expectedLength * Constants.AllowableErrorFraction))
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
                }
                while (!sourceList.Consumed &&
                    ((lengthType == Models.Enums.TrainingLengthType.QuestionsCount && consumedValue < Math.Round(expectedLength)) ||
                    (lengthType == Models.Enums.TrainingLengthType.Time && consumedValue != Math.Round(expectedLength) && tryingAttemptInARowWithoutResult < MaxCountOfTryingAttemptInARowWithoutResult && !stoppedByAlgorithm)));
            }
            resultLength = consumedValue;
            return selectedQuestions;
        }

        private static RatingTape InitializeRatingTape(List<Question> basicList, IEnumerable<Question> questionsToFilter)
        {
            RatingTape result = new();
            List<Question> questionsToRemoveFromBasicList = new List<Question>();

            foreach(Question question in basicList)
            {
                if (!questionsToFilter.Any(q => q.Id == question.Id))
                {
                    result.Add(question);
                    questionsToRemoveFromBasicList.Add(question);
                }
            }
            
            return result;
        }
    }
}
