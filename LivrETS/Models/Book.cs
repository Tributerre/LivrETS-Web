using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class Book
        : Article
    {
        [Required]
        public string ISBN { get; set; }

        public Book()
            : base(articleCode: BOOK_CODE)
        { }

    }
    public class BookApi
    {
        public static bool Search(string isbn, string title)
        {
            BooksService _booksService = new BooksService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleBookApiKey"],
                ApplicationName = ConfigurationManager.AppSettings["GoogleProject"]
            });
            var result = _booksService.Volumes.List(isbn).Execute();

            if (result != null && result.Items != null)
            {
                var items = result.Items;
                
                for (int i =0; i < items.Count(); i++)
                {
                    var name = items[i].VolumeInfo.Title;
                    if (items[i].VolumeInfo.Title.ToLower().IndexOf(title.ToLower().Trim()) >= 0)
                        return true;
                }
                    
            }
            /*if (result != null)
                return true;*/
            return false;
        }
    }

}