namespace SkorpFiles.Memorizer.Api.Web.Models.Requests.Authorization
{
    public class ConfirmRegistrationRequest
    {
        public Guid UserId { get; set; }
        public string? ConfirmationCode { get; set; }
    }
}
