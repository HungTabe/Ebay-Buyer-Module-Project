using CloneEbay.Interfaces;
using CloneEbay.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CloneEbay.Services
{
    public partial class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration)
        {
            _emailSettings = new EmailSettings
            {
                SenderEmail = configuration["EmailSettings:SenderEmail"],
                SenderName = configuration["EmailSettings:SenderName"],
                SmtpHost = configuration["EmailSettings:SmtpHost"],
                SmtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]),
                SmtpUsername = configuration["EmailSettings:SmtpUsername"],
                SmtpPassword = configuration["EmailSettings:SmtpPassword"]
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            using (var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))
            {
                smtp.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }
    }
}