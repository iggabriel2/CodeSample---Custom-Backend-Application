using Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.Utilities
{
    public class EmailSender
    {
        public static async Task sendEmail(string mailMessage, string subject, List<string> emails)
        {
            foreach (var email in emails)
            {
                String userName = AppSettings.EmailUsername();
                String password = AppSettings.EmailPassword();
                MailMessage msg = new MailMessage(AppSettings.EmailFrom(), email);
                msg.Subject = subject;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(mailMessage);
                msg.Body = sb.ToString();
                msg.IsBodyHtml = true;
                SmtpClient SmtpClient = new SmtpClient();
                SmtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
                SmtpClient.Host = AppSettings.SmtpServer();
                SmtpClient.Port = AppSettings.EmailPort();
                SmtpClient.EnableSsl = true;
                SmtpClient.Send(msg);
            }
        }
    }
}
