namespace SkorpFiles.Memorizer.Api.Web.Models.Responses
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public bool IsConfirmationRequired { get; set; }
    }
}
