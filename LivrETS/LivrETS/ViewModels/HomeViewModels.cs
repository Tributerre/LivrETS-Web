using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LivrETS.Models;
using System.ComponentModel.DataAnnotations;

namespace LivrETS.ViewModels
{
    public class ArticleViewModel
    {
        public string Model { get; set; }
        public CalculatorModel CalculatorModel
        {
            get
            {
                if (Model == null)
                {
                    return CalculatorModel.NSPIRE;
                }

                switch (Model)
                {
                    case "VOYAGE200":
                        return CalculatorModel.VOYAGE200;
                    case "NSPIRE":
                        return CalculatorModel.NSPIRE;
                    default:
                        return CalculatorModel.NSPIRE;
                }
            }
        }

        public string ISBN { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer un titre à votre offre.")]
        [MaxLength(256, ErrorMessage = "Maximum de 256 caractères.")]
        public string Title { get; set; }
        public string Course { get; set; }
        
        [Required(ErrorMessage = "Veuillez indiquer la condition de l'article.")]
        public string Condition { get; set; }

        [Required]
        public bool ForNextFair { get; set; }
    }
}