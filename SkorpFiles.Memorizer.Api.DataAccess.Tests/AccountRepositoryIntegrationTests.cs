using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Exceptions;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using SkorpFiles.Memorizer.Api.DataAccess.Repositories;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    [TestClass]
    [TestCategory(TestCategories.Integration)]
    public class AccountRepositoryIntegrationTests : IntegrationTestsBase
    {
        private AccountRepository CreateRepository() => new(DbContext);

        [TestMethod]
        public async Task RegisterUserActivityAsync_ExistingUser_CreatesActivityRecord()
        {
            // Arrange - the script seeds the users in AspNetUsers but not a fresh activity row
            // for a brand-new registration, so register activity for an existing user id.
            var userId = ScriptData.Bob.ToAspNetUserIdString()!;

            // Remove any activity the script created for this user, to test creation cleanly.
            var existing = await DbContext.UserActivities.Where(a => a.UserId == userId).ToListAsync();
            DbContext.UserActivities.RemoveRange(existing);
            await DbContext.SaveChangesAsync();

            var repository = CreateRepository();

            // Act
            await repository.RegisterUserActivityAsync("bob", userId);

            // Assert
            var activity = await FreshContext().UserActivities.SingleAsync(a => a.UserId == userId);
            activity.UserName.Should().Be("bob");
            activity.UserIsEnabled.Should().BeTrue();
            activity.ObjectIsRemoved.Should().BeFalse();
        }

        [TestMethod]
        public async Task RegisterUserActivityAsync_UnknownUser_Throws()
        {
            var repository = CreateRepository();

            var act = async () => await repository.RegisterUserActivityAsync("ghost", Guid.NewGuid().ToString());

            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [TestMethod]
        public async Task SetTokenToCacheAsync_NewKey_StoresValue()
        {
            var repository = CreateRepository();

            await repository.SetTokenToCacheAsync("token-key", "token-value");

            var cached = await FreshContext().AuthenticationCache.SingleAsync(c => c.Key == "token-key");
            cached.Value.Should().Be("token-value");
        }

        [TestMethod]
        public async Task SetTokenToCacheAsync_ExistingKey_OverwritesValue()
        {
            var repository = CreateRepository();

            await repository.SetTokenToCacheAsync("token-key", "first");
            await repository.SetTokenToCacheAsync("token-key", "second");

            var cached = await FreshContext().AuthenticationCache.Where(c => c.Key == "token-key").ToListAsync();
            cached.Should().ContainSingle();
            cached[0].Value.Should().Be("second");
        }

        [TestMethod]
        public async Task GetTokenInfoFromCacheAsync_ExistingKey_ReturnsValue()
        {
            var repository = CreateRepository();
            await repository.SetTokenToCacheAsync("token-key", "token-value");

            var value = await repository.GetTokenInfoFromCacheAsync("token-key");

            value.Should().Be("token-value");
        }

        [TestMethod]
        public async Task GetTokenInfoFromCacheAsync_UnknownKey_ReturnsNull()
        {
            var repository = CreateRepository();

            var value = await repository.GetTokenInfoFromCacheAsync("missing-key");

            value.Should().BeNull();
        }
    }
}
