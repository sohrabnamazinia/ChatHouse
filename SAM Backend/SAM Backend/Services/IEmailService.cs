using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using SAM_Backend.ViewModels.Email;

namespace SAM_Backend.Services
{
    public interface IEmailService
    {
        public void SendEmail(MimeMessage message, BodyBuilder bodyBuilder);
        public void SendEmailConfirmation(SendEmailConfirmationViewModel model);
    }
}
