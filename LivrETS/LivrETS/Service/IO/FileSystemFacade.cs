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
using System.IO;
using System.Drawing;

namespace LivrETS.Service.IO
{
    /// <summary>
    /// Facade to the file system.
    /// </summary>
    public class FileSystemFacade
    {
        public static readonly string THUMBNAILS = "Thumbnails";
        public static readonly string ORIGINALS = "Originals";
        public static readonly string TEMP = "Temp";

        /// <summary>
        /// Save an uploaded image to disk in the right folder.
        /// </summary>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="image">The image that has been uploaded.</param>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        /// <returns>A tuple containing the original (first) and thumbnail (second) paths.</returns>
        public static Tuple<string, string> SaveUploadedImage(string userId, HttpPostedFileBase image, string uploadsPath)
        {
            CreateTemp(uploadsPath);
            var imageName = $"{userId}-{Guid.NewGuid()}.{image.FileName.Split('.').Last()}".Trim();
            var thumbnailPath = Path.Combine(uploadsPath, THUMBNAILS, TEMP, imageName);
            var originalPath = Path.Combine(uploadsPath, ORIGINALS, TEMP, imageName);
            var virtualThumbnailPath = $"/Content/Uploads/{THUMBNAILS}/{TEMP}/{imageName}";
            var virtualOriginalPath = $"/Content/Uploads/{ORIGINALS}/{TEMP}/{imageName}";

            image.SaveAs(originalPath);

            using (Image originalImage = Image.FromFile(originalPath),
                thumbnailImage = originalImage.GetThumbnailImage(100, 100, () => { return false; }, IntPtr.Zero))
            {
                thumbnailImage.Save(thumbnailPath);
            }

            return new Tuple<string, string>(virtualOriginalPath, virtualThumbnailPath);
        }

        /// <summary>
        /// Cleans the temporary folders.
        /// </summary>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        public static void CleanTempFolder(string uploadsPath)
        {
            CreateTemp(uploadsPath);
            var thumbnailsTempPath = Path.Combine(uploadsPath, THUMBNAILS, TEMP);
            var originalsTempPath = Path.Combine(uploadsPath, ORIGINALS, TEMP);

            foreach (var file in Directory.EnumerateFiles(thumbnailsTempPath).Concat(Directory.EnumerateFiles(originalsTempPath)))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Creates the temporary folders if they don't exist.
        /// </summary>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        private static void CreateTemp(string uploadsPath)
        {
            var thumbnailsTempPath = Path.Combine(uploadsPath, THUMBNAILS, TEMP);
            var originalsTempPath = Path.Combine(uploadsPath, ORIGINALS, TEMP);

            Directory.CreateDirectory(thumbnailsTempPath);
            Directory.CreateDirectory(originalsTempPath);
        }
    }
}