namespace SkorpFiles.Memorizer.Api.Models.Requests.Authorization
{
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
