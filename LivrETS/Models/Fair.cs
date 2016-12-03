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

        public DateTime PickingStartDate { get; set; }
        public DateTime PickingEndDate { get; set; }

        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }

        public DateTime RetrievalStartDate { get; set; }
        public DateTime RetrievalEndDate { get; set; }

        [Required]
        public string Trimester { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }

        [Required]
        public double CommissionOnSale { get; set; }

        [NotMapped]
        public FairPhase Phase {
            get
            {
                DateTime now = DateTime.Now;

                if (now < StartDate || now < PickingStartDate)
                {
                    return FairPhase.PREFAIR;
                }
                else if (now >= PickingStartDate && now < PickingEndDate)
                {
                    return FairPhase.PICKING;
                }
                else if (now >= SaleStartDate && now < SaleEndDate)
                {
                    return FairPhase.SALE;
                }
                else if (now >= RetrievalStartDate && now < RetrievalEndDate)
                {
                    return FairPhase.RETRIEVAL;
                }
                else
                {
                    return FairPhase.POSTFAIR;
                }
            }
        }

        [NotMapped]
        public string LivrETSID => $"{Trimester}{StartDate.Year}";

        public Fair()
        {
            Offers = new List<Offer>();
            Sales = new List<Sale>();
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

        /// <summary>
        /// Set the dates for the fair or one of its phases.
        /// </summary>
        /// <param name="start">Start date of the fair or phase</param>
        /// <param name="end">End date of the fair or phase</param>
        /// <param name="forPhase">Phase to set the dates for. Choose PREFAIR or POSTFAIR for fair.</param>
        public Fair SetDates(DateTime start, DateTime end, FairPhase forPhase = FairPhase.PREFAIR)
        {
            if (end <= start)
            {
                throw new ArgumentException("End must be greater than start");
            }

            switch (forPhase)
            {
                case FairPhase.PREFAIR:
                case FairPhase.POSTFAIR:
                    StartDate = start;
                    EndDate = end;
                    break;

                case FairPhase.PICKING:
                    PickingStartDate = start;
                    PickingEndDate = end;
                    break;

                case FairPhase.SALE:
                    SaleStartDate = start;
                    SaleEndDate = end;
                    break;

                case FairPhase.RETRIEVAL:
                    RetrievalStartDate = start;
                    RetrievalEndDate = end;
                    break;
            }

            return this;
        }

        public static bool CheckStatusFair()
        {
            List<ApplicationUser> listAllUsers = null;
            List<ApplicationUser> listUsersParicipateFair = null;
            Fair fair = null;

            using (var db = new ApplicationDbContext())
            {
                var now = DateTime.Now;
                Fair nextFair =  (
                    from fairF in db.Fairs
                    where fairF.StartDate > now
                    orderby fairF.StartDate ascending
                    select fairF
                ).FirstOrDefault();

                Fair currentFair = (
                            from fairdb in db.Fairs
                            where fairdb.StartDate <= now && now <= fairdb.EndDate
                            select fairdb
                        ).FirstOrDefault();

                fair = (currentFair != null) ? currentFair : nextFair;

                listAllUsers = (from user in db.Users
                                select user).ToList();
                listUsersParicipateFair = listAllUsers.Where(user =>
                                        user.Offers.Where(offer =>
                                        offer.ManagedByFair == true) != null).ToList();
            }

            DateTime currentDate = (Convert.ToDateTime(DateTime.Now.ToString()));

            DateTime fairPickingEndDate = (Convert.ToDateTime(fair.PickingEndDate));
            DateTime fairPickingStartDate = (Convert.ToDateTime(fair.PickingStartDate));
            DateTime fairSaleStartDate = (Convert.ToDateTime(fair.SaleStartDate));
            DateTime fairSaleEndDate = (Convert.ToDateTime(fair.SaleEndDate));
            DateTime fairRetrievalStartDate = (Convert.ToDateTime(fair.RetrievalStartDate));
            DateTime fairRetrievalEndDate = (Convert.ToDateTime(fair.RetrievalEndDate));

            if (CompareDate(currentDate, fairPickingStartDate))
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.STARTFAIRPICKING, listAllUsers)
                );
            else if (CompareDate(currentDate, fairPickingEndDate))
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ENDFAIRPICKING, listAllUsers)
                );
            else if (CompareDate(currentDate, fairSaleStartDate))
            {
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.STARTFAIRSALE, listAllUsers)
                );
            }
            else if (CompareDate(currentDate, fairSaleEndDate))
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ENDFAIRSALE, listAllUsers)
                );
            else if (CompareDate(currentDate, fairRetrievalStartDate))
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.STARTFAIRRETREIVAL, listUsersParicipateFair)
                );
            else if (CompareDate(currentDate, fairRetrievalEndDate))
                return NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ENDFAIRRETREIVAL, listUsersParicipateFair)
                );
            return false;
        }

        private static bool CompareDate(DateTime currentDate, DateTime CompareDate)
        {
            return currentDate.Year == currentDate.Year &&
                currentDate.Month == CompareDate.Month &&
                currentDate.Day == CompareDate.Day;
        }
    }

    /// <summary>
    /// Enumerations of the different fair phases. Learn about
    /// those phases in the documentation.
    /// </summary>
    public enum FairPhase
    {
        PREFAIR = 0,
        PICKING,
        SALE,
        RETRIEVAL,
        POSTFAIR
    }
}