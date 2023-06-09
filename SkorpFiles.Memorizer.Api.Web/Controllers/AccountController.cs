using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Authorization;
using SkorpFiles.Memorizer.Api.Web.Authorization;
using SkorpFiles.Memorizer.Api.Web.Enums;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SkorpFiles.Memorizer.Api.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
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
        private readonly IAccountLogic _accountLogic;
        private readonly ITokenCache _tokenCache;

        public AccountController( 
            ITokenCache tokenCache, IConfiguration configuration, IAccountLogic accountLogic, IUserStore<IdentityUser> userStore, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _tokenCache = tokenCache;
            _configuration = configuration;
            _accountLogic = accountLogic;

            _userStore = userStore;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailStore = GetEmailStore();
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
                if (await _tokenCache.SetAsync(encodedJwt, Constants.DefaultName))
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

        [Route("Check")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult CheckToken()
        {
            return Ok();
        }

        [Route("Register")]
        [HttpPut]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (request.Email == null || request.Password == null)
                return BadRequest(new { ErrorText = "Login and password cannot be null." });

            if (CaptchaUtils.ShouldCheckCaptcha(_configuration))
            {
                if (request.CaptchaToken == null || !await CaptchaUtils.IsCaptchaValidAsync(_configuration, request.CaptchaToken))
                    return Unauthorized("CAPTCHA isn't passed.");
            }

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
                    await _accountLogic.RegisterUserActivityAsync(request.Login ?? request.Email, userId);
                    return CreatedAtRoute("GetAccount", new { id = userId }, new { UserId = userId });
                }
                else
                    return BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \n" + string.Join('\n', confirmingResult.Errors.Select(er => er.Description))));
            }
            else
                return BadRequest(new ErrorMessageResponse("There are errors during creating a user: \n" + string.Join('\n', userCreatingResult.Errors.Select(er => er.Description))));
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken))
            {
                if (await _tokenCache.GetAsync(accessToken) == Constants.DefaultName)
                {
                    if (await _tokenCache.SetAsync(accessToken, Constants.DisabledManuallyName))
                    {
                        return Ok();
                    }
                    else
                        throw new InternalAuthenticationErrorException("Unable to logout the token.");
                }
                else
                    return Ok();
            }
            else
                return BadRequest("No authentication token.");
        }

        [Route("{id}", Name = "GetAccount")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Index(string id)
        {
            throw new NotImplementedException();
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
