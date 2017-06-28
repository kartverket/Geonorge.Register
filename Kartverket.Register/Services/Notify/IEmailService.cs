using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Kartverket.Register.Services.Notify
{
    public interface IEmailService
    {
        void Send(MailMessage message);
    }
}