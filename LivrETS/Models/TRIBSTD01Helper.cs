using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using LivrETS.Repositories;

namespace LivrETS.Models
{
    /// <summary>
    /// This class is an helper for the standard 01 of Tributerre. This
    /// standard is available on request.
    /// </summary>
    public class TRIBSTD01Helper
    {
        private const string REGEX_EXPR = @"^(?<semesterletter>A|H|E)(?<year>\d{4})-(?<sellerinitials>[A-Z]{2})(?<sellernumber>\d+)-(?<article>A(L|N|C))(?<articlenumber>\d+)$";

        private string _livretsID;
        private Regex _regex;

        public string SellerLivrETSID
        {
            get
            {
                Match match = _regex.Match(_livretsID);
                var sellerInitials = match.Groups["sellerinitials"];
                var sellerNumber = match.Groups["sellernumber"];
                return sellerInitials.Value + sellerNumber.Value;
            }
        }

        public string ArticleLivrETSID
        {
            get
            {
                Match match = _regex.Match(_livretsID);
                var articleData = match.Groups["article"];
                var articleNumber = match.Groups["articlenumber"];
                return articleData.Value + articleNumber.Value;
            }
        }

        /// <summary>
        /// Constructor of the helper.
        /// </summary>
        /// <param name="livretsID">The LivrETS ID to parse.</param>
        /// <exception cref="RegexNoMatchException">Thrown if the expression doesn't match anything handled.</exception>
        public TRIBSTD01Helper(string livretsID)
        {
            _livretsID = livretsID;
            _regex = new Regex(REGEX_EXPR);

            if (!_regex.Match(livretsID).Success)
                throw new RegexNoMatchException("The expression is not a LivrETSID. (A9999-AA9+-AA9+)");
        }

        /// <summary>
        /// Get the seller associated with this LivrETS ID.
        /// </summary>
        /// <returns>An ApplicationUser or null of not found.</returns>
        public ApplicationUser GetSeller()
        {
            ApplicationUser user = null;
            using (var repo = new LivrETSRepository())
            {
                user = repo.GetUserBy(LivrETSID: SellerLivrETSID);
            }

            return user;
        }

        /// <summary>
        /// Get the offer associated with this LivrETS ID.
        /// </summary>
        /// <returns>An Offer or null if not found.</returns>
        public Offer GetOffer()
        {

            Offer offer = null;
            using (var repo = new LivrETSRepository())
            {
                offer = repo.GetOfferAssociatedWith(userLivrETSID: SellerLivrETSID, andArticleLivrETSID: ArticleLivrETSID);
            }
            return offer;
        }

        /// <summary>
        /// Generates the LivrETS ID of a user. It should be called when the user has already
        /// been persisted and has not this already. It should also be called only at the creation
        /// process of the user and never changed after that.
        /// </summary>
        /// <param name="user">The user from which to generate the ID.</param>
        /// <returns>The LivrETS ID generated.</returns>
        /// <exception cref="ArgumentException">Thrown when the ID of the user isn't null.</exception>
        public static string GenerateLivrETSID(ApplicationUser user)
        {
            if (user.LivrETSID != null)
            {
                throw new ArgumentException("The ID should be unique and never set again.");
            }

            var firstInitial = user.FirstName[0].ToString().ToUpper();
            var lastInitial = user.LastName[0].ToString().ToUpper();

            return $"{firstInitial}{lastInitial}{user.GeneratedNumber}";
        }

        /// <summary>
        /// Gets the LivrETS ID associated with a user, an article and a fair.
        /// </summary>
        /// <param name="user">A ApplicationUser from the database.</param>
        /// <param name="article">A Article from the database.</param>
        /// <param name="fair">A Fair from the database.</param>
        /// <returns></returns>
        public static string LivrETSIDOf(ApplicationUser user, Article article, Fair fair)
        {
            return $"{fair.LivrETSID}-{user.LivrETSID}-{article.LivrETSID}";
        }
    }

    /// <summary>
    /// Can be thrown if no match is found in expression.
    /// </summary>
    public class RegexNoMatchException : Exception
    {
        public RegexNoMatchException() { }

        public RegexNoMatchException(string message)
            : base(message)
        {}
    }
}