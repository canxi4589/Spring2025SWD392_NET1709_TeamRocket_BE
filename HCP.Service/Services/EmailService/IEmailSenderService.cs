namespace HCP.Service.Services.EmailService
{
    public interface IEmailSenderService
    {
        void SendEmail(string to, string subject, string body);
    }
}
