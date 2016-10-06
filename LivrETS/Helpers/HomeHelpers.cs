using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.Helpers
{
    public class HomeHelpers
    {

        /// <summary>
        /// Get path image for article in home page 
        /// </summary>
        /// /// <param name="image">the image of article</param>
        /// <returns>The path of image</returns>
        public static string ThbImgArticleHelper(OfferImage image)
        {
            return (image == null) ? "http://placehold.it/262x202" : image.RelativeThumbnailPath;
        }

        /// <summary>
        /// Get path image for article in detail page 
        /// </summary>
        /// /// <param name="image">the image of article</param>
        /// /// <param name="alt">alt element of image</param>
        /// /// <param name="flag">flag element for first image in caroussel</param>
        /// <returns>The path of image</returns>
        public static IHtmlString ImgArticleHelper(OfferImage image, string alt, bool flag)
        {
            var img = (image == null) ? "http://placehold.it/590x590" : image.RelativeOriginalPath;
            string active = ""; 

            if (flag) active = "active";

            string html = String.Format("<div class='item {0}'><a href='#'><img src='{1}' data-echo='{2}' alt='{3}' class='img-responsive img-caroussel' /></a></div>", active, img, img, alt);

            return new HtmlString(html);
        }

        /// <summary>
        /// Get a short name of article in home page
        /// </summary>
        /// /// <param name="text">name of article</param>
        /// /// <param name="nb_tronc">text size accept to home pagee</param>
        /// <returns>The name short</returns>
        public static string TroncText(string text, int nb_tronc)
        {
            if(text.Count() >= nb_tronc)
            {
                string textTmp = null;
                textTmp = text.Substring(0, nb_tronc);
                return textTmp + "...";
            }
            return text;
        }
    }
}