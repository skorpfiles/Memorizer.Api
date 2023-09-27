using SkorpFiles.Memorizer.Api.Models;
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
        private Random _random = new Random();

        public List<Question> BasicQuestionsList { get; set; } = new List<Question>();
        public List<Question> NewQuestionsList { get; set; } = new List<Question>();
        public List<Question> PrioritizedPenaltyQuestionsList { get; set; } = new List<Question>();

        public static TrainingBuilder Build(IEnumerable<Question> initialQuestionsList)
        {
            TrainingBuilder result = new TrainingBuilder();
            IEnumerator<Api.Models.Question> questionsEnumerator = initialQuestionsList.GetEnumerator();
            while (questionsEnumerator.MoveNext())
            {
                var currentQuestion = questionsEnumerator.Current;

                if (currentQuestion.IsEnabled)
                {
                    if (currentQuestion.MyStatus == null || currentQuestion.MyStatus.IsNew)
                        result.NewQuestionsList.Add(currentQuestion);
                    else
                    {
                        result.BasicQuestionsList.Add(currentQuestion);
                        if (currentQuestion.MyStatus.PenaltyPoints > 0)
                            result.PrioritizedPenaltyQuestionsList.Add(currentQuestion);
                    }
                }
            }
            return result;
        }

        public List<Question> MakeQuestionsListForTraining(TrainingOptions options)
        {
            const int SecondsInMinute = 60;
            List<Question> result = new List<Question>();

            double expectedLengthForBasicQuestionList;
            double expectedLengthForNewQuestionList = options.LengthValue * options.NewQuestionsFraction;
            double expectedLengthForPrioritizedPenaltyQuestionsList = options.LengthValue * options.PrioritizedPenaltyQuestionsFraction;

            double consumedValue = 0;

            Dictionary<Guid, Question> selectedNewQuestions = new Dictionary<Guid, Question>();
            if (expectedLengthForNewQuestionList > 0)
            {
                Question? selectedQuestion;
                do
                {
                    selectedQuestion = PickRandomElementFromList(NewQuestionsList);
                    if (selectedQuestion?.Id == null)
                    {
                        throw new InvalidOperationException("Question cannot have null ID while creating training list.");
                    }

                    if (!selectedNewQuestions.ContainsKey(selectedQuestion.Id.Value))
                    {
                        int lengthValue = 1;
                        if (options.LengthType == Models.Enums.TrainingLengthType.Time)
                        {
                            lengthValue = selectedQuestion.EstimatedTrainingTimeSeconds * SecondsInMinute;

                            if (consumedValue + lengthValue - expectedLengthForNewQuestionList > expectedLengthForNewQuestionList * (1 - Settings.AllowableErrorFraction))
                            {
                                selectedNewQuestions.Add(selectedQuestion.Id.Value, selectedQuestion);
                                consumedValue += lengthValue;
                            }
                            else
                            {

                            }

                        }
                    }
                    else
                        
                }
                while (consumedValue < expectedLengthForNewQuestionList);

                if (consumedValue - expectedLengthForNewQuestionList > expectedLengthForNewQuestionList * (1 - Settings.AllowableErrorFraction))
                {
                    selectedNewQuestions.Remove(selectedNewQuestions.Last());
                    
                    selectedQuestion = null;
                    DateTime startTime = DateTime.UtcNow;
                    int attemptsCounter = 0;
                    do
                    {
                        selectedQuestion = PickRandomElementFromList(NewQuestionsList);
                    }
                    while()
                }
            }


        }

        private T Find

        private T PickRandomElementFromList<T>(List<T> list)
        {
            return list[_random.Next(list.Count - 1)];
        }
    }
}
