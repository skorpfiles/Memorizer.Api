namespace SkorpFiles.Memorizer.Api.ApiModels.Responses
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
