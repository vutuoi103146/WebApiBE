using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApiBE.BL
{
    public class L3Email
    {
        public static bool SendMail(string mailTo, string mailSubject, string mailBody, string mailCc)
        {
            try
            {
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                SmtpServer.Credentials = new System.Net.NetworkCredential("phongsonpt@yahoo.com", "Vutuoi@@103146");
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.mail.yahoo.com";
                SmtpServer.EnableSsl = true;
                mail = new MailMessage();
                mail.From = new MailAddress("phongsonpt@yahoo.com", "Mail Server System");
                mail.To.Add(mailTo);
                if (mailCc !="" ) mail.CC.Add(mailCc);
                mail.Subject = mailSubject;
                mail.Body = mailBody;
                mail.IsBodyHtml = true;
                // SmtpServer.UseDefaultCredentials = False
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                LogFileApi.LogFile("Send mail Error: Mail To:" + mailTo);
                return false;
            }
        }
    }
}