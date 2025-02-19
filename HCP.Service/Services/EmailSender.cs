using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
namespace HCP.Service.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_configuration["MailSettings:Host"])
        {
            Port = int.Parse(_configuration["MailSettings:Port"]),
            Credentials = new NetworkCredential(
                _configuration["MailSettings:Mail"],
                _configuration["MailSettings:Password"]),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["MailSettings:Mail"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
