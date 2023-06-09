using Newtonsoft.Json;
using RestSharp;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Models.SideApis;
using System.Linq;

namespace SkorpFiles.Memorizer.Api.Web
{
    public static class CaptchaUtils
    {
        private static readonly string[] _responseErrorCodesForInvalidCaptcha =
        {
            "missing-input-response",
            "invalid-input-response"
        };

        public static bool ShouldCheckCaptcha(IConfiguration configuration)
        {
            try
            {
                return configuration.GetValue<bool>("CheckCaptcha");
            }
            catch(Exception ex)
            {
                throw new InternalErrorException("Error while checking captcha: hasn't been configured.", ex);
            }
        }

        public static async Task<bool> IsCaptchaValidAsync(IConfiguration configuration, string captchaToken)
        {
            try
            {
                var secretKey = configuration["CaptchaSecretKey"]!;
                RestClient restClient = new(string.Format(configuration["Captcha:UrlTemplateForCheck"]!, secretKey, captchaToken));
                var response = await restClient.PostAsync(new RestRequest());
                if (response.IsSuccessful)
                {
                    var responseObject = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(response.Content!);
                    if (responseObject!.Success)
                    {
                        return true;
                    }
                    else if (responseObject!.ErrorCodes!=null && responseObject!.ErrorCodes.Intersect(_responseErrorCodesForInvalidCaptcha).Any())
                    {
                        return false;
                    }
                    else
                    {
                        throw new InternalErrorException($"A negative CAPTCHA answer with unexpected error codes.");
                    }
                }
                else
                {
                    throw new InternalErrorException($"An unsuccessful response from the CAPTCHA server.");
                }
            }
            catch (Exception ex)
            {
                throw new InternalErrorException(ex.Message, ex);
            }
        }
    }
}
