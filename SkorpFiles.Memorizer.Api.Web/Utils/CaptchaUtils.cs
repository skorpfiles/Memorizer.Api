using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Models.SideApis;

namespace SkorpFiles.Memorizer.Api.Web.Utils
{
    public static class CaptchaUtils
    {
        private static readonly string[] _responseErrorCodesForInvalidCaptcha =
        {
            "missing-input-response",
            "invalid-input-response",
            "timeout-or-duplicate"
        };

        public static bool ShouldCheckCaptcha(IConfiguration configuration)
        {
            try
            {
                return configuration.GetValue<bool>("CheckCaptcha");
            }
            catch (Exception ex)
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
                    else if (responseObject!.ErrorCodes != null && responseObject!.ErrorCodes.Intersect(_responseErrorCodesForInvalidCaptcha).Any())
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

        // Create an assessment to analyze the risk of an UI action.
        // projectID: GCloud Project ID.
        // recaptchaSiteKey: Site key obtained by registering a domain/app to use recaptcha.
        // token: The token obtained from the client on passing the recaptchaSiteKey.
        // recaptchaAction: Action name corresponding to the token.
        public static void CreateAssessment(string projectID = "project-id", string recaptchaSiteKey = "recaptcha-site-key",
            string token = "action-token", string recaptchaAction = "login")
        {

            // Create the client.
            // TODO: To avoid memory issues, move this client generation outside
            // of this example, and cache it (recommended) or call client.close()
            // before exiting this method.
            RecaptchaEnterpriseServiceClient client = RecaptchaEnterpriseServiceClient.Create();

            ProjectName projectName = new ProjectName(projectID);

            // Build the assessment request.
            CreateAssessmentRequest createAssessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    // Set the properties of the event to be tracked.
                    Event = new Event()
                    {
                        SiteKey = recaptchaSiteKey,
                        Token = token,
                        ExpectedAction = recaptchaAction
                    },
                },
                ParentAsProjectName = projectName
            };

            Assessment response = client.CreateAssessment(createAssessmentRequest);

            // Check if the token is valid.
            if (response.TokenProperties.Valid == false)
            {
                System.Console.WriteLine("The CreateAssessment call failed because the token was: " +
                    response.TokenProperties.InvalidReason.ToString());
                return;
            }

            // Check if the expected action was executed.
            if (response.TokenProperties.Action != recaptchaAction)
            {
                System.Console.WriteLine("The action attribute in reCAPTCHA tag is: " +
                    response.TokenProperties.Action.ToString());
                System.Console.WriteLine("The action attribute in the reCAPTCHA tag does not " +
                    "match the action you are expecting to score");
                return;
            }

            // Get the risk score and the reason(s).
            // For more information on interpreting the assessment,
            // see: https://cloud.google.com/recaptcha-enterprise/docs/interpret-assessment
            System.Console.WriteLine("The reCAPTCHA score is: " + ((decimal)response.RiskAnalysis.Score));

            foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
            {
                System.Console.WriteLine(reason.ToString());
            }
        }
    }
}
