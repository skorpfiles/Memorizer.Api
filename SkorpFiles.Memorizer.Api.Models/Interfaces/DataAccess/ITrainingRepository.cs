﻿namespace SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<Question>> GetQuestionsForTrainingAsync(Guid userId, IEnumerable<Guid> questionnairesIds);
        Task UpdateQuestionStatusAsync(UserQuestionStatus questionStatus);
    }
}
