using SkorpFiles.Memorizer.Api.Models;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class RatingTape:IPickable<GetQuestionsForTrainingResult>
    {
        private int _nextCoordinate = 1;

        private readonly Func<int, int> _ratingToWeightConversion;

        public SortedDictionary<int, RatingComponent> CoordinatesAndComponents { get; private set; } = [];
        public SortedSet<int> Coordinates { get; private set; } = [];
        public Dictionary<int, RatingComponent> RatingsAndComponents { get; private set; } = [];

        public int Length => _nextCoordinate - 1;

        public bool Consumed => Length == 0;

        public RatingTape()
        {
            _ratingToWeightConversion = (rating) => rating;
        }

        public RatingTape(Func<int, int> ratingToWeightConversion)
        {
            _ratingToWeightConversion = ratingToWeightConversion;
        }

        public void Add(GetQuestionsForTrainingResult question)
        {
            int rating = question.QuestionUserRating ?? Constants.InitialQuestionRating;

            if (RatingsAndComponents.TryGetValue(rating, out RatingComponent? ratingComponent))
            {
                ratingComponent.Add(question);
            }
            else
            {
                ratingComponent = CreateRatingComponent(rating);
                ratingComponent.Add(question);
            }
        }

        public void AddRange(IEnumerable<GetQuestionsForTrainingResult> items)
        {
            foreach(GetQuestionsForTrainingResult item in items)
            {
                Add(item);
            }
        }

        public void Delete(Guid questionId, int rating)
        {
            if (CoordinatesAndComponents.TryGetValue(rating, out RatingComponent? ratingComponent))
            {
                ratingComponent.Delete(questionId);
                if (ratingComponent.Consumed)
                    DeleteRatingComponent(ratingComponent);
            }
            else
                throw new InvalidOperationException("No rating component with such rating.");
        }

        public GetQuestionsForTrainingResult Pick(Random random)
        {
            RatingComponent foundRatingComponent = PickRatingComponent(random);
            return foundRatingComponent.Pick(random);
        }

        public GetQuestionsForTrainingResult PickAndDelete(Random random)
        {
            RatingComponent foundRatingComponent = PickRatingComponent(random);
            GetQuestionsForTrainingResult foundQuestion = foundRatingComponent.PickAndDelete(random);
            if (foundRatingComponent.Consumed)
                DeleteRatingComponent(foundRatingComponent);
            return foundQuestion;
        }

        public bool Return(GetQuestionsForTrainingResult question)
        {
            bool result;

            int rating = question.QuestionUserRating ?? Constants.InitialQuestionRating;

            if (RatingsAndComponents.TryGetValue(rating, out RatingComponent? ratingComponent))
            {
                result = ratingComponent.Return(question);
            }
            else
            {
                ratingComponent = CreateRatingComponent(rating);
                result = ratingComponent.Return(question);
            }
            return result;
        }

        private RatingComponent PickRatingComponent(Random random)
        {
            int coordinateForPick = random.Next(1, Length);
            int coordinateOfFoundElement = Coordinates.GetViewBetween(0, coordinateForPick).Max();
            return CoordinatesAndComponents[coordinateOfFoundElement];
        }

        private RatingComponent CreateRatingComponent(int rating)
        {
            int weight = _ratingToWeightConversion(rating);
            RatingComponent ratingComponent = new(_nextCoordinate, weight);
            RatingsAndComponents.Add(rating, ratingComponent);
            Coordinates.Add(_nextCoordinate);
            CoordinatesAndComponents.Add(_nextCoordinate, ratingComponent);
            _nextCoordinate += weight;
            return ratingComponent;
        }

        private void DeleteRatingComponent(RatingComponent ratingComponent)
        {
            var coordinate = ratingComponent.Coordinate;
            var length = ratingComponent.Length;
            CoordinatesAndComponents.Remove(coordinate);
            Coordinates.Remove(coordinate);
            RatingsAndComponents.Remove(coordinate);
            MoveCoordinatesBack(coordinate, length);
        }

        private void MoveCoordinatesBack(int coordinateOfDeletedElement, int lengthOfDeletedElement)
        {
            if (_nextCoordinate - lengthOfDeletedElement > coordinateOfDeletedElement)
            {
                SortedDictionary<int, RatingComponent> newCoordinatesAndComponents = [];
                SortedSet<int> newCoordinates = [];
                foreach (var key in CoordinatesAndComponents.Keys)
                {
                    if (key == coordinateOfDeletedElement)
                        throw new InvalidOperationException("Before moving coordinates back, none element can have the coordinate of deleted element.");
                    else if (key > coordinateOfDeletedElement)
                    {
                        RatingComponent movedRatingComponent = CoordinatesAndComponents[key];
                        int newCoordinate = key - lengthOfDeletedElement;
                        if (newCoordinate < 0)
                            throw new InvalidOperationException("Error while moving coordinates back: cannot move to a negative coordinate.");
                        else
                        {
                            newCoordinatesAndComponents.Add(newCoordinate, movedRatingComponent);
                            movedRatingComponent.Coordinate = newCoordinate;
                            newCoordinates.Add(newCoordinate);
                        }
                    }
                    else
                    {
                        newCoordinatesAndComponents.Add(key, CoordinatesAndComponents[key]);
                        newCoordinates.Add(key);
                    }
                }
                CoordinatesAndComponents = newCoordinatesAndComponents;
                Coordinates = newCoordinates;
            }
            _nextCoordinate -= lengthOfDeletedElement;
        }
    }
}
