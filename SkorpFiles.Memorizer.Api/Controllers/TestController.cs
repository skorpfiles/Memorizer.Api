using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace SkorpFiles.Memorizer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController:Controller
    {
        private readonly IConnectionMultiplexer _redis;

        public TestController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [Route("AuthorizedTest")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AuthorizedTest()
        {
            return Ok("Success");
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
    }
}
