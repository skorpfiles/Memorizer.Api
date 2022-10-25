namespace SkorpFiles.Memorizer.Api.Web.Models.Responses
{
    public class ErrorMessageResponse
    {
        public string ErrorMessage { get; set; }

        public ErrorMessageResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
