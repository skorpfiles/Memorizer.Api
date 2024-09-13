using SkorpFiles.Memorizer.Api.Web.Interfaces;
using SkorpFiles.Memorizer.Api.Web.Models.Helpers;

namespace SkorpFiles.Memorizer.Api.Web.Helpers.SendingEmails
{
    public class SmtpSender : IEmailSender
    {
        public Task<bool> SendEmailAsync(EmailParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
