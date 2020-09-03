using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        private Task Execute(string email, string subject, string htmlMessage)
        {

            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Sender),
            };

            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            smtp.Connect(_options.SmtpServer, _options.Port);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_options.Username, _options.Password);
            smtp.Send(message);
            smtp.Disconnect(true);

            return Task.FromResult(0);
        }
    }
}
