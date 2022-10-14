using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Authorization;
using SkorpFiles.Memorizer.Api.DataAccess;
using SkorpFiles.Memorizer.Api.Enums;
using SkorpFiles.Memorizer.Api.Exceptions;
using SkorpFiles.Memorizer.Api.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Models.Requests.Authorization;
using SkorpFiles.Memorizer.Api.Models.Responses;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SkorpFiles.Memorizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountLogic _accountLogic;

        private readonly IConnectionMultiplexer _redis;

        public AccountController(IUserStore<IdentityUser> userStore, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            IConnectionMultiplexer redis, IConfiguration configuration, ApplicationDbContext dbContext, IAccountLogic accountLogic)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _redis = redis;
            _configuration = configuration;
            _dbContext = dbContext;
            _accountLogic = accountLogic;
        }

        [Route("Token")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            if (request.Login == null || request.Password == null)
                return BadRequest(new { errorText = "Login and password cannot be null." });

            var identity = await GetIdentityAsync(request.Login, request.Password);
            if (identity == null)
                return BadRequest(new { errorText = "Invalid login or password." });

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(_configuration), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            if (encodedJwt != null)
            {
                var redisDb = _redis.GetDatabase();
                var redisResult = await redisDb.StringSetAsync(new RedisKey(encodedJwt), new RedisValue(Constants.DefaultName));
                if (redisResult)
                {
                    return Json(new
                    {
                        AccessToken = encodedJwt,
                        Login = identity.Name
                    });
                }
                else
                    throw new InternalAuthenticationErrorException("Unable to cache authentication information.");
            }
            else
                throw new InternalAuthenticationErrorException("Unable to generate a JWT token.");
        }

        [Route("Register")]
        [HttpPut]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (request.Email == null || request.Password == null)
                return BadRequest(new { ErrorText = "Login and password cannot be null." });

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, request.Login ?? request.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);

            var userCreatingResult = await _userManager.CreateAsync(user, request.Password);

            if (userCreatingResult.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmingResult = await _userManager.ConfirmEmailAsync(user, code);
                if (confirmingResult.Succeeded)
                {
                    if (_dbContext.UserActivities is not null)
                    {

                        await _accountLogic.RegisterUserActivityAsync(request.Login ?? request.Email, userId);

                        return CreatedAtAction("Register", new { UserId = userId });
                    }
                    else
                        return BadRequest(new ErrorMessageResponse("There are errors during creating a user: \nUnable to add a User Activity record."));
                }
                else
                    return BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \n" + string.Join('\n', confirmingResult.Errors.Select(er => er.Description))));
            }
            else
                return BadRequest(new ErrorMessageResponse("There are errors during creating a user: \n" + string.Join('\n', userCreatingResult.Errors.Select(er => er.Description))));
        }

        [Route("Logout")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LogoutAsync()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var redisDb = _redis.GetDatabase();
            var redisResult = await redisDb.StringSetAsync(new RedisKey(accessToken), new RedisValue(Constants.DisabledManuallyName));
            if (redisResult)
                return Ok();
            else
                throw new InternalAuthenticationErrorException("Unable to logout the token.");
        }

        private async Task<ClaimsIdentity?> GetIdentityAsync(string username, string password)
        {
            var signInStatus = await CheckUserCredentialsAsync(username, password);
            
            if (signInStatus == SignInStatus.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            else
                return null;
        }

        private async Task<SignInStatus> CheckUserCredentialsAsync(string username, string password)
        {
            if (_userManager == null)
            {
                return SignInStatus.Failure;
            }
            else
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return SignInStatus.Failure;
                }
                else if (await _userManager.IsLockedOutAsync(user))
                {
                    return SignInStatus.LockedOut;
                }
                else if (await _userManager.CheckPasswordAsync(user, password))
                {
                    return SignInStatus.Success;
                }
                else
                {
                    return SignInStatus.Failure;
                }
            }
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. ");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
