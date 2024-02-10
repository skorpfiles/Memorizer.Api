using Bogus;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Tests.DataSources
{
    public static class UtilsUnitTestDataSource
    {
        static UtilsUnitTestDataSource()
        {
            Randomizer.Seed = new Random(44472135);
        }

        public static IEnumerable<object[]> FindBestQuestionsTimesCombination_CorrectParameters_CorrectResult
        {
            get
            {
                var generalFaker = new Faker();
                yield return new object[]
                {
                    new List<Question>(),
                    0,
                    new List<Question>()
                };

                yield return new object[]
                {
                    new List<Question>(),
                    generalFaker.Random.Number(1,int.MaxValue),
                    new List<Question>()
                };
            }
        }
    }
}
