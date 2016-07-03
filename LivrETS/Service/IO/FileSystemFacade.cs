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
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using PdfSharp.Pdf;

namespace LivrETS.Service.IO
{
    /// <summary>
    /// Facade to the file system.
    /// </summary>
    public class FileSystemFacade
    {
        public const string THUMBNAILS = "Thumbnails";
        public const string ORIGINALS = "Originals";
        public const string TEMP = "Temp";
        public const string RELATIVE_ORIGINALS_PATH = "/Content/Uploads/" + ORIGINALS;
        public const string RELATIVE_THUMBNAILS_PATH = "/Content/Uploads/" + THUMBNAILS;
        public const string RELATIVE_TEMP_ORIGINALS_PATH = "/Content/Uploads/" + ORIGINALS + "/" + TEMP;
        public const string RELATIVE_TEMP_THUMBNAILS_PATH = "/Content/Uploads/" + THUMBNAILS + "/" + TEMP;
        public const string RELATIVE_STICKERS_PDF_PATH = "/Content/PDF/stickers.pdf";

        /// <summary>
        /// Save an uploaded image to disk in the right folder.
        /// </summary>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="image">The image that has been uploaded.</param>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        public static void SaveUploadedImage(string userId, HttpPostedFileBase image, string uploadsPath, out string fullOriginalPath, out string fullThumbnailPath)
        {
            CreateTemp(uploadsPath);
            var imageName = $"{userId}-{Guid.NewGuid()}.{image.FileName.Split('.').Last()}".Trim();
            fullThumbnailPath = Path.Combine(uploadsPath, THUMBNAILS, TEMP, imageName);
            fullOriginalPath = Path.Combine(uploadsPath, ORIGINALS, TEMP, imageName);

            image.SaveAs(fullOriginalPath);

            using (Image originalImage = Image.FromFile(fullOriginalPath),
                thumbnailImage = originalImage.GetThumbnailImage(100, 100, () => { return false; }, IntPtr.Zero))
            {
                thumbnailImage.Save(fullThumbnailPath);
            }
        }

        /// <summary>
        /// Cleans the temporary folders.
        /// </summary>
        /// <param name="uploadsPath">The path to the Uploads folder.</param>
        /// <param name="userId">The ID of the current user.</param>
        public static void CleanTempFolder(string uploadsPath, string userId)
        {
            CreateTemp(uploadsPath);
            var thumbnailsTempPath = Path.Combine(uploadsPath, THUMBNAILS, TEMP);
            var originalsTempPath = Path.Combine(uploadsPath, ORIGINALS, TEMP);

            foreach (var file in 
                Directory.EnumerateFiles(thumbnailsTempPath)
                .Concat(Directory.EnumerateFiles(originalsTempPath))
                .Where(file => Path.GetFileName(file).StartsWith(userId)))
            {
                File.Delete(file);
            }
        }

        public static void DeleteFile(string location)
        {
            if (File.Exists(location))
            {
                File.Delete(location);
            }
        }

        public static void MoveFileToLocation(string oldLocation, string newLocation)
        {
            var newParent = Directory.GetParent(newLocation);

            if (newParent.Exists && File.Exists(oldLocation))
            {
                File.Move(oldLocation, newLocation);
            }
        }

        /// <summary>
        /// Saves a pdf document containing the stickers layout to print
        /// to a file.
        /// </summary>
        /// <param name="document">The pdf document to save.</param>
        /// <param name="pdfPath">The full path to the pdf folder.</param>
        /// <returns></returns>
        public static string SaveStickersDocument(PdfDocument document, string pdfPath)
        {
            document.Save(Path.Combine(pdfPath, "stickers.pdf"));
            return RELATIVE_STICKERS_PDF_PATH;
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