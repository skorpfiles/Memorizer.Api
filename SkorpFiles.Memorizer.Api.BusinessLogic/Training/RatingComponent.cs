using Microsoft.EntityFrameworkCore.Metadata;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class RatingComponent
    {

        private readonly List<Guid> _existingQuestionsIds = new();
        private readonly Dictionary<Guid,Question> _existingQuestions = new();

        public int Coordinate { get; set; }
        public int Length { get; set; }

        public bool Consumed => !_existingQuestions.Any();

        public RatingComponent(int coordinate, int length) 
        { 
            Coordinate = coordinate;
            Length = length; 
        }

        public void Add(Question question)
        {
            if (question.Id != null)
            {
                _existingQuestions.Add(question.Id.Value, question);
                _existingQuestionsIds.Add(question.Id.Value);
            }
            else
                throw new InvalidOperationException("Unable to add null question ID to a Rating Component.");
        }

        public void Delete(Guid questionId)
        {
            _existingQuestionsIds.Remove(questionId);
        }

        public Question Pick(Random random)
        {
            var index = random.Next(_existingQuestionsIds.Count-1);
            return _existingQuestions[_existingQuestionsIds[index]];
        }

        public Question PickAndDelete(Random random)
        {
            var index = random.Next(_existingQuestionsIds.Count-1);
            var result = _existingQuestions[_existingQuestionsIds[index]];
            _existingQuestionsIds[index] = _existingQuestionsIds[^1];
            _existingQuestionsIds.RemoveAt(_existingQuestionsIds.Count - 1);
            return result;
        }
    }
}
