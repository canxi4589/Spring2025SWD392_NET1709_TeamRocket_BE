using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HCP.Service.Services.EmailService
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string to, string subject, string body)
        {
            BackgroundJob.Enqueue(() => SendEmailAsync(to, subject, body));
            Console.WriteLine($"Email queued for {to}");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Home Cleaning Service Platform", _configuration["EmailSettings:SenderEmail"]));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();

                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                int smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
                string smtpUsername = _configuration["EmailSettings:Username"];
                string smtpPassword = _configuration["EmailSettings:Password"];

                Console.WriteLine($"Attempting to connect to {smtpServer}:{smtpPort}");
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                Console.WriteLine("Connected, attempting authentication");
                await smtp.AuthenticateAsync(smtpUsername, smtpPassword);

                Console.WriteLine("Authenticated, sending email");
                await smtp.SendAsync(email);

                Console.WriteLine("Email sent successfully");
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {to}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Consider rethrowing if you want failed jobs to retry
                // throw; 
            }
        }
    }
}
