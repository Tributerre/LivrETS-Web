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
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.ViewModels
{
    public class FairViewModel
    {
        public float CurrentPhase { get; set; }
        public float NumberOfPhases { get; set; }
        public ApplicationUser User { get; set; }
        public Fair Fair { get; set; }

        [Required(ErrorMessage = "Veuillez entrer le numéro du code à bar du vendeur.")]
        public string UserBarCode { get; set; }
    }
}