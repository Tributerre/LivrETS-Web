using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LivrETS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Veuillez indiquer votre courriel.")]
        [EmailAddress(ErrorMessage = "Le courriel que vous avez indiqué est invalide.")]
        [Display(Name = "Courriel")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Veuilez indiquer votre prénom.")]
        [Display(Name = "Prénom")]
        [MaxLength(length: 256, ErrorMessage = "Maximum 256 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer votre nom.")]
        [Display(Name = "Nom")]
        [MaxLength(length: 256, ErrorMessage = "Maximum 256 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le numéro du code à barres de votre carte étudiante.")]
        [Display(Name = "Code à barres")]
        public string BarCode { get; set; }

        public ExternalLoginConfirmationViewModel() { }

        public ExternalLoginConfirmationViewModel(ApplicationUser user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            BarCode = user.BarCode;
        }
    }

    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Veuilez indiquer votre prénom.")]
        [Display(Name = "Prénom")]
        [MaxLength(length: 256, ErrorMessage = "Maximum 256 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer votre nom.")]
        [Display(Name = "Nom")]
        [MaxLength(length: 256, ErrorMessage = "Maximum 256 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer le numéro du code à barres de votre carte étudiante.")]
        [Display(Name = "Code à barres")]
        public string BarCode { get; set; }
    }


    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }


    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
}
