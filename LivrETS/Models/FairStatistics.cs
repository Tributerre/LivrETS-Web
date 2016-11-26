using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class FairStatistics
    {
        private Fair fair = null;
        private ICollection<Offer> offers = null;
        private SqlConnection conn = null;

        public FairStatistics(Fair fair)
        {
            conn = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;" +
                "Initial Catalog=aspnet-LivrETS-20160629111902;Integrated Security=True");
            this.fair = fair;
            this.offers = fair.Offers;
        }

        public FairStatistics()
        {
            conn = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;" +
                "Initial Catalog=aspnet-LivrETS-20160629111902;Integrated Security=True");
        }

        public string GetTotalSalesAmount()
        {
            /*double total = 0;
            List<Offer> offersSoldOn = offers.
                        Where(offer => offer.MarkedSoldOn != offer.StartDate).ToList();

            foreach (Offer offer in offersSoldOn)
                total += offer.Price;

            return total;*/
            SqlDataReader reader = null;
            string result = null;
            try
            {
                conn.Open();
                string query = "SELECT SUM(o.Price) AS totalCmpt " +
                            "FROM Fairs f " +
                            "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                            "WHERE f.Id = @fairId " +
                            "AND o.MarkedSoldOn <> o.StartDate";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@fairId", this.fair.Id);


                reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                     result = reader["totalCmpt"].ToString();
                }
                

            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

        public List<Object> GetTotalSalesAmountByArticleType(/*string articleType*/)
        {
            /*double total = 0;

            List<Offer> offersSoldOn = offers.Where(offer => 
                                offer.MarkedSoldOn != offer.StartDate &&
                                offer.Article.TypeName.Equals(articleType)
                                ).ToList();

            foreach (Offer offer in offersSoldOn)
                total += offer.Price;

            return total;*/
            List<Object> DataList = new List<object>();
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                string query = "SELECT a.Discriminator, SUM(o.Price) as Total " +
                            "FROM Fairs f " +
                            "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                            "INNER JOIN Articles a ON o.ArticleId = a.Id "+
                            "WHERE f.Id = @fairId " +
                            "AND o.MarkedSoldOn <> o.StartDate "+
                            "GROUP BY a.Discriminator";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@fairId", this.fair.Id);


                reader = cmd.ExecuteReader();

                while (reader.Read())
                    DataList.Add(new
                    {
                        label = reader["Discriminator"],
                        data = reader["Total"]
                    });


            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return DataList;
        }

        public int GetTotalSales()
        {
            return this.offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate).Count();
        }

        public List<Object> GetTotalSalesByArticleType(/*string articleType*/) {
            /*return offers.Where(offer =>
                                offer.MarkedSoldOn != offer.StartDate &&
                                offer.Article.TypeName.Equals(articleType)
                                ).Count();*/
            List<Object> DataList = new List<object>();
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                string query = "SELECT a.Discriminator, COUNT(o.Id) as Total " +
                            "FROM Fairs f " +
                            "INNER JOIN Offers o ON f.Id = o.Fair_Id " +
                            "INNER JOIN Articles a ON o.ArticleId = a.Id " +
                            "WHERE f.Id = @fairId " +
                            "AND o.MarkedSoldOn <> o.StartDate " +
                            "GROUP BY a.Discriminator";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@fairId", this.fair.Id);


                reader = cmd.ExecuteReader();

                while (reader.Read())
                    DataList.Add(new
                    {
                        label = reader["Discriminator"],
                        data = reader["Total"]
                    });


            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return DataList;
        }

        public List<Object> GetTotalSalesBySeller()
        {
            List<Object> DataList = new List<object>();
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                string query = "SELECT u.FirstName, u.LastName, SUM(o.Price) as Ventes, "+
                                "COUNT(si.Id) Total FROM AspNetUsers u "+
                                "INNER JOIN Sales s ON u.Id = s.SellerID "+
                                "INNER JOIN SaleItems si ON s.Id = si.sale_Id "+
                                "INNER JOIN Offers o ON o.Id = si.OfferID "+
                                "GROUP BY u.FirstName, u.LastName, u.BarCode";

                SqlCommand cmd = new SqlCommand(query, conn);


                reader = cmd.ExecuteReader();

                while (reader.Read())
                    DataList.Add(new
                    {
                        FirstName = reader["FirstName"],
                        LastName = reader["LastName"],
                        Ventes = reader["Ventes"],
                        Total = reader["Total"]
                    });
                
            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return DataList;
        }

        public List<Object> GetTotalSalesByCourse()
        {
            List<Object> DataList = new List<object>();
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                string query = "SELECT c.Acronym, SUM(o.Price) as Ventes, " +
                                "COUNT(si.Id) Total FROM Sales s " +
                                "INNER JOIN SaleItems si ON s.Id = si.sale_Id " +
                                "INNER JOIN Offers o ON o.Id = si.OfferID " +
                                "INNER JOIN Articles a ON o.ArticleId = a.Id " +
                                "INNER JOIN Courses c ON a.CourseID = c.Id " +
                                "GROUP BY c.Acronym";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@fairId", this.fair.Id);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                    DataList.Add(new
                    {
                        Acronym = reader["Acronym"],
                        Ventes = reader["Ventes"],
                        Total = reader["Total"]
                    });

            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return DataList;
        }

        public string GetTotalAmountForLateRetreivals()
        {
            List<Object> DataList = new List<object>();
            SqlDataReader reader = null;
            string result = "0";

            try
            {
                conn.Open();
                string query = "SELECT SUM(o.Price) as Total " +
                                "FROM Fairs f " +
                                "INNER JOIN Sales s ON f.Id = s.FairID " +
                                "INNER JOIN SaleItems si ON s.Id = si.Sale_Id " +
                                "INNER JOIN Offers o ON o.Id = si.OfferID " +
                                "WHERE f.Id = @fairId "+
                                "AND o.MarkedSoldOn <> o.StartDate AND f.EndDate < GETDATE()";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@fairId", this.fair.Id);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                    if(reader["Total"].ToString() != "")
                        result = reader["Total"].ToString();

            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }
    }
}