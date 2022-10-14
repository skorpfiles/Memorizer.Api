using SkorpFiles.Memorizer.Api.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Interfaces.DataAccess;

namespace SkorpFiles.Memorizer.Api.BusinessLogic
{
    public class AccountLogic : IAccountLogic
    {
        private readonly IAccountRepository _accountRepository;

        public AccountLogic(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task RegisterUserActivityAsync(string userName, string userId)
        {
            await _accountRepository.RegisterUserActivityAsync(userName, userId);
        }
    }
}
