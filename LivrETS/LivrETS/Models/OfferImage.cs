using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class OfferImage
    {
        [NotMapped]
        public static readonly string RelativeOriginalsPath = "/Content/Uploads/Originals";
        [NotMapped]
        public static readonly string RealtiveThumbnailsPath = "/Content/Uploads/Thumbnails";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string PathOnDisk { get; set; }
        [Required]
        public string ThumbnailPathOnDisk { get; set; }

        /// <summary>
        /// Saves an image to disk and populates the paths.
        /// </summary>
        /// <param name="image">The image uploaded</param>
        /// <param name="userId">The Id of the user that have uploaded it</param>
        /// <param name="absoluteOriginalsPath">The absolute path to the originals</param>
        /// <param name="absoluteThumbnailsPath">The absolute path to the thumbnails</param>
        public void SaveImage(
            HttpPostedFileBase image,
            string userId,
            string absoluteOriginalsPath,
            string absoluteThumbnailsPath)
        {
            var fileName = $"{new Guid().ToString()}.{image.FileName.Split('.').Last()}";
            var imageOriginalPath = Path.Combine(absoluteOriginalsPath, userId, fileName);
            var imageThumbnailPath = Path.Combine(absoluteThumbnailsPath, userId, fileName);
            image.SaveAs(imageOriginalPath);

            using (var srcImage = Image.FromFile(imageOriginalPath))
            {
                var dstImage = srcImage.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);
                dstImage.Save(imageThumbnailPath);
                dstImage.Dispose();
            }

            PathOnDisk = imageOriginalPath;
            ThumbnailPathOnDisk = imageThumbnailPath;
        }
    }
}