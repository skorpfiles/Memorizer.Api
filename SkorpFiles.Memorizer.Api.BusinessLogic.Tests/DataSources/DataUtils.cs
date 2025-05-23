﻿using Bogus;
using Bogus.Extensions;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    internal class DataUtils
    {
        public static Faker<UserQuestionStatus> GetUserQuestionStatusFaker(Faker generalFaker)
        {
            return new Faker<UserQuestionStatus>()
                    .RuleSet("default", rs =>
                    {
                        rs.RuleFor(uqs => uqs.AverageTrainingTimeSeconds, f => f.Random.Number(Restrictions.QuestionTrainingTimeSecondsMinValue, Restrictions.QuestionTrainingTimeSecondsMaxValue));
                    })
                    .RuleSet("new", rs =>
                    {
                        rs.RuleFor(uqs => uqs.PenaltyPoints, 0);
                        rs.RuleFor(uqs => uqs.IsNew, true);
                        rs.RuleFor(uqs => uqs.Rating, Restrictions.InitialQuestionRating);
                    })
                    .RuleSet("penalty", rs =>
                    {
                        rs.RuleFor(uqs => uqs.PenaltyPoints, f => generalFaker.Random.Number(min: 1));
                        rs.RuleFor(uqs => uqs.IsNew, false);
                        rs.RuleFor(uqs => uqs.Rating, Restrictions.InitialQuestionRating);
                    })
                    .RuleSet("usual", rs =>
                    {
                        rs.RuleFor(uqs => uqs.PenaltyPoints, 0);
                        rs.RuleFor(uqs => uqs.IsNew, false);
                        rs.RuleFor(uqs => uqs.Rating, f => f.Random.Number(Restrictions.MinQuestionRating, Restrictions.MaxQuestionRating));
                    });
        }

        public static Faker<GetQuestionsForTrainingResult> GetQuestionFaker(Faker generalFaker)
        {
            return new Faker<GetQuestionsForTrainingResult>()
                    .RuleSet("default", rs =>
                    {
                        rs.RuleFor(q => q.Id, f => f.Random.Guid());
                        rs.RuleFor(q => q.QuestionType, f => f.PickRandom<QuestionType>());
                        rs.RuleFor(q => q.QuestionText, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionTextMaxLength));
                        rs.RuleFor(q => q.QuestionIsEnabled, true);
                        rs.RuleFor(q => q.QuestionEstimatedTrainingTimeSeconds, f => f.Random.Number(Restrictions.QuestionTrainingTimeSecondsMinValue, Restrictions.QuestionTrainingTimeSecondsMaxValue));
                        rs.RuleFor(q => q.QuestionnaireId, f => f.Random.Guid());
                        rs.RuleFor(q => q.CreationTimeUtc, DateTime.UtcNow);
                        rs.RuleFor(q => q.QuestionUntypedAnswer, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionUntypedAnswerMaxLength));
                        rs.RuleFor(q => q.QuestionActualTrainingTimeSeconds, f => f.Random.Number(Restrictions.QuestionTrainingTimeSecondsMinValue, Restrictions.QuestionTrainingTimeSecondsMaxValue));
                    })
                    .RuleSet("new", rs =>
                    {
                        rs.RuleFor(q => q.QuestionUserPenaltyPoints, 0);
                        rs.RuleFor(q => q.QuestionUserIsNew, true);
                        rs.RuleFor(q => q.QuestionUserRating, Restrictions.InitialQuestionRating);
                    })
                    .RuleSet("penalty", rs =>
                    {
                        rs.RuleFor(q => q.QuestionUserPenaltyPoints, f => generalFaker.Random.Number(min: 1));
                        rs.RuleFor(q => q.QuestionUserIsNew, false);
                        rs.RuleFor(q => q.QuestionUserRating, Restrictions.InitialQuestionRating);
                    })
                    .RuleSet("usual", rs =>
                    {
                        rs.RuleFor(q => q.QuestionUserPenaltyPoints, 0);
                        rs.RuleFor(q => q.QuestionUserIsNew, false);
                        rs.RuleFor(q => q.QuestionUserRating, f => f.Random.Number(Restrictions.MinQuestionRating, Restrictions.MaxQuestionRating));
                    });
        }

        public static Faker<ExistingQuestion> GetQuestionFaker(Faker<UserQuestionStatus> userQuestionStatusFaker)
        {
            return new Faker<ExistingQuestion>()
                    .RuleSet("default", rs =>
                    {
                        rs.RuleFor(q => q.Id, f => f.Random.Guid());
                        rs.RuleFor(q => q.Type, f => f.PickRandom<QuestionType>());
                        rs.RuleFor(q => q.Text, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionTextMaxLength));
                        rs.RuleFor(q => q.IsEnabled, true);
                        rs.RuleFor(q => q.EstimatedTrainingTimeSeconds, f => f.Random.Number(Restrictions.QuestionTrainingTimeSecondsMinValue, Restrictions.QuestionTrainingTimeSecondsMaxValue));
                        rs.RuleFor(q => q.QuestionnaireId, f => f.Random.Guid());
                        rs.RuleFor(q => q.CodeInQuestionnaire, f => f.IndexFaker);
                        rs.RuleFor(q => q.CreationTimeUtc, DateTime.UtcNow);
                        rs.RuleFor(q => q.UntypedAnswer, f => f.Lorem.Text().ClampLength(1, Restrictions.QuestionUntypedAnswerMaxLength));
                    })
                    .RuleSet("new", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, f => userQuestionStatusFaker.Generate("default,new"));
                    })
                    .RuleSet("penalty", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, f => userQuestionStatusFaker.Generate("default,penalty"));
                    })
                    .RuleSet("usual", rs =>
                    {
                        rs.RuleFor(q => q.MyStatus, f => userQuestionStatusFaker.Generate("default,usual"));
                    });
        }

        public static void Shuffle<T>(List<T> list, Faker generalFaker)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = generalFaker.Random.Number(0, n);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
