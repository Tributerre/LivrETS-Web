using System;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace LivrETS.Models
{
    public class NotificationManager
    {
        private const string EMAIL_SUBJECT_NOTIF = "Notification de TRIBUTERRE";


        private static NotificationManager notificationManager;
        static readonly object instanceLock = new object();

        private NotificationManager(){}

        public static NotificationManager getInstance()
        {
            if (notificationManager == null)
            {
                lock (instanceLock)
                {
                    if (notificationManager == null)
                        notificationManager = new NotificationManager();
                }
            }

            return notificationManager;
        }

        public void sendNotification(Notification notification)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_CLIENT"]);

                mail.From = new MailAddress(notification.emailProvider);

                foreach (ApplicationUser user in notification.listUser)
                {
                    mail.To.Add(user.Email);
                }

                mail.Subject = EMAIL_SUBJECT_NOTIF;

                mail.IsBodyHtml = true;
                mail.Body = notification.template;

                SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["EMAIL_PORT"]);
                SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_USERNAME"], 
                    ConfigurationManager.AppSettings["EMAIL_PWD"]);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
        }
    }
}