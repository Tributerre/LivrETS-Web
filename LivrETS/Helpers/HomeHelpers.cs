using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.Helpers
{
    public class HomeHelpers
    {
        public static string ImgArticleHelper(ICollection<OfferImage> images)
        {
            return (images.Count == 0) ? "http://placehold.it/262x202" : images.FirstOrDefault().ThumbnailPathOnDisk;
        }
    }
}