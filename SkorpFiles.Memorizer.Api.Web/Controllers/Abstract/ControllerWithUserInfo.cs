using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Web.Exceptions;

namespace SkorpFiles.Memorizer.Api.Web.Controllers.Abstract
{
    public abstract class ControllerWithUserInfo : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;

        protected internal ControllerWithUserInfo(UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore)
        {
            _userManager = userManager;
            _userStore = userStore;
        }

        protected internal async Task<IdentityUser> GetCurrentUserAsync()
        {
            var user = await _userStore.FindByNameAsync(User.Identity!.Name, new CancellationToken());
            if (user != null)
                return user;
            else
                throw new IncorrectUserException("Unable to find the user by name.");
        }

        protected internal async Task<string> GetCurrentUserIdAsync()
        {
            var user = await GetCurrentUserAsync();
            var userId = await _userManager.GetUserIdAsync(user);
            if (userId != null)
                return userId;
            else
                throw new IncorrectUserException("Unable to get user ID.");
        }

        protected internal async Task<Guid> GetCurrentUserGuidAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            if (Guid.TryParse(userId, out var userGuid))
                return userGuid;
            else
                throw new IncorrectUserException("Unable to convert the user ID to GUID.");
        }
    }
}
