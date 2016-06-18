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

                switch (Model.Trim().ToUpper())
                {
                    case "VOYAGE200":
                        return CalculatorModel.VOYAGE200;
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

        [Required(ErrorMessage = "Vous devez indiquer la méthode de vente.")]
        public string SellingStrategy { get; set; }
        public bool ForNextFair
        {
            get
            {
                return SellingStrategy.Trim().ToUpper() == "FAIR";
            }
        }
    }
}