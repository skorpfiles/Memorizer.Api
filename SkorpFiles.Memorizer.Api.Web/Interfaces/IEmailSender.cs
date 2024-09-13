using SkorpFiles.Memorizer.Api.Web.Models.Helpers;

namespace SkorpFiles.Memorizer.Api.Web.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(EmailParameters parameters);
    }
}
