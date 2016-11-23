using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace LivrETS.Models
{
    public class Notification
    {
        public string template = null;
        public string emailProvider = null;
        public List<ApplicationUser> listUser = null;
        public ApplicationUser user = null;

        public Notification(NotificationOptions option, List<ApplicationUser> listUser)
        {
            this.emailProvider = ConfigurationManager.AppSettings["EMAIL_PROVIDER"];
            this.template = GetMessage(option);
            this.listUser = listUser;
        }

        public Notification(NotificationOptions option, ApplicationUser user)
        {
            this.emailProvider = ConfigurationManager.AppSettings["EMAIL_PROVIDER"];
            this.template = GetMessage(option);
            this.user = user;
        }

        public string GetMessage(NotificationOptions option)
        {
            string pathMail = "~/Content/Notifications/";

            string header_mail = "<div style='background:#629c49;padding:3px 10px;color:black;'>"+
                                    "<div style='float:left;'><h1>TRIBUTERRE</h1></div>"+
                                    "<div style='margin-left:70%;'><h1 style='color:white;'>"+
                                    "Notification de LivRÈTS</h1></div></div></div>";
            string footer_mail = "<br><p>L'équipe LivrÈTS</p><p>livrets@tributerre.com</p>"+
                                   "<div style='background:#629c49;padding:3px 10px;color:black;'>"+
                                   "<h1>MERCI</h1>";

            switch (option)
            {
                case NotificationOptions.STARTFAIRPICKING:
                    pathMail += "start_fair_picking.txt";
                    break;
                case NotificationOptions.ENDFAIRPICKING:
                    pathMail += "end_fair_picking.txt";
                    break;
                case NotificationOptions.ENDFAIRRETREIVAL:
                    pathMail += "end_fair_retreival.txt";
                    break;
                case NotificationOptions.STARTFAIRSALE:
                    pathMail += "start_fair_sale.txt";
                    break;
                case NotificationOptions.ENDFAIRSALE:
                    pathMail += "end_fair_sale.txt";
                    break;
                case NotificationOptions.STARTFAIRRETREIVAL:
                    pathMail += "start_fair_retreival.txt";
                    break;
                case NotificationOptions.LATEFORRETREIVALBEFOREAWEEK:
                    pathMail += "late_for_retreival_before_a_week.txt";
                    break;
                case NotificationOptions.LATEFORRETREIVALAFTERAWEEK:
                    pathMail += "late_for_retreival_after_a_week.txt";
                    break;
                case NotificationOptions.ARTICLEMARKEDASSOLDDURINGFAIR:
                    pathMail += "article_marked_as_soldduring_fair.txt";
                    break;
                case NotificationOptions.ARTICLEPICKEDCONFIRMATION:
                    pathMail += "article_picked_confirmation.txt";
                    break;
                case NotificationOptions.ARTICLERETREIVEDCONFIRMATION:
                    pathMail += "article_retreived_confirmation.txt";
                    break;
                case NotificationOptions.ARTICLETRANSFEREDTOTRIBUTERRE:
                    pathMail += "article_transfered_to_tributerre.txt";
                    break;
            }

            string message = System.IO.File.ReadAllText(Path.GetFullPath(
                System.Web.HttpContext.Current.Server.MapPath(pathMail)
                ));

            return header_mail +"<div color:black;>"+ message +"</div>"+ footer_mail;
        }
    }

    public enum NotificationOptions
    {
        STARTFAIRPICKING = 0,
        ENDFAIRPICKING = 1,
        ENDFAIRRETREIVAL = 2,
        STARTFAIRSALE = 3,
        ENDFAIRSALE = 4,
        STARTFAIRRETREIVAL = 5,
        LATEFORRETREIVALBEFOREAWEEK = 6,
        LATEFORRETREIVALAFTERAWEEK = 7,
        ARTICLEMARKEDASSOLDDURINGFAIR = 8,
        ARTICLEPICKEDCONFIRMATION = 9,
        ARTICLERETREIVEDCONFIRMATION = 10,
        ARTICLETRANSFEREDTOTRIBUTERRE = 11

    }
}