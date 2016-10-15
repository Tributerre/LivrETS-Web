using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace LivrETS.Models
{
    public class Notification
    {
        public string template = null;
        public string emailProvider = null;
        public List<string> listUser = null;

        public Notification(NotificationOptions option, List<string> listUser)
        {
            this.emailProvider = LivrETS.Properties.Resources.EMAIL_PROVIDER;
            this.template = GetMessage(option);
            this.listUser = listUser;
        }

        public string GetMessage(NotificationOptions option)
        {
            string pathMail = "~/Content/Notifications/";

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

            return System.IO.File.ReadAllText(Path.GetFullPath(System.Web.HttpContext.Current.Server.MapPath(pathMail)));
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