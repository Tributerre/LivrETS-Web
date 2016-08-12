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
        public long GeneratedNumber { get; private set; }

        [Required]
        [MaxLength(1)]
        public readonly string ArticleCode;

        [NotMapped]
        public string LivrETSID => $"A{ArticleCode}{GeneratedNumber}";

        [NotMapped]
        public const string BOOK_CODE = "L";
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

        public Article MarkAsPicked()
        {
            FairState = ArticleFairState.PICKED;
            return this;
        }

        public Article MarkAsSold()
        {
            FairState = ArticleFairState.SOLD;
            return this;
        }

        public Article MarkAsRetrieved()
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