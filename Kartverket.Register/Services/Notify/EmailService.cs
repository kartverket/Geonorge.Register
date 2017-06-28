using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace Kartverket.Register.Services.Notify
{
    public class EmailService : IEmailService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Send(MailMessage message)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = WebConfigurationManager.AppSettings["SmtpHost"];
                smtpClient.Send(message);
            }
        }
    }
}
