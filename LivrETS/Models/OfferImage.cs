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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Web;
using LivrETS.Service.IO;

namespace LivrETS.Models
{
    public class OfferImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string PathOnDisk { get; set; }
        [Required]
        public string ThumbnailPathOnDisk { get; set; }

        [NotMapped]
        public string RelativeOriginalPath
        {
            get
            {
                
                if (Array.Exists(PathOnDisk.Split(Path.DirectorySeparatorChar), component => component == FileSystemFacade.TEMP))
                {
                    return $"{FileSystemFacade.RELATIVE_TEMP_ORIGINALS_PATH}/{Path.GetFileName(PathOnDisk)}";
                }
                else
                {
                    return $"{FileSystemFacade.RELATIVE_ORIGINALS_PATH}/{Path.GetFileName(PathOnDisk)}";
                }
            }
        }
        [NotMapped]
        public string RelativeThumbnailPath
        {
            get
            {
                if (Array.Exists(ThumbnailPathOnDisk.Split(Path.DirectorySeparatorChar), component => component == FileSystemFacade.TEMP))
                {
                    return $"{FileSystemFacade.RELATIVE_TEMP_THUMBNAILS_PATH}/{Path.GetFileName(ThumbnailPathOnDisk)}";
                }
                else
                {
                    return $"{FileSystemFacade.RELATIVE_THUMBNAILS_PATH}/{Path.GetFileName(ThumbnailPathOnDisk)}";
                }
            }
        }

        public OfferImage()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Saves an image on disk in a temporary location.
        /// </summary>
        /// <param name="image">The ID of the current user.</param>
        /// <param name="userId">The image that has been uploaded.</param>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        public void SaveImageTemporarily(HttpPostedFileBase image, string userId, string uploadsPath)
        {
            string fullOriginalPath = string.Empty;
            string fullThumbnailPath = string.Empty;

            FileSystemFacade.SaveUploadedImage(userId, image, uploadsPath, out fullOriginalPath, out fullThumbnailPath);
            PathOnDisk = fullOriginalPath;
            ThumbnailPathOnDisk = fullThumbnailPath;
        }

        /// <summary>
        /// Move the image from temp to the permanent location.
        /// </summary>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        public void MovePermanently(string uploadsPath)
        {
            var fileName = Path.GetFileName(PathOnDisk);
            var newOriginalLocation = Path.Combine(uploadsPath, FileSystemFacade.ORIGINALS, fileName);
            var newThumbnsilLocation = Path.Combine(uploadsPath, FileSystemFacade.THUMBNAILS, fileName);

            FileSystemFacade.MoveFileToLocation(PathOnDisk, newOriginalLocation);
            FileSystemFacade.MoveFileToLocation(ThumbnailPathOnDisk, newThumbnsilLocation);
            PathOnDisk = newOriginalLocation;
            ThumbnailPathOnDisk = newThumbnsilLocation;
        }

        /// <summary>
        /// Deletes the image on disk.
        /// </summary>
        public void Delete()
        {
            FileSystemFacade.DeleteFile(PathOnDisk);
            FileSystemFacade.DeleteFile(ThumbnailPathOnDisk);
        }
    }
}