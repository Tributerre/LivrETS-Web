using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
        public List<OfferImage> Images { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}