using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Exceptions;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class AccountRepository: RepositoryBase, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task RegisterUserActivityAsync(string userName, string userId)
        {
            if (!DbContext.Users.Any(u => u.Id == userId))
                throw new UserNotFoundException(userId);

            var userActivity = new UserActivity(userName, userId)
            {
                UserIsEnabled = true,
                ObjectCreationTimeUtc = DateTime.UtcNow
            };
            DbContext.UserActivities?.Add(userActivity);
            await DbContext.SaveChangesAsync(); //todo one transaction with creating user in AspNetUsers
        }

        public async Task SetTokenToCacheAsync(string key, string value)
        {
            var existingKeyRecord = DbContext.AuthenticationCache.Where(k => k.Key == key).FirstOrDefault();
            if (existingKeyRecord == null)
            {
                var authenticationCacheRecord = new AuthenticationCache() { Key = key, Value = value };
                DbContext.AuthenticationCache?.Add(authenticationCacheRecord);
            }
            else
            {
                existingKeyRecord.Value = value;
            }    
            await DbContext.SaveChangesAsync();
        }

        public async Task<string?> GetTokenInfoFromCacheAsync(string key)
        {
            return (await DbContext.AuthenticationCache.Where(ac => ac.Key == key).FirstOrDefaultAsync())?.Value;
        }

    }
}
