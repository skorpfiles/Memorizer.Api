using SendGrid.Helpers.Mail;
using SendGrid;

namespace SkorpFiles.Memorizer.Api.Web
{
    public static class SendGridUtils
    {
        public static async Task<bool> SendEmailAsync(string key, string emailFrom, string emailFromName, string emailTo, string emailToName, string subject, string htmlContent)
        {
            var client = new SendGridClient(key);
            var from = new EmailAddress(emailFrom, emailFromName);
            var to = new EmailAddress(emailTo, emailToName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
    }
}
