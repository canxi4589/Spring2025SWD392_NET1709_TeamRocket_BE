using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.EmailService
{
    public interface IEmailSenderService
    {
        void SendEmail(string to, string subject, string body);
    }
}
