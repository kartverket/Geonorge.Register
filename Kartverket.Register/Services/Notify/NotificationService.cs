using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Kartverket.Register.Services.Notify
{
    public class NotificationService : INotificationService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEmailService _emailService;

        public NotificationService
        (
            IEmailService emailService
        )
        {
            _emailService = emailService;
        }

        public void SendSubmittedNotification(Models.Document document)
        {
            var message = CreateReadyForDownloadEmailMessage(document);

            SendEmailNotification(message);
        }

        public MailMessage CreateReadyForDownloadEmailMessage(Models.Document document)
        {
            var message = new MailMessage();
            var email = WebConfigurationManager.AppSettings["EmailNotification"];
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(WebConfigurationManager.AppSettings["WebmasterEmail"]);
            message.Subject = "Register sendt inn: " + document.name;
            var body = new StringBuilder();
            body.AppendLine(document.name + " er sendt inn.");
            var registerLink = WebConfigurationManager.AppSettings["RegistryUrl"] + document.GetDocumentUrl();
            registerLink = registerLink.Replace("//", "/");
            body.AppendLine("<br />Link til register: <a href='"+ registerLink + "'>" + registerLink + "</a>");

            message.IsBodyHtml = true;
            message.Body = body.ToString();

            Log.Info("Sending email notification for submitted document: " + document.name + ", to:" + email);

            return message;
        }

        public void SendEmailNotification(MailMessage message)
        {
            _emailService.Send(message);
        }
    }
}
