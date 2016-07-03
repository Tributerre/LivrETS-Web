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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using Zen.Barcode;
using LivrETS.Service.IO;

namespace LivrETS.Service
{
    public class PrintManager
    {
        public const int MAX_NUMBER_OF_STICKERS_PER_SHEET = 14;
        public const int NUMBER_OF_COLUMNS = 2;
        public static readonly int MAX_NUMBER_OF_STICKERS_PER_COLUMN = MAX_NUMBER_OF_STICKERS_PER_SHEET / NUMBER_OF_COLUMNS;

        public struct StickerInfo
        {
            public string ArticleTitle { get; set; }
            public float OfferPrice { get; set; }
            public string ArticleLivrETSID { get; set; }
            public string FairLivrETSID { get; set; }
            public string UserLivrETSID { get; set; }
        }

        /// <summary>
        /// Generates the pdf document containing the specified stickers. No specs of the sheet are
        /// provided with the system. It should be a good idea.
        /// </summary>
        /// <param name="pdfPath">The full path to the pdf folder.</param>
        /// <param name="numberOfStickersLeftInSheet">The number of stickers left on the sheet used.</param>
        /// <param name="stickers">The stickers to layout in the document.</param>
        /// <returns></returns>
        public static string GeneratePreview(
            string pdfPath,
            int numberOfStickersLeftInSheet,
            IEnumerable<StickerInfo> stickers)
        {
            var numberOfBlanks = MAX_NUMBER_OF_STICKERS_PER_SHEET - numberOfStickersLeftInSheet;
            var numberOfStickersLeft = stickers.Count();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gtx = XGraphics.FromPdfPage(page);
            var nbFormatInfo = new CultureInfo("fr-CA", false).NumberFormat;
            
            var firstLeftRect = new XRect(
                XUnit.FromCentimeter(0),
                XUnit.FromCentimeter(1.9),
                XUnit.FromCentimeter(10.2),
                XUnit.FromCentimeter(3.4)
            );

            page.Size = PdfSharp.PageSize.Letter;
            page.TrimMargins = new TrimMargins()
            {
                Left = XUnit.FromMillimeter(4),
                Top = XUnit.FromCentimeter(2) + XUnit.FromMillimeter(1),
                Right = XUnit.FromMillimeter(4),
                Bottom = XUnit.FromCentimeter(2) + XUnit.FromMillimeter(1)
            };

            // O(n^2)
            for (int currentColumn = 0; currentColumn < NUMBER_OF_COLUMNS && numberOfStickersLeft > 0; currentColumn++)
            {
                var currentXPos = currentColumn * (firstLeftRect.Width + XUnit.FromMillimeter(9));

                for (int position = 0; position < MAX_NUMBER_OF_STICKERS_PER_COLUMN && numberOfStickersLeft > 0; position++)
                {
                    if (numberOfBlanks == 0)
                    {
                        var sticker = stickers.ElementAt(stickers.Count() - numberOfStickersLeft);
                        var LivrETSID = $"{sticker.FairLivrETSID}-{sticker.UserLivrETSID}-{sticker.ArticleLivrETSID}";
                        var barcode = GenerateLivrETSBarCode(LivrETSID);
                        var padding = XUnit.FromMillimeter(2);
                        var currentYPos = (firstLeftRect.Height + XUnit.FromMillimeter(2.5)) * position;
                        var currentRect = new XRect(
                            currentXPos,
                            currentYPos,
                            firstLeftRect.Width,
                            firstLeftRect.Height
                        );
                        var fairTextRect = new XRect(
                            currentRect.Left + padding,
                            currentRect.Top + padding,
                            currentRect.Width / 2,
                            XUnit.FromMillimeter(5)
                        );
                        var livretsIdRect = new XRect(
                            currentRect.Left,
                            currentRect.Bottom - XUnit.FromMillimeter(4),
                            currentRect.Width,
                            XUnit.FromMillimeter(4)
                        );
                        var barCodeRect = new XRect(
                            currentRect.Left + padding,
                            livretsIdRect.Top - XUnit.FromMillimeter(6.2),
                            currentRect.Width - 2 * padding,
                            XUnit.FromMillimeter(6)
                        );
                        var priceRect = new XRect(
                            fairTextRect.Right,
                            currentRect.Top + padding,
                            currentRect.Width / 2,
                            fairTextRect.Height
                        );
                        var titleRect = new XRect(
                            currentRect.Left + padding,
                            fairTextRect.Bottom,
                            currentRect.Width - 2 * padding,
                            barCodeRect.Top - fairTextRect.Bottom
                        );

                        gtx.DrawString(
                            $"Foire {sticker.FairLivrETSID}",
                            new XFont("Arial", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            fairTextRect,
                            XStringFormats.TopLeft
                        );
                        gtx.DrawString(
                            sticker.OfferPrice.ToString("C", nbFormatInfo),
                            new XFont("Arial", 12, XFontStyle.Regular),
                            XBrushes.Black,
                            priceRect,
                            XStringFormats.TopLeft
                        );
                        gtx.DrawString(
                            sticker.ArticleTitle,
                            new XFont("Arial", 10, XFontStyle.Regular),
                            XBrushes.Black,
                            titleRect,
                            XStringFormats.TopLeft
                        );
                        gtx.DrawImage(
                            XImage.FromGdiPlusImage(barcode),
                            barCodeRect
                        );
                        gtx.DrawString(
                            LivrETSID,
                            new XFont("Arial", 9, XFontStyle.Regular),
                            XBrushes.Black,
                            livretsIdRect,
                            XStringFormats.TopCenter
                        );
                        numberOfStickersLeft--;
                    }
                    else
                    {
                        numberOfBlanks--;
                    }
                }
            }

            return FileSystemFacade.SaveStickersDocument(document, pdfPath);
        }

        /// <summary>
        /// Generates a Code 39 bar code from the complete LivrETSID from TRIBSTD01-2016
        /// </summary>
        /// <param name="LivrETSID">See TRIBSTD01-2016 for how to construct the Id.</param>
        /// <returns>An image of the barcode.</returns>
        private static Image GenerateLivrETSBarCode(string LivrETSID)
        {
            Code93BarcodeDraw barCodeDrawer = BarcodeDrawFactory.Code93WithChecksum;
            return barCodeDrawer.Draw(LivrETSID, maxBarHeight: (int)XUnit.FromMillimeter(6));
        }
    }
}