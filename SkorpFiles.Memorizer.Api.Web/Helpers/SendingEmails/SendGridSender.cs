using SendGrid.Helpers.Mail;
using SendGrid;
using SkorpFiles.Memorizer.Api.Web.Interfaces;
using SkorpFiles.Memorizer.Api.Web.Models.Helpers;
using SendGrid.Helpers.Mail.Model;
using SkorpFiles.Memorizer.Api.Web.Exceptions;

namespace SkorpFiles.Memorizer.Api.Web.Helpers.SendingEmails
{
    public class SendGridSender : IEmailSender
    {
        public async Task<bool> SendEmailAsync(EmailParameters parameters)
        {
            if (parameters.ApiKey != null)
            {
                var client = new SendGridClient(parameters.ApiKey);
                var from = new EmailAddress(parameters.FromAddress, parameters.FromName);
                var to = new EmailAddress(parameters.ToAddress, parameters.ToName);
                var msg = MailHelper.CreateSingleEmail(from, to, parameters.Subject, parameters.Content, parameters.Content);
                var response = await client.SendEmailAsync(msg);
                return response.IsSuccessStatusCode;
            }
            else
            {
                throw new SendingEmailException("There is no correct access key");
            }
        }
    }
}
