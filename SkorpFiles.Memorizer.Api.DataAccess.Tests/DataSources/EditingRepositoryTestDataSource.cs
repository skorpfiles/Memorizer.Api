using Bogus;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests.DataSources
{
    public static class EditingRepositoryTestDataSource
    {
        public static IEnumerable<object[]> GetQuestionnaireAsync_CorrectData_ReturnResultAsync
        {
            get
            {
                yield return new object[]
                {
                    new Faker<Questionnaire>()
                        .RuleFor(q=>q.QuestionnaireName,f=>f.Commerce.ProductName())
                        .Generate()
                };
            }
        }
    }
}
