using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class FairStatistics
    {
        private Fair fair = null;
        private ICollection<Offer> offers = null;

        public FairStatistics(Fair fair)
        {
            this.fair = fair;
            this.offers = fair.Offers;
        }

        public double GetTotalSalesAmount()
        {
            double total = 0;
            List<Offer> offersSoldOn = offers.
                        Where(offer => offer.MarkedSoldOn != offer.StartDate).ToList();

            foreach (Offer offer in offersSoldOn)
                total += offer.Price;

            return total;
        }

        public double GetTotalSalesAmountByArticleType(string articleType)
        {
            double total = 0;

            List<Offer> offersSoldOn = offers.Where(offer => 
                                offer.MarkedSoldOn != offer.StartDate &&
                                offer.Article.TypeName.Equals(articleType)
                                ).ToList();

            foreach (Offer offer in offersSoldOn)
                total += offer.Price;

            return total;
        }

        public int GetTotalSales()
        {
            return this.offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate).Count();
        }

        public int GetTotalSalesByArticleType(string articleType) {
            return offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate &&
                                offer.Article.TypeName.Equals(articleType)
                                ).Count();
        }

        public int GetTotalSalesByCourse(string courseId)
        {
            return offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate &&
                                offer.Article.Course.Id.Equals(courseId)
                                ).Count();
        }

        public int GetTotalSalesBySeller(string sellerId)
        {
            using (var db = new ApplicationDbContext())
            {
                ApplicationUser seller = (from user in db.Users
                        where user.Id.Equals(sellerId)
                        select user).FirstOrDefault();

                return seller.Sales.Count();
            }
        }

        /*public double GetTotalAmountForLateRetreivals()
        {
            double total = 0;
            

        }*/
    }
}