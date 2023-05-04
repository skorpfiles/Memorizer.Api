using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.Net.Http.Headers;
using SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic;
using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController:Controller
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IAccountLogic _accountLogic;

        public TestController(IConnectionMultiplexer redis, IAccountLogic accountLogic)
        {
            _redis = redis;
            _accountLogic = accountLogic;
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
            var redisDb = _redis.GetDatabase();
            var redisResult = await redisDb.StringGetAsync(new RedisKey(accessToken));
            return Ok(redisResult.ToString() !=null && redisResult.ToString() != Constants.DisabledManuallyName);
        }

        [Route("SetRedisKeyValue")]
        [HttpPost]
        public async Task<IActionResult> SetRedisKeyValue([FromQuery]string key, [FromQuery]string value)
        {
            var db = _redis.GetDatabase();
            if (await db.StringSetAsync(new RedisKey(key), new RedisValue(value)))
                return Ok();
            else
                throw new Exception();
        }

        [Route("GetRedisValue")]
        [HttpGet]
        public async Task<IActionResult> GetRedisValue([FromQuery] string key)
        {
            var db = _redis.GetDatabase();
            var foo = await db.StringGetAsync(new RedisKey(key));
            return Ok(foo.ToString());
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
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = "InstrumentationKey=48f82062-d2fa-469a-a4a6-e873a3dc6db9;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/";
            var ai = new TelemetryClient(config);
            ai.TrackException(new NullReferenceException());
            return Ok();
        }
    }
}
