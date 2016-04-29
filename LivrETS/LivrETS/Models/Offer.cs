using System;
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
                {
                    throw new ArgumentOutOfRangeException("Price must be positive");
                }
                else
                {
                    _price = value;
                }
            }
        }

        [Required]
        public string Condition { get; set; }
        public DateTime MarkedSoldOn { get; set; }
        public DbSet<OfferImage> Images { get; set; }

        [Required]
        public Article Article { get; set; }

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
            else
            {
                Images.Add(image);
            }
        }
    }
}