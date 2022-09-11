namespace SkorpFiles.Memorizer.Api.Models.Requests.Authorization
{
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
