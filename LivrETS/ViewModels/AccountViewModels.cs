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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LivrETS.Models;

namespace LivrETS.ViewModels
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
