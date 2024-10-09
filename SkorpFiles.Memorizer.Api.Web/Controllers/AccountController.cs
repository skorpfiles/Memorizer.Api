using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Web.Models.Requests.Authorization;
using SkorpFiles.Memorizer.Api.Web.Authorization;
using SkorpFiles.Memorizer.Api.Web.Enums;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SkorpFiles.Memorizer.Api.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;
using SkorpFiles.Memorizer.Api.DataAccess.Extensions;
using System.Web;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System.Text;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IAccountLogic _accountLogic;
        private readonly ITokenCache _tokenCache;

        public AccountController( 
            ITokenCache tokenCache, IConfiguration configuration, IAccountLogic accountLogic, IUserStore<ApplicationUser> userStore, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
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

            var authenticationTokenCipherKey = _configuration["AuthenticationTokenCipherKey"] ?? throw new InternalAuthenticationErrorException("Configuration problem: unable to get cipher key.");

            var (status, user, identity) = await GetIdentityAsync(request.Login, request.Password);
            if (status == SignInStatus.Failure)
                return Unauthorized(new { errorCode = "InvalidLoginPassword", errorText = "Invalid login or password." });
            else if ((status == SignInStatus.Success || status == SignInStatus.EmailNotConfirmed) && identity!=null && user!=null)
            {

                var now = DateTime.UtcNow;

                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationTokenCipherKey)), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                if (encodedJwt != null)
                {
                    if (await _tokenCache.SetAsync(encodedJwt, Constants.DefaultName))
                    {
                        return Json(new
                        {
                            AccessToken = encodedJwt,
                            Login = identity.Name,
                            UserId = user.Id,
                            IsEmailConfirmed = status != SignInStatus.EmailNotConfirmed
                        });
                    }
                    else
                        throw new InternalAuthenticationErrorException("Unable to cache authentication information.");
                }
                else
                    throw new InternalAuthenticationErrorException("Unable to generate a JWT token.");
            }
            else
                throw new InternalAuthenticationErrorException("Unknow sign-in status identifier.");
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
                    return Unauthorized(new { errorText = "CAPTCHA isn't passed." });
            }

            IActionResult result;

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, request.Login ?? request.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);

            var userCreatingResult = await _userManager.CreateAsync(user, request.Password);

            if (userCreatingResult.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                if (_configuration.GetValue<bool>("CheckEmailConfirmation"))
                {
                    if (await SendUserConfirmationEmailAsync(userId, request.Email, request.Login, code))
                        result = CreatedAtRoute("GetAccount", new { id = userId }, new RegisterResponse { UserId = Guid.Parse(userId), IsConfirmationRequired = true });
                    else
                        result = BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \nUnsuccessful email provider request."));
                }
                else
                {
                    var confirmingResult = await _userManager.ConfirmEmailAsync(user, code);
                    if (confirmingResult.Succeeded)
                    {
                        await _accountLogic.RegisterUserActivityAsync(request.Login ?? request.Email, userId);
                        result = CreatedAtRoute("GetAccount", new { id = userId }, new RegisterResponse { UserId = Guid.Parse(userId), IsConfirmationRequired = false });
                    }
                    else
                        result = BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \n" + string.Join('\n', confirmingResult.Errors.Select(er => er.Description))));
                }
            }
            else
                result = BadRequest(new ErrorMessageResponse("There are errors during creating a user: \n" + string.Join('\n', userCreatingResult.Errors.Select(er => er.Description))));

            return result;
        }

        private async Task<bool> SendUserConfirmationEmailAsync(string userId, string email, string? login, string confirmationCode)
        {
            return await SendGridUtils.SendEmailAsync(
                    _configuration["EmailConfirmation_ApiKey"]!,
                    _configuration["EmailConfirmation_EmailFrom"]!,
                    _configuration["EmailConfirmation_EmailFromName"]!,
                    email,
                    login ?? "New User",
                    _configuration["AuthenticationConfirmation:Subject"]!,
                    string.Format(_configuration["AuthenticationConfirmation:BodyTemplate"]!, string.Format(_configuration["EmailConfirmation_FrontendLinkTemplate"]!, userId, HttpUtility.UrlEncode(confirmationCode)))
                    );
        }

        [Route("RepeatEmailConfirmation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> RepeatEmailConfirmationAsync()
        {
            IActionResult result;
            var user = await _userStore.FindByNameAsync(User.Identity!.Name!, new CancellationToken());
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user!);

            if (_configuration.GetValue<bool>("CheckEmailConfirmation"))
            {
                if (await SendUserConfirmationEmailAsync(user!.Id, user!.Email!, user.UserName, code))
                    result = Ok("Check the email to continue.");
                else
                    result = BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \nUnsuccessful email provider request."));
            }
            else
            {
                var confirmingResult = await _userManager.ConfirmEmailAsync(user!, code);
                if (confirmingResult.Succeeded)
                    result = Ok();
                else
                    result = BadRequest(new ErrorMessageResponse("There are errors during email confirmation: \n" + string.Join('\n', confirmingResult.Errors.Select(er => er.Description))));
            }

            return result;
        }

        [Route("ConfirmRegistration")]
        [HttpPost]
        public async Task<IActionResult> ConfirmRegistrationAsync(ConfirmRegistrationRequest request)
        {
            if (request.UserId == Guid.Empty || request.ConfirmationCode == null)
                return BadRequest(new { ErrorText = "UserId and ConfirmationCode cannot be empty." });

            const string unauthorizedErrorCode = "Unable to confirm the user by the code.";

            var user = await _userManager.FindByIdAsync(request.UserId.ToAspNetUserIdString()!);

            if (user == null)
                return Unauthorized(new { errorCode = "NoConfirmation", errorText = unauthorizedErrorCode });

            var confirmingResult = await _userManager.ConfirmEmailAsync(user, request.ConfirmationCode);
            if (confirmingResult.Succeeded)
                return Ok();
            else
                return Unauthorized(new { errorCode = "NoConfirmation", errorText = unauthorizedErrorCode });
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

        private async Task<(SignInStatus status, ApplicationUser? user, ClaimsIdentity? claims)> GetIdentityAsync(string username, string password)
        {
            var (status, user) = await CheckUserCredentialsAsync(username, password);

            if (status == SignInStatus.Success || status == SignInStatus.EmailNotConfirmed)
            {
                var claims = new List<Claim>
                {
                    new(ClaimsIdentity.DefaultNameClaimType, username),
                    new(ClaimsIdentity.DefaultRoleClaimType, "admin")
                };
                ClaimsIdentity claimsIdentity =
                new(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return (status, user, claimsIdentity);
            }
            else
                return (status, null, null);
        }

        private async Task<(SignInStatus status, ApplicationUser? user)> CheckUserCredentialsAsync(string username, string password)
        {
            if (_userManager == null)
            {
                return (SignInStatus.Failure, null);
            }
            else
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return (SignInStatus.Failure, null);
                }
                else if (await _userManager.IsLockedOutAsync(user))
                {
                    return (SignInStatus.LockedOut, null);
                }
                else if (await _userManager.CheckPasswordAsync(user, password))
                {
                    if (user.EmailConfirmed)
                    {
                        return (SignInStatus.Success, user);
                    }
                    else
                    {
                        return (SignInStatus.EmailNotConfirmed, user);
                    }    
                }
                else
                {
                    return (SignInStatus.Failure, null);
                }
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. ");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
