using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MimeKit;
using SAM_Backend.Controllers;
using SAM_Backend.Models;
using SAM_Backend.ViewModels.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using SAM_Backend.Utility;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SAM_Backend.Services
{
    public class EmailService
    {
        #region Fields
        private readonly IWebHostEnvironment env;
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IJWTService jWTHandler;
        private readonly IMinIOService minIO;
        private readonly IConfiguration config;
        #endregion

        public EmailService(IWebHostEnvironment env, AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IJWTService jWTHandler, IMinIOService minIO, IConfiguration config)
        {
            #region CI
            this.env = env;
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.jWTHandler = jWTHandler;
            this.minIO = minIO;
            this.config = config;
            #endregion
        }

        #region general send email method
        public void SendEmail(MimeMessage message, BodyBuilder bodyBuilder)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect(Constants.SMTPGoogleDomain, Constants.SMTPPort, false);
                message.Body = bodyBuilder.ToMessageBody();
                client.Authenticate(Constants.ProjectEmail, config.GetValue<string>("ChatHouseEmailPassword"));
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
        #endregion

        #region send email confirmation 
        public void SendEmailConfirmation(SendEmailConfirmationViewModel model)
        {
            var pathToFile = env.WebRootPath + Constants.ConfirmAccountRegisterationViewPath;

            MimeMessage message = new MimeMessage();
            MailboxAddress sender = new MailboxAddress(Constants.ProjectSender, Constants.ProjectEmail);
            MailboxAddress reciever = new MailboxAddress(Constants.ProjectReciever, model.Email);
            message.From.Add(sender);
            message.To.Add(reciever);
            message.Subject = Constants.EmailConfirmationSubject;
            BodyBuilder bodyBuilder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                bodyBuilder.HtmlBody = SourceReader.ReadToEnd();
            }

            bodyBuilder.HtmlBody = string.Format(bodyBuilder.HtmlBody, model.FirstName, model.ConfirmationLink, model.Email, model.Username);
            SendEmail(message, bodyBuilder);
        }
        #endregion
    }
}
