﻿using SkorpFiles.Memorizer.Api.BusinessLogic.Training;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;
using SkorpFiles.Memorizer.Api.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class TrainingLogic:ITrainingLogic
    {
        private readonly ITrainingRepository _trainingRepository;

        public TrainingLogic(ITrainingRepository trainingRepository)
        {
            _trainingRepository = trainingRepository;
        }

        public async Task<IEnumerable<Api.Models.Question>> SelectQuestionsForTrainingAsync(Guid userId, Guid trainingId, TrainingOptions options)
        {
            if (options.NewQuestionsFraction < 0 || options.PrioritizedPenaltyQuestionsFraction < 0)
                throw new IncorrectTrainingOptionsException(Constants.NegativeFractionsMessage);

            if (options.NewQuestionsFraction + options.PrioritizedPenaltyQuestionsFraction > 1)
                throw new IncorrectTrainingOptionsException(Constants.SumOfFractionsCannotBeMoreThan1Message);

            if (options.LengthValue <= 0)
                throw new IncorrectTrainingOptionsException(Constants.NonPositiveLengthValueMessage);


            var allQuestions = (await _trainingRepository.GetQuestionsForTrainingAsync(userId, trainingId)).ToList();
            var questionsListsCollection = new TrainingBuilder(allQuestions);
            var questionsList = questionsListsCollection.MakeQuestionsListForTraining(options);

            return questionsList;
        }
    }
}