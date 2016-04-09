using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LivrETS.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        public DateTime SubscribedAt { get; set; }
        
        [Required]
        public string BarCode { get; set; }
        
        [Key]
        public string ID { get; set; }
        
        public int GeneratedNumber { get; set; }
        
        [NotMapped]
        public string LivrETSID 
        {
            get 
            {
                return $"{FirstName[0].ToString().ToUpper()}{LastName[0].ToString().ToUpper()}{ID}";
            }
        }
    }
}
