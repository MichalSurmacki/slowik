using System;
using System.Configuration;
using Application.Interfaces;
using Application.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings;
        }
        
        public void SendCorpusGuidViaEmail(string emailTo, Guid corpusId)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("slowik", "slowik@slowik.pl"));
            mailMessage.To.Add(new MailboxAddress(emailTo, emailTo));
            mailMessage.Subject = "Your corpus has been prepared!";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Id of your corpus is: {corpusId.ToString()}"
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(_emailSettings.Value.Email, _emailSettings.Value.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }
    }
}