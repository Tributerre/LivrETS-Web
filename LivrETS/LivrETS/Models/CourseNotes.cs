using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class CourseNotes
        : Article
    {
        [Required]
        public string SubTitle { get; set; }

        [Required]
        public string BarCode { get; set; }

        public CourseNotes()
            : base(articleCode: COURSE_NOTES_CODE)
        { }
    }
}