/*
LivrETS - Centralized system that manages selling of pre-owned ETS goods.
Copyright (C) 2016  TribuTerre

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/> 
 */
using LivrETS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    /// <summary>
    /// Represents a TribuTerre books fair organised at each start
    /// of a new semester at the ETS.
    /// </summary>
    public class Fair
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string Trimester { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<FairStep> FairSteps { get; set; }

        [Required]
        public double CommissionOnSale { get; set; }

        [NotMapped]
        public FairStep Phase {
            get
            {
                DateTime now = DateTime.Now;

                FairSteps.OrderBy(stp => stp.StartDateTime);
                foreach (FairStep step in FairSteps)
                {
                    if (DateTime.Compare(now, step.StartDateTime) == 0)
                    {
                        return step;
                    }
                }
                
                return null;
            }
        }

        [NotMapped]
        public string LivrETSID => $"{Trimester}{StartDate.Year}";

        public Fair()
        {
            Offers = new List<Offer>();
            Sales = new List<Sale>();
            FairSteps = new List<FairStep>();
        }

        /// <summary>
        /// Constructor with start and end dates of the fair.
        /// </summary>
        /// <param name="start">Start date of the fair</param>
        /// <param name="end">End date of the fair</param>
        public Fair(DateTime start, DateTime end)
            : this()
        {
            if (end <= start)
            {
                throw new ArgumentException("End must be greater than start");
            }

            StartDate = start;
            EndDate = end;
        }

        public static bool CheckStatusFair()
        {
            List<UserViewModel> listUsersParicipateFair = null;
            //Fair fair = null;
            var now = DateTime.Now;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    listUsersParicipateFair = db.Database.SqlQuery<UserViewModel>(
                            "SELECT u.Email, u.FirstName, u.LastName " +
                            "FROM AspNetUsers u " +
                            "INNER JOIN Offers o ON u.Id = o.ApplicationUser_Id " +
                            "INNER JOIN Fairs f ON f.Id = o.Fair_Id " +
                            "WHERE o.ManagedByFair = 1 " +
                            "AND GETDATE() BETWEEN f.StartDate AND f.EndDate ").ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Fair fair = (
                            from fairdb in db.Fairs
                            join fs in db.FairSteps on fairdb.Id equals fs.FairID
                            where fairdb.StartDate <= now && now <= fairdb.EndDate
                            select fairdb
                        ).FirstOrDefault();

                if (fair == null || listUsersParicipateFair == null)
                    return false;

                DateTime fairRetrievalStartDate = DateTime.Now.AddDays(-10000);
                try
                {
                    fairRetrievalStartDate = fair.FairSteps.Where(step =>
                                            step.Phase == "R").FirstOrDefault().StartDateTime;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                

                if (DateTime.Compare(now.Date, fairRetrievalStartDate.Date) == 0)
                    return NotificationManager.getInstance().sendNotification(
                        new Notification(NotificationOptions.STARTFAIRRETREIVAL, listUsersParicipateFair)
                    );
                else if (DateTime.Compare(now.Date, fairRetrievalStartDate.Date.AddDays(7)) == 0)
                    return NotificationManager.getInstance().sendNotification(
                        new Notification(NotificationOptions.LATEFORRETREIVALAFTERAWEEK, listUsersParicipateFair)
                    );

                return true;

            }
           
        }

        public string TrimesterToString()
        {

            switch(this.Trimester){
                case "H":
                    return "Hiver";
                case "A":
                    return "Automne";
                case "E":
                    return "Été";
                default:
                    return "Pas définie";
            }
        }
    }

}