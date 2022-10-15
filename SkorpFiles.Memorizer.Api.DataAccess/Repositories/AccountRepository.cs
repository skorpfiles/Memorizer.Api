﻿using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.Models.Interfaces.DataAccess;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class AccountRepository: RepositoryBase, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task RegisterUserActivityAsync(string userName, string userId)
        {
            var userActivity = new UserActivity(userName, userId)
            {
                UserIsEnabled = true,
                ObjectCreationTimeUtc = DateTime.UtcNow
            };
            DbContext.UserActivities?.Add(userActivity);
            await DbContext.SaveChangesAsync(); //todo one transaction with creating user in AspNetUsers
        }
    }
}
