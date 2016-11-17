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
using System.Data.Entity;
using System.Linq;

namespace LivrETS.Models
{
    /// <summary>
    /// Represents an offer of selling from a user to the community.
    /// </summary>
    public class Offer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        private float _price;
        [Required]
        public float Price {
            get
            {
                return _price;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Price must be positive");
                else
                    _price = value;
            }
        }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public string Condition { get; set; }
        public DateTime MarkedSoldOn { get; set; }
        public virtual ICollection<OfferImage> Images { get; set; }

        [Required]
        public virtual Article Article { get; set; }
        [ForeignKey(nameof(Article))]
        public Guid ArticleID { get; set; }

        [Required]
        public bool ManagedByFair { get; set; }

        [NotMapped]
        public bool Sold => MarkedSoldOn != StartDate;

        public Offer()
        {
            Images = new List<OfferImage>();
            Id = Guid.NewGuid();
            ManagedByFair = false;
        }

        /// <summary>
        /// Use this method to add an image to the offer.
        /// </summary>
        /// <param name="image">The image to add</param>
        public void AddImage(OfferImage image)
        {
            if (Images.Count() == 5)
            {
                throw new ArgumentException("Maximum 5 images");
            }

            Images.Add(image);
        }
    }
}