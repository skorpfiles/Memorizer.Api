using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class TrainingLogicTestDataSource
    {
        static TrainingLogicTestDataSource()
        {
            Randomizer.Seed = new Random(44472134);
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_IncorrectOptions_IncorrectTrainingOptionsException
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(0.01, 1),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,1),
                    faker.Random.Double(-1, -0.01),
                    faker.Random.Int(min:1),
                    Constants.NegativeFractionsMessage
                };

                yield return new object[]
                {
                    faker.Random.Double(min:1.01),
                    faker.Random.Double(min:1.01),
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,0.99),
                    1,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    1,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                var term1 = faker.Random.Double(0.02, 0.5);
                var term2 = faker.Random.Double(1-term1+0.01, 0.99);

                yield return new object[]
                {
                    term1,
                    term2,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    term2,
                    term1,
                    faker.Random.Int(min:1),
                    Constants.SumOfFractionsCannotBeMoreThan1Message
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    0,
                    Constants.NonPositiveLengthValueMessage
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Int(max:1),
                    Constants.NonPositiveLengthValueMessage
                };
            }
        }

        public static IEnumerable<object[]> SelectQuestionsForTrainingAsync_CorrectOptions_NoExceptions
        {
            get
            {
                var faker = new Faker();

                yield return new object[]
                {
                    0,
                    0,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    0,
                    faker.Random.Double(0.01,0.99),
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,0.99),
                    0,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    0,
                    1,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    1,
                    0,
                    faker.Random.Int(min:1)
                };

                var term1 = faker.Random.Double(0.01, 0.98);
                var term2 = 1 - term1;

                yield return new object[]
                {
                    term1,
                    term2,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    term2,
                    term1,
                    faker.Random.Int(min:1)
                };

                yield return new object[]
                {
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Double(0.01,0.49),
                    faker.Random.Int(min:1)
                };
            }
        }
    }
}
