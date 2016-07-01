using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public abstract class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public ArticleFairState FairState { get; set; }

        [ForeignKey(nameof(Course))]
        public Guid CourseID { get; set; }
        public virtual Course Course { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GeneratedNumber { get; }

        [Required]
        [MaxLength(1)]
        public readonly string ArticleCode;

        [NotMapped]
        public string LivrETSID => $"{ArticleCode}{GeneratedNumber}";

        [NotMapped]
        public const string BOOK_CODE = "B";
        [NotMapped]
        public const string CALCULATOR_CODE = "C";
        [NotMapped]
        public const string COURSE_NOTES_CODE = "N";
        [NotMapped]
        public string TypeName
        {
            get
            {
                switch (ArticleCode)
                {
                    case BOOK_CODE:
                        return "Livre";
                    case CALCULATOR_CODE:
                        return "Calculatrice";
                    case COURSE_NOTES_CODE:
                        return "Notes de cours";
                    default:
                        return string.Empty;
                }
            }
        }

        public Article()
        {
            FairState = ArticleFairState.UNKNOWN;
            Id = Guid.NewGuid();
        }

        public Article(string articleCode)
            : this()
        {
            ArticleCode = articleCode;
        }

        public Article MarkedAsPicked()
        {
            FairState = ArticleFairState.PICKED;
            return this;
        }

        public Article MarkedAsSold()
        {
            FairState = ArticleFairState.SOLD;
            return this;
        }

        public Article MarkedAsRetrieved()
        {
            FairState = ArticleFairState.RETREIVED;
            return this;
        }
    }


    public enum ArticleFairState
    {
        UNKNOWN = 0,
        PICKED,
        SOLD,
        RETREIVED
    }
}