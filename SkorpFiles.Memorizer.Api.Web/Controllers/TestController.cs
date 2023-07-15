﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;
using SkorpFiles.Memorizer.Api.Web.Utils;
using StackExchange.Redis;
using System.Reflection;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController:Controller
    {
        private readonly ITokenCache _tokenCache;
        private readonly IAccountLogic _accountLogic;
        private readonly ILogger<TestController> _logger;

        public TestController(ITokenCache tokenCache, IAccountLogic accountLogic, ILogger<TestController> logger)
        {
            _tokenCache = tokenCache;
            _accountLogic = accountLogic;
            _logger = logger;
        }

        [Route("AuthorizedTest")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AuthorizedTest()
        {
            return Ok("Success");
        }

        [Route("TryAuthorizeWithAbsentToken")]
        [HttpPost]
        public async Task<IActionResult> TryAuthorizeWithAbsentTokenAsync()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var tokenCacheResult = await _tokenCache.GetAsync(accessToken);
            return Ok(tokenCacheResult?.ToString() !=null && tokenCacheResult.ToString() != Constants.DisabledManuallyName);
        }

        [Route("SetRedisKeyValue")]
        [HttpPost]
        public async Task<IActionResult> SetRedisKeyValue([FromQuery]string key, [FromQuery]string value)
        {
            if (await _tokenCache.SetAsync(key, value))
                return Ok();
            else
                throw new Exception();
        }

        [Route("GetRedisValue")]
        [HttpGet]
        public async Task<IActionResult> GetRedisValue([FromQuery] string key)
        {
            var foo = await _tokenCache.GetAsync(key);
            return Ok(foo?.ToString());
        }

        [Route("AddUserActivityWithoutUser")]
        [HttpPost]
        public async Task<IActionResult> AddUserActivityWithoutUser()
        {
            await _accountLogic.RegisterUserActivityAsync("testUserName", Guid.NewGuid().ToString());
            return Ok();
        }

        [Route("TestAzureInsights")]
        [HttpGet]
        public IActionResult TestAzureInsights()
        {
            throw new Exception("Test exception");
            return Ok();
        }

        [Route("TestCaptcha")]
        public IActionResult TestCaptcha(string token)
        {
            CaptchaUtils.CreateAssessment(token);
            return Ok();
        }
    }
}
