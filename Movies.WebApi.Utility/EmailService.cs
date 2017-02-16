using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Movies.WebApi.Utility
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var key = ConfigurationManager.AppSettings["SendGridApiKey"];
            var sg = new SendGridAPIClient(key);
            var from = new Email("michalglowaczewski@gmail.com");
            var subject = message.Subject;
            var to = new Email(message.Destination);
            var content = new Content("text/html", message.Body);
            var mail = new Mail(from, subject, to, content);

            sg.client.mail.send.post(requestBody: mail.Get());
            return Task.FromResult(0);
        }
    }
}