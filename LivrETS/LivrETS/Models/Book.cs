using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            : base(articleCode: "B")
        { }
    }
}