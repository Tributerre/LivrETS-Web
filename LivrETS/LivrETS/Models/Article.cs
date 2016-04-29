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

        public Course Course { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GeneratedNumber { get; }

        [NotMapped]
        public readonly string ArticleCode;

        [NotMapped]
        public string LivrETSID => $"{ArticleCode}{GeneratedNumber}";

        public Article()
        {
            FairState = ArticleFairState.UNKNOWN;
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