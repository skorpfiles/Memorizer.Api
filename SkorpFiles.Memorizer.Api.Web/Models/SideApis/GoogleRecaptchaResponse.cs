using Newtonsoft.Json;

namespace SkorpFiles.Memorizer.Api.Web.Models.SideApis
{
    public class GoogleRecaptchaResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        [JsonProperty(PropertyName = "hostname")]
        public string? Hostname { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public string[]? ErrorCodes { get; set; } 
    }
}
