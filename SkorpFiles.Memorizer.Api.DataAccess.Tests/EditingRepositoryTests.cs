using Autofac;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public async Task GetQuestionnaireAsync_CorrectData_ReturnResultAsync(Questionnaire expectedQuestionnaireDb, int questionsCount)
        {
            //Arrange
            await RegisterUserAsync();

            DbContext.Questionnaires.Add(expectedQuestionnaireDb);
            await DbContext.SaveChangesAsync();

            var expectedQuestionnaireMapped = Mapper.Map<Api.Models.Questionnaire>(expectedQuestionnaireDb);

            //Act
            var actualQuestionnaire = await _editingRepository.GetQuestionnaireAsync(Constants.DefaultUserId, expectedQuestionnaireDb.QuestionnaireId);

            //Assert
            //checking to prevent lost fields
            actualQuestionnaire.Should().BeEquivalentTo(expectedQuestionnaireMapped, opts =>
            {
                opts.Excluding(t => t.Id);
                opts.Excluding(t => t.Code);
                return opts;
            });

            //checking fields without mapping
            actualQuestionnaire.Id.Should().NotBeNull();
            actualQuestionnaire.Code.Should().NotBeNull();
            actualQuestionnaire.Name.Should().Be(expectedQuestionnaireDb.QuestionnaireName);
            actualQuestionnaire.Availability.Should().Be(expectedQuestionnaireDb.QuestionnaireAvailability);
            actualQuestionnaire.OwnerId.Should().Be(expectedQuestionnaireDb.OwnerId);
            actualQuestionnaire.QuestionsCount.Should().Be(questionsCount);
            actualQuestionnaire.Labels.Should().BeEquivalentTo(expectedQuestionnaireDb.LabelsForQuestionnaire);
        }
    }
}