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

        public bool sendNotification(Notification notification)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_CLIENT"]);

                mail.From = new MailAddress(notification.emailProvider);
                mail.Subject = EMAIL_SUBJECT_NOTIF;

                mail.IsBodyHtml = true;
                mail.Body = notification.template;

                SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["EMAIL_PORT"]);
                SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_USERNAME"], 
                    ConfigurationManager.AppSettings["EMAIL_PWD"]);
                SmtpServer.EnableSsl = true;
                
                if(notification.user == null)
                {
                    foreach (ApplicationUser user in notification.listUser)
                    {
                        mail.Body = string.Format(notification.template, user.FirstName);
                        mail.To.Add(user.Email);
                        
                    }
                    SmtpServer.Send(mail);
                }
                else
                {
                    ApplicationUser user = notification.user;
                    mail.Body = string.Format(notification.template, user.FirstName);
                    mail.To.Add(user.Email);
                    SmtpServer.Send(mail);
                }
                
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }

            return false;
        }

        private string getBetween(string strSource, string addelt)
        {
            int End = strSource.IndexOf("Bonjour");
            return strSource.Insert(End, " "+ addelt);
            
        }
    }
}