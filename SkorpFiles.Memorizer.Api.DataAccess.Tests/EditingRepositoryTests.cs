using Autofac;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;
using SkorpFiles.Memorizer.Api.DataAccess.Tests.DataSources;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    public class EditingRepositoryTests:IntegrationTestsBase
    {
        private IEditingRepository _editingRepository;

        public EditingRepositoryTests() : base()
        {
            _editingRepository = Container.Resolve<IEditingRepository>();
        }

        [TestMethod]
        [DynamicData(nameof(EditingRepositoryTestDataSource.GetQuestionnaireAsync_CorrectData_ReturnResultAsync), typeof(EditingRepositoryTestDataSource))]
        public async Task GetQuestionnaireAsync_CorrectData_ReturnResultAsync(Questionnaire expectedQuestionnaire)
        {
            DbContext.Questionnaires.Add(expectedQuestionnaire);
            await DbContext.SaveChangesAsync();

            Guid userId = RegisterUser();

            var actualQuestionnaire = await _editingRepository.GetQuestionnaireAsync(userId, expectedQuestionnaire.QuestionnaireId);

            actualQuestionnaire.Should().BeEquivalentTo(expectedQuestionnaire);
        }
    }
}