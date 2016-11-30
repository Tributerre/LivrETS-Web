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
using System.Linq;
using System.Web;
using LivrETS.Models;
using LivrETS.ViewModels;
using System.Data.SqlClient;
using System.Data;

namespace LivrETS.Repositories
{
    /// <summary>
    /// Only repository of the system for now. Add a new one if needed.
    /// </summary>
    public class LivrETSRepository : IDisposable
    {
        private ApplicationDbContext _db;
        private SqlConnection conn = null;

        public LivrETSRepository()
        {
            _db = new ApplicationDbContext();
            conn = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;" +
                                            "Initial Catalog=aspnet-LivrETS-20160629111902;Integrated " +
                                            "Security=True");
        }

        /*************************** Users ***************************/
        /// <summary>
        /// Gets all the roles 
        /// </summary>
        /// <returns>The roles or null if not found.</returns>
        public IQueryable<Object> GetAllRoles()
        {
            return (from role in _db.Roles
                    select role);
        }

        /// <summary>
        /// Gets all the users 
        /// </summary>
        /// <returns>The Users or null if not found.</returns>
        public IQueryable<Object> GetAllUsersForAdmin()
        {
            return (from user in _db.Users
                        orderby user.FirstName descending
                        select new
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            SubscribedAt = user.SubscribedAt,
                            BarCode = user.BarCode,
                            Role = user.Roles.Join(_db.Roles, userRole => userRole.RoleId,
                                                    role => role.Id, (userRole, role) => role)
                                                    .Select(role => role.Name)
                        });
        }

        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return (from user in _db.Users
                    select user);
        }

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="BarCode">The bar code of the user to retrieve.</param>
        /// <param name="Id">The Id of the user.</param>
        /// <param name="LivrETSID">The LivrETS ID of the user.</param>
        /// <returns>An ApplicationUser or null if not found.</returns>
        public ApplicationUser GetUserBy(string BarCode = null, string Id = null, string LivrETSID = null)
        {
            ApplicationUser userToReturn = null;

            if (BarCode != null)
            {
                userToReturn = (
                    from user in _db.Users
                    where user.BarCode == BarCode
                    select user
                ).FirstOrDefault();
            }
            else if (Id != null)
            {
                userToReturn = (
                    from user in _db.Users
                    where user.Id == Id
                    select user
                ).FirstOrDefault();
            }
            else if (LivrETSID != null)
            {
                userToReturn = (
                    from user in _db.Users
                    where user.LivrETSID == LivrETSID
                    select user
                ).FirstOrDefault();
            }

            return userToReturn;
        }

        /*************************** Fairs ***************************/

        public List<Object> GetStatsFairs()
        {
            SqlDataReader reader = null;
            List<Object> DataList = new List<object>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                
                cmd.CommandText = "SELECT "+
                                    "f.Trimester Trimester, YEAR(f.StartDate) StartYear, " +
                                    "(SELECT COUNT(o.Id) "+
                                        "FROM Offers o "+
                                        "WHERE o.MarkedSoldOn = o.StartDate AND o.Fair_Id = f.Id) AS articles, " +
                                    "(SELECT COUNT(o.Id) "+
                                        "FROM Offers o "+
                                        "WHERE o.MarkedSoldOn <> o.StartDate AND o.Fair_Id = f.Id) AS articlesSold " +
                                    "FROM Fairs f "+
                                    "ORDER BY f.StartDate ASC";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                conn.Open();
                reader = cmd.ExecuteReader();

                
                while (reader.Read()){
                    DataList.Add(new
                                    {
                                        year = reader["Trimester"] +"-"+reader["StartYear"],
                                        articles = reader["articles"],
                                        articles_sold = reader["articlesSold"]
                                    });
                    
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

            return DataList;
        }

        /// <summary>
        /// Gets all the fairs 
        /// </summary>
        /// <returns>The Fairs or null if not found.</returns>
        public IQueryable<Object> GetAllFairs()
        {
            return (from fair in _db.Fairs
                        select new
                        {
                            Id = fair.Id,
                            Trimester = fair.Trimester,
                            NbOffer = fair.Offers.Count(),
                            StartDate = fair.StartDate,
                            EndDate = fair.EndDate
                        });
        }

        public Fair GetFairById(string id)
        {
            return (from fair in _db.Fairs
                    where fair.Id.ToString().Equals(id)
                    select fair).FirstOrDefault();
        }

        /// <summary>
        /// Gets the next not started fair from now.
        /// </summary>
        /// <returns>A Fair or null if not found.</returns>
        public Fair GetNextFair()
        {
            var now = DateTime.Now;
            return (
                from fair in _db.Fairs
                where fair.StartDate > now
                orderby fair.StartDate ascending
                select fair
            ).FirstOrDefault();
        }

        /// <summary>
        /// Get the current fair (current date between StartDate and EndDate) if there is one.
        /// </summary>
        /// <returns>A Fair or null if not found.</returns>
        public Fair GetCurrentFair()
        {
            var now = DateTime.Now;
            return (
                from fairdb in _db.Fairs
                where fairdb.StartDate <= now && now <= fairdb.EndDate
                select fairdb
            ).FirstOrDefault();
        }

        /*************************** Courses ***************************/

        /// <summary>
        /// Returns an enumerable of all courses in the system.
        /// </summary>
        /// <returns>An enumerable of Course. Not Null.</returns>
        public IEnumerable<Course> GetAllCourses()
        {
            return (
                from course in _db.Courses
                orderby course.Acronym ascending
                select course
            );
        }

        /// <summary>
        /// Gets the course associated with a specific acronym.
        /// </summary>
        /// <param name="acronym">The acronym to test.</param>
        /// <returns>A Course or null if not found.</returns>
        public Course GetCourseByAcronym(string acronym)
        {
            return (
                from course in _db.Courses
                where course.Acronym == acronym
                select course
            ).FirstOrDefault();
        }

        /// <summary>
        /// Adds a new course to the system.
        /// </summary>
        /// <param name="acronym">The acronym of the new course.</param>
        public void AddNewCourse(string acronym, string title)
        {
            Course course = new Course() { Acronym = acronym, Title = title };
            _db.Courses.Add(course);
            _db.SaveChanges();
        }


        /*************************** Offer ***************************/
        public ApplicationUser GetOfferByUser(Offer offer)
        {
            List<ApplicationUser> listUsers = this.GetAllUsers().ToList();

            foreach(ApplicationUser user in listUsers)
            {
                if(user.Offers.Contains(offer)){
                    return user;
                }
            }

            return null;
        }
        public void DeleteFair(string id)
        {
            Fair fair = this.GetFairById(id);

            if (fair.Offers != null)
            {
                Offer[] tabOffer = fair.Offers.ToArray();
                for (int j = 0; j < tabOffer.Length; j++)
                {
                    Offer offer = tabOffer[j];
                    

                    if (offer.Images != null)
                    {
                        OfferImage[] tabOfferImg = offer.Images.ToArray();
                        for (int i = 0; i < tabOfferImg.Length; i++)
                        {
                            _db.OfferImage.Remove(tabOfferImg[i]);
                            tabOfferImg[i].Delete();
                        }
                    }
                    _db.Articles.Remove(offer.Article);
                    _db.Offers.Remove(offer);
                }
            }
            
            _db.Fairs.Remove(fair);
            
            _db.SaveChanges();
        }

        /*public ApplicationUser GetUserByOffer(string Id)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;" +
                "Initial Catalog=aspnet-LivrETS-20160629111902;Integrated Security=True");

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            cmd.CommandText = "SELECT * " +
                                    "FROM Offers o " +
                                    "WHERE o.Id = o. AND o.Fair_Id = f.Id) AS articles, " +
                                "(SELECT COUNT(o.Id) " +
                                    "FROM Offers o " +
                                    "WHERE o.MarkedSoldOn <> o.StartDate AND o.Fair_Id = f.Id) AS articlesSold " +
                                "FROM Fairs f " +
                                "ORDER BY f.StartDate ASC";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            reader = cmd.ExecuteReader();

            List<Object> DataList = new List<object>();
            while (reader.Read())
            {
                DataList.Add(new
                {
                    year = reader["Trimester"] + "-" + reader["StartYear"],
                    articles = reader["articles"],
                    articles_sold = reader["articlesSold"]
                });

            }
            con.Close();

            return DataList;
            return null;
        }*/

        /// <summary>
        /// Gets all the offers 
        /// 
        /// </summary>
        /// <returns>The offers or null if not found.</returns>
        public IEnumerable<Offer> GetAllOffers()
        {
            return (from offer in _db.Offers
                    orderby offer.StartDate descending
                    select offer).ToList();
        }

        /// <summary>
        /// Gets all the offers for Admin page
        /// 
        /// </summary>
        /// ///  <param name="priceMin">minimal price</param>
        /// ///  <param name="priceMax">maximal price</param>
        /// ///  <param name="itemSearch">the element search</param>
        /// ///  <param name="sortOrder">the element sort</param>
        /// <returns>The offers or null if not found.</returns>
        public IEnumerable<Offer> GetAllOffers(double priceMin, double priceMax, 
            string itemSearch = null, string sortOrder = null)
        {
            List<Offer> offers = (from offer in _db.Offers
                                    where offer.Price >= priceMin && offer.Price <= priceMax &&
                                    DateTime.Compare(offer.Article.DeletedAt, offer.StartDate) == 0 &&
                                    !offer.ManagedByFair
                                    orderby offer.StartDate descending
                                    select offer).ToList();
            IEnumerable<Offer> results = offers;

            if (itemSearch != null)
            {
                string[] tabItemSearch = itemSearch.Split(':');
                string sigle = null;
                string elt = null; 

                if(tabItemSearch.Count() == 2)
                {
                    sigle = tabItemSearch[0];
                    elt = tabItemSearch[1];
                }else
                {
                    sigle = "cr";
                    elt = tabItemSearch[0];
                }

                if(sigle.Equals("cr"))
                {
                    results = offers.Where(offer => offer.Title.Contains(elt));
                }else if (sigle.Equals("sg"))
                {
                    results = offers.Where(offer => offer.Article.Course.Acronym.Contains(elt));
                }

                //return offers;
            }else if(sortOrder != null)
            {
                if (sortOrder.Equals("DateDesc"))
                {
                    results = offers.OrderByDescending(offer => offer.StartDate);
                } else if (sortOrder.Equals("PriceDesc"))
                {
                    results = offers.OrderByDescending(offer => offer.Price);
                } else if (sortOrder.Equals(Article.BOOK_CODE) || sortOrder.Equals(Article.CALCULATOR_CODE) || 
                    sortOrder.Equals(Article.COURSE_NOTES_CODE))
                {
                    results = offers.Where(offer => offer.Article.ArticleCode.Equals(sortOrder));
                }
            }

            return results;
        }

        public IQueryable<Offer> GetAllAdminOffers()
        {
            return (from offer in _db.Offers
                                  orderby offer.StartDate descending
                                  select offer);
        }

        /// <summary>
        /// Delete offer 
        /// </summary>
        /// <param name="offerIds">
        /// Offer Id array
        /// </param>
        public bool DeleteOffer(string[] Ids)
        {
            try
            {
                foreach (string Id in Ids)
                {
                    Offer offer = GetOfferBy(Id);

                    if(offer.Images != null)
                    {
                        OfferImage[] tabOffer = offer.Images.ToArray();
                        for(int i = 0; i<tabOffer.Length; i++)
                        {
                            _db.OfferImage.Remove(tabOffer[i]);
                            tabOffer[i].Delete();
                        }
                    }
                    _db.Articles.Remove(offer.Article);
                    _db.Offers.Remove(offer);
                }

                _db.SaveChanges();

                return true;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Disable offer 
        /// </summary>
        /// <param name="offerIds">Offer Id array</param>
        public bool DisableOffer(string[] offerIds)
        {
            for (int i = 0; i < offerIds.Length; i++)
            {
                Offer offer = this.GetOfferBy(offerIds[i]);
                if (offer == null)
                    return false;

                this.AttachToContext(offer);
                offer.Article.DeletedAt = DateTime.Now;
            }

            this.Update();

            return true;
        }

        public int CountArticle(string category = null)
        {
            IQueryable<Offer> offers = from offer in _db.Offers
                                       select offer;

            if (category != null)
            {
                offers.Where(offer => offer.Article.ArticleCode.Equals(category));
            }

            return offers.Count();
        }

        /// <summary>
        /// Add a new offer to a user.
        /// </summary>
        /// <param name="toUser">The id of the user in which to add the offer.</param>
        /// <param name="offer">The offer to add.</param>
        public void AddOffer(Offer offer, string toUser)
        {
            var user = (
                from userDb in _db.Users
                where userDb.Id.ToString() == toUser
                select userDb
            ).FirstOrDefault();

            if (user != null)
            {
                user.Offers.Add(offer);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets an article by one of its unique fields.
        /// </summary>
        /// <param name="Id">The Id of the article.</param>
        /// <param name="LivrETSID">The LivrETS ID of the article.</param>
        /// <returns>An Article or null if not found.</returns>
        public Article GetArticleBy(string Id = null, string LivrETSID = null)
        {
            Article articleToReturn = null;

            if (Id != null)
            {
                articleToReturn = (
                    from article in _db.Articles
                    where article.Id.ToString() == Id
                    select article
                ).FirstOrDefault();
            }
            else if (LivrETSID != null)
            {
                articleToReturn = (
                    from article in _db.Articles
                    where article.LivrETSID == LivrETSID
                    select article
                ).FirstOrDefault();
            }

            return articleToReturn;
        }

        /// <summary>
        /// Gets an article by one of its unique fields.
        /// </summary>
        /// <param name="Id">The Id of the article.</param>
        /// <param name="LivrETSID">The LivrETS ID of the article.</param>
        /// <returns>An Article or null if not found.</returns>
        public string[] GetISBNByArticle(string articleId)
        {
            SqlDataReader reader = null;
            
            string[] data = { null, null, null};
            try
            {
                string query = "SELECT ISBN, BarCode, Model " +
                            "FROM Articles " +
                            "WHERE Id = @id ";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlParameter param = new SqlParameter();
                cmd.Parameters.AddWithValue("@id", articleId);
                conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    data[0] = reader["ISBN"].ToString();
                    data[1] = reader["BarCode"].ToString();
                    data[2] = reader["Model"].ToString();
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

            return data;
        }

        /// <summary>
        /// Gets an offer.
        /// </summary>
        /// <param name="Id">The Id of the offer.</param>
        /// <returns>An Offer or null if not found.</returns>
        public Offer GetOfferBy(string Id)
        {
            if (Id == null)
                return null;

            return (
                    from offer in _db.Offers
                    where offer.Id.ToString() == Id
                    select offer
                ).FirstOrDefault();

        }

        public bool ConcludeSell(string[] offerIds)
        {
            for (int i = 0; i < offerIds.Length; i++)
            {
                Offer offer = this.GetOfferBy(offerIds[i]);
                if (offer == null)
                    return false;
                
                this.AttachToContext(offer);
                offer.Article.MarkAsSold();
                offer.MarkedSoldOn = DateTime.Now;               
            }

            this.Update();

            return true;
        }

        public bool DeleteOfferImg(string[] imgIds)
        {
            for (int i = 0; i < imgIds.Length; i++)
            {
                string id = imgIds[i];
                OfferImage img = (from image in _db.OfferImage
                                  where image.Id.ToString().Equals(id)
                                  select image).FirstOrDefault();

                if (img == null)
                    return false;

                _db.OfferImage.Remove(img);
                img.Delete();
            }

            _db.SaveChanges();

            return true;
        }

        /// <summary>
        /// Gets an offer associated with some data.
        /// </summary>
        /// <param name="userLivrETSID">The LivrETS ID of the associated user.</param>
        /// <param name="andArticleLivrETSID">The article LivrETS ID of the associated article.</param>
        /// <returns>An Offer or null if not found.</returns>
        public Offer GetOfferAssociatedWith(string userLivrETSID, string andArticleLivrETSID)
        {
            Offer offerToReturn = null;

            if (userLivrETSID != null && andArticleLivrETSID != null)
            {
                var user = GetUserBy(LivrETSID: userLivrETSID);
                offerToReturn = user.Offers.Where(offer => offer.Article.LivrETSID == andArticleLivrETSID).FirstOrDefault();
            }

            return offerToReturn;
        }

        /// <summary>
        /// Attaches a model to a context. Does nothing if the object isn't
        /// a model of the domain.
        /// </summary>
        /// <param name="modelToAttach">The model to attach.</param>
        public void AttachToContext(object modelToAttach)
        {
            if (modelToAttach is Offer)
            {
                _db.Offers.Attach(modelToAttach as Offer);
            }
        }

        public void Update()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
