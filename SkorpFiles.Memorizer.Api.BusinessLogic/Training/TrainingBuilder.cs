using Microsoft.Extensions.Options;
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
            double expectedLengthForBasicQuestionList = options.LengthValue - expectedLengthForNewQuestionList - expectedLengthForPrioritizedPenaltyQuestionsList;

            //create new questions list
            result.AddRange(GetSelectedQuestionsFromGeneralList(NewQuestionsList, options.LengthType, expectedLengthForNewQuestionList, _random).Values);
            //create penalty questions list
            result.AddRange(GetSelectedQuestionsFromGeneralList(PrioritizedPenaltyQuestionsList, options.LengthType, expectedLengthForPrioritizedPenaltyQuestionsList, _random).Values);

            //basic list
            RatingTape ratingTape = InitializeRatingTape(BasicQuestionsList, result);
            result.AddRange(GetSelectedQuestionsFromGeneralList(ratingTape, options.LengthType, expectedLengthForBasicQuestionList, _random).Values);

            return result;
        }

        private static Dictionary<Guid, Question> GetSelectedQuestionsFromGeneralList(IPickable<Question> sourceList, Models.Enums.TrainingLengthType lengthType, double expectedLength, Random random)
        {
            Dictionary<Guid, Question> selectedQuestions = new();

            double consumedValue = 0;
            int tryingAttemptInARowWithoutResult = 0;
            const int MaxCountOfTryingAttemptInARowWithoutResult = 100;

            if (expectedLength > 0 && !sourceList.Consumed)
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
                                int lengthValue = selectedQuestion.EstimatedTrainingTimeSeconds;

                                if (Math.Abs(consumedValue + lengthValue - expectedLength) > expectedLength * (1 - Constants.AllowableErrorFraction))
                                {
                                    selectedQuestions.Add(selectedQuestion.Id.Value, selectedQuestion);
                                    consumedValue += lengthValue;
                                    tryingAttemptInARowWithoutResult = 0;
                                }
                                else
                                {
                                    tryingAttemptInARowWithoutResult++;
                                }
                                break;
                            case Models.Enums.TrainingLengthType.QuestionsCount:
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
                while (!sourceList.Consumed && (consumedValue < expectedLength || tryingAttemptInARowWithoutResult >= MaxCountOfTryingAttemptInARowWithoutResult));
            }
            return selectedQuestions;
        }

        private static RatingTape InitializeRatingTape(IEnumerable<Question> basicList, IEnumerable<Question> questionsToFilter)
        {
            RatingTape result = new();

            foreach(Question question in basicList)
            {
                if (!questionsToFilter.Any(q => q.Id == question.Id))
                    result.Add(question);
            }
            return result;
        }
    }
}
