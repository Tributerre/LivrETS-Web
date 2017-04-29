using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System;
using LivrETS.ViewModels;

namespace LivrETS.Models
{
    public class Notification
    {
        public string template = null;
        public string emailProvider = null;
        public List<UserViewModel> listUser = null;

        public Notification(NotificationOptions option, List<UserViewModel> listUser)
        {
            this.emailProvider = ConfigurationManager.AppSettings["EMAIL_PROVIDER"];
            this.template = GetMessage(option);
            this.listUser = listUser;
        }



        public string GetMessage(NotificationOptions option)
        {
            string pathMail = "~/Content/Notifications/";

            string header_mail = "<div style='background:#629c49;padding:3px 10px;color:black;'>"+
                                    "<div style='float:left;'><h1>TRIBUTERRE</h1></div>"+
                                    "<div style='margin-left:70%;'><h1 style='color:white;'>"+
                                    "Notification de LivrÈTS</h1></div></div></div>";
            string footer_mail = "<br><p>L'équipe LivrÈTS</p><p>livrets@tributerre.com</p>"+
                                   "<div style='background:#629c49;padding:3px 10px;color:black;'>"+
                                   "<h1>MERCI</h1>";
            string message = null;

            switch (option)
            {
                case NotificationOptions.STARTFAIRRETREIVAL:
                    pathMail += "late_for_retreival_before_a_week.txt";
                    break;
                case NotificationOptions.LATEFORRETREIVALAFTERAWEEK:
                    pathMail += "late_for_retreival_after_a_week.txt";
                    break;
            }

            try
            {
                message = System.IO.File.ReadAllText(Path.GetFullPath(
                            System.Web.HttpContext.Current.Server.MapPath(pathMail)
                ));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            return header_mail +"<div color:black;>"+ message +"</div>"+ footer_mail;
        }
    }

    public enum NotificationOptions
    {
        //courriel vieille de la recuperation
        STARTFAIRRETREIVAL = 0,    
        //courriel de recuperation apres une semaine
        LATEFORRETREIVALAFTERAWEEK = 1

    }
}