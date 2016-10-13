using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using LivrETS.Properties;

namespace LivrETS.Models
{
    public class NotificationManager
    {       
        private const string EMAIL_SUBJECT_NOTIF = "Notification de TRIBUTERRE";
        private static NotificationManager notificationManager;
        static readonly object instanceLock = new object();

        private NotificationManager()
        {

        }

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
                SmtpClient SmtpServer = new SmtpClient(Resources.SMTP_CLIENT);

                mail.From = new MailAddress(notification.emailProvider);
                mail.To.Add("willkoua@yahoo.fr");
                mail.Subject = EMAIL_SUBJECT_NOTIF;

                mail.IsBodyHtml = true;
                mail.Body = notification.template;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Resources.EMAIL_USERNAME, Resources.EMAIL_PWD);
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