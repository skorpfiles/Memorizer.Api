using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Authorization;
using SkorpFiles.Memorizer.Api.Enums;
using SkorpFiles.Memorizer.Api.Exceptions;
using SkorpFiles.Memorizer.Api.Models.Requests;
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

        private readonly IConnectionMultiplexer _redis;

        public AccountController(IUserStore<IdentityUser> userStore, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _redis = redis;
            _configuration = configuration;
        }

        [Route("Token")]
        [HttpPost]
        public async Task<IActionResult> Token(AuthenticationRequest request)
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
                        Username = identity.Name
                    });
                }
                else
                    throw new InternalAuthenticationErrorException("Unable to cache authentication information.");
            }
            else
                throw new InternalAuthenticationErrorException("Unable to generate JWT token.");
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(AuthenticationRequest request)
        {
            if (request.Login == null || request.Password == null)
                return BadRequest(new { errorText = "Login and password cannot be null." });

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, request.Login, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, request.Login, CancellationToken.None);

            var userCreatingResult = await _userManager.CreateAsync(user, request.Password);

            if (userCreatingResult.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var confirmingResult = await _userManager.ConfirmEmailAsync(user, code);
                if (confirmingResult.Succeeded)
                    return new JsonResult(new { UserId = userId });
                else
                    return new BadRequestResult();
            }
            else
                return new BadRequestResult();
        }

        [Route("Logout")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
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
