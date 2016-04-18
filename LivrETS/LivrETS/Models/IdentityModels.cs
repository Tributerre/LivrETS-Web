using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivrETS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(length: 256)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(length: 256)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SubscribedAt { get; set; }

        [Required]
        public string BarCode { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GeneratedNumber { get; }

        [NotMapped]
        public string LivrETSID => $"{FirstName[0].ToString().ToUpper()}{LastName[0].ToString().ToUpper()}{GeneratedNumber}";

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}