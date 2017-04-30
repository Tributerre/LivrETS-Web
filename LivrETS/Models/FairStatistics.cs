using LivrETS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        public FairStatistics(){}

        public string GetTotalSalesAmount()
        {
            double result = 0;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<double>("SELECT SUM(o.Price) AS totalCmpt " +
                        "FROM Fairs f " +
                        "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                        "WHERE f.Id = @p0 " +
                        "AND o.MarkedSoldOn <> o.StartDate", this.fair.Id).Single();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result.ToString("0.00");
        }

        
        public List<FlotStatisticsPrice> GetTotalSalesAmountByArticleType()
        {
            List<FlotStatisticsPrice> result = null;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<FlotStatisticsPrice>(
                            "SELECT a.Discriminator as label, SUM(o.Price) as data " +
                            "FROM Fairs f " +
                            "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                            "INNER JOIN Articles a ON o.ArticleId = a.Id " +
                            "WHERE f.Id = @p0 " +
                            "AND o.MarkedSoldOn <> o.StartDate " +
                            "GROUP BY a.Discriminator", this.fair.Id).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public int GetTotalSales()
        {
            return this.offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate).Count();
        }

        public List<FlotStatisticsCount> GetTotalSalesByArticleType() {
            List<FlotStatisticsCount> result = null;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<FlotStatisticsCount>(
                            "SELECT a.Discriminator as label, COUNT(o.Id) as data " +
                            "FROM Fairs f " +
                            "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                            "INNER JOIN Articles a ON o.ArticleId = a.Id " +
                            "WHERE f.Id = @p0 " +
                            "AND o.MarkedSoldOn <> o.StartDate " +
                            "GROUP BY a.Discriminator", this.fair.Id).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public List<TabStatistics> GetTotalSalesBySeller()
        {
            List<TabStatistics> result = null;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<TabStatistics>(
                            "SELECT u.FirstName, u.LastName, SUM(o.Price) as Ventes, " +
                                "COUNT(si.Id) Total FROM AspNetUsers u " +
                                "INNER JOIN Sales s ON u.Id = s.SellerID " +
                                "INNER JOIN SaleItems si ON s.Id = si.sale_Id " +
                                "INNER JOIN Offers o ON o.Id = si.OfferID " +
                                "INNER JOIN Fairs f ON f.Id = o.Fair_id " +
                                "WHERE f.Id = @p0 " +
                                "GROUP BY u.FirstName, u.LastName, u.BarCode", this.fair.Id).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public List<TabStatistics> GetTotalSalesByCourse()
        {
            List<TabStatistics> result = null;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<TabStatistics>(
                            "SELECT c.Acronym, SUM(o.Price) as Ventes, " +
                                "COUNT(si.Id) Total FROM Sales s " +
                                "INNER JOIN SaleItems si ON s.Id = si.sale_Id " +
                                "INNER JOIN Offers o ON o.Id = si.OfferID " +
                                "INNER JOIN Fairs f ON f.Id = o.Fair_id " +
                                "INNER JOIN Articles a ON o.ArticleId = a.Id " +
                                "INNER JOIN Courses c ON a.CourseID = c.Id " +
                                "WHERE f.Id = @p0 " +
                                "GROUP BY c.Acronym", this.fair.Id).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public string GetTotalAmountForLateRetreivals()
        {
            double result = 0;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<double>("SELECT SUM(o.Price) as Total " +
                                "FROM Fairs f " +
                                "INNER JOIN Sales s ON f.Id = s.FairID " +
                                "INNER JOIN SaleItems si ON s.Id = si.Sale_Id " +
                                "INNER JOIN Offers o ON o.Id = si.OfferID " +
                                "WHERE f.Id = @p0 " +
                                "AND o.MarkedSoldOn <> o.StartDate AND f.EndDate < GETDATE()", 
                                this.fair.Id).Single();
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }

            return result.ToString("0.00");
        }

        public List<MorrisFairsStatistic> GetStatsFairs()
        {
            List<MorrisFairsStatistic> result = null;

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    result = db.Database.SqlQuery<MorrisFairsStatistic>(
                            "SELECT " +
                            "f.Trimester trimester, YEAR(f.StartDate) year, " +
                            "(SELECT COUNT(o.Id) " +
                                "FROM Offers o " +
                                "INNER JOIN Articles a ON a.id = o.ArticleID " +
                                "WHERE o.MarkedSoldOn = o.StartDate AND o.Fair_Id = f.Id " +
                                "AND a.DeletedAt = o.StartDate) AS articles, " +
                            "(SELECT COUNT(o.Id) " +
                                "FROM Offers o " +
                                "INNER JOIN Articles a ON a.id = o.ArticleID " +
                                "WHERE o.MarkedSoldOn <> o.StartDate AND o.Fair_Id = f.Id " +
                                "AND a.DeletedAt = o.StartDate) AS articles_sold " +
                            "FROM Fairs f " +
                            "ORDER BY f.StartDate ASC").ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }
    }
}