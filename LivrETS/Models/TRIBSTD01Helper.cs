using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

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
        public ApplicationUser GetSeller()
        {
            Match match = _regex.Match(_livretsID);
            var sellerInitials = match.Groups["sellerinitials"];
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