using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.Helpers
{
    public class HomeHelpers
    {
        public static string ThbImgArticleHelper(OfferImage images)
        {
            return (images == null) ? "http://placehold.it/262x202" : images.ThumbnailPathOnDisk;
        }

        public static IHtmlString ImgArticleHelper(OfferImage images, string alt, bool flag)
        {
            var img = (images == null) ? "http://placehold.it/590x590" : images.PathOnDisk;
            string active = null; 

            if (flag) active = "active";
            string html = String.Format("<div class='item {0}'><a href='#'><img src='{1}' data-echo='{2}' alt='{3}' class='img-responsive img-caroussel' /></a></div>", active, img, img, alt );

            return new HtmlString(html);
        }
    }
}